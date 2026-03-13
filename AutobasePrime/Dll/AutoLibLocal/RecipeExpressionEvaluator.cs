using System;
using System.Collections.Generic;

namespace AutoLibLocal
{
    /// <summary>
    /// 레시피 조건식 파서 및 평가기 (ISA-88 표현식 기반 조건)
    /// 문법:
    ///   expression := or_expr
    ///   or_expr    := and_expr ('||' and_expr)*
    ///   and_expr   := not_expr ('&&' not_expr)*
    ///   not_expr   := '!' not_expr | comparison
    ///   comparison := value (('==' | '!=' | '>' | '<' | '>=' | '<=') value)?
    ///   value      := '$' TAG_NAME | NUMBER | STRING | '(' expression ')'
    /// </summary>
    public static class RecipeExpressionEvaluator
    {
        #region 토큰

        enum TokenType
        {
            TagRef,       // $TAG_NAME
            Number,       // 123, 45.67
            StringLiteral,// 'hello', "hello"
            LParen,       // (
            RParen,       // )
            And,          // &&
            Or,           // ||
            Not,          // !
            Equal,        // ==
            NotEqual,     // !=
            Greater,      // >
            Less,         // <
            GreaterEqual, // >=
            LessEqual,    // <=
            EOF
        }

        class Token
        {
            public TokenType Type;
            public string Value;
            public Token(TokenType type, string value) { Type = type; Value = value; }
        }

        #endregion

        #region 렉서

        static List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            int i = 0;
            int len = expression.Length;

            while (i < len)
            {
                char c = expression[i];

                // 공백
                if (char.IsWhiteSpace(c)) { i++; continue; }

                // 태그 참조: $TAG_NAME
                if (c == '$')
                {
                    i++;
                    int start = i;
                    while (i < len && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;
                    if (i == start)
                        throw new FormatException("Empty tag name after '$' at position " + (i - 1));
                    tokens.Add(new Token(TokenType.TagRef, expression.Substring(start, i - start)));
                    continue;
                }

                // 숫자
                if (char.IsDigit(c) || (c == '-' && i + 1 < len && char.IsDigit(expression[i + 1]) && (tokens.Count == 0 || IsOperator(tokens[tokens.Count - 1].Type))))
                {
                    int start = i;
                    if (c == '-') i++;
                    while (i < len && (char.IsDigit(expression[i]) || expression[i] == '.'))
                        i++;
                    tokens.Add(new Token(TokenType.Number, expression.Substring(start, i - start)));
                    continue;
                }

                // 문자열 리터럴
                if (c == '\'' || c == '"')
                {
                    char quote = c;
                    i++;
                    int start = i;
                    while (i < len && expression[i] != quote)
                        i++;
                    if (i >= len)
                        throw new FormatException("Unterminated string literal starting at position " + (start - 1));
                    tokens.Add(new Token(TokenType.StringLiteral, expression.Substring(start, i - start)));
                    i++; // skip closing quote
                    continue;
                }

                // 연산자
                if (c == '(' ) { tokens.Add(new Token(TokenType.LParen, "(")); i++; continue; }
                if (c == ')' ) { tokens.Add(new Token(TokenType.RParen, ")")); i++; continue; }
                if (c == '!' && i + 1 < len && expression[i + 1] == '=') { tokens.Add(new Token(TokenType.NotEqual, "!=")); i += 2; continue; }
                if (c == '!' ) { tokens.Add(new Token(TokenType.Not, "!")); i++; continue; }
                if (c == '&' && i + 1 < len && expression[i + 1] == '&') { tokens.Add(new Token(TokenType.And, "&&")); i += 2; continue; }
                if (c == '|' && i + 1 < len && expression[i + 1] == '|') { tokens.Add(new Token(TokenType.Or, "||")); i += 2; continue; }
                if (c == '=' && i + 1 < len && expression[i + 1] == '=') { tokens.Add(new Token(TokenType.Equal, "==")); i += 2; continue; }
                if (c == '>' && i + 1 < len && expression[i + 1] == '=') { tokens.Add(new Token(TokenType.GreaterEqual, ">=")); i += 2; continue; }
                if (c == '<' && i + 1 < len && expression[i + 1] == '=') { tokens.Add(new Token(TokenType.LessEqual, "<=")); i += 2; continue; }
                if (c == '>' ) { tokens.Add(new Token(TokenType.Greater, ">")); i++; continue; }
                if (c == '<' ) { tokens.Add(new Token(TokenType.Less, "<")); i++; continue; }

                throw new FormatException("Unexpected character '" + c + "' at position " + i);
            }

            tokens.Add(new Token(TokenType.EOF, ""));
            return tokens;
        }

        static bool IsOperator(TokenType type)
        {
            return type == TokenType.And || type == TokenType.Or || type == TokenType.Not ||
                   type == TokenType.Equal || type == TokenType.NotEqual ||
                   type == TokenType.Greater || type == TokenType.Less ||
                   type == TokenType.GreaterEqual || type == TokenType.LessEqual ||
                   type == TokenType.LParen;
        }

        #endregion

        #region 파서 + 평가

        class Parser
        {
            List<Token> _tokens;
            int _pos;
            bool _validateOnly;

            public Parser(List<Token> tokens, bool validateOnly)
            {
                _tokens = tokens;
                _pos = 0;
                _validateOnly = validateOnly;
            }

            Token Current => _tokens[_pos];

            Token Consume(TokenType expected)
            {
                if (Current.Type != expected)
                    throw new FormatException("Expected " + expected + " but got " + Current.Type + " ('" + Current.Value + "')");
                return _tokens[_pos++];
            }

            public double ParseExpression()
            {
                double result = ParseOr();
                if (Current.Type != TokenType.EOF)
                    throw new FormatException("Unexpected token '" + Current.Value + "' after expression");
                return result;
            }

            double ParseOr()
            {
                double left = ParseAnd();
                while (Current.Type == TokenType.Or)
                {
                    _pos++;
                    double right = ParseAnd();
                    // short-circuit: true(nonzero) || anything = true
                    left = (left != 0 || right != 0) ? 1.0 : 0.0;
                }
                return left;
            }

            double ParseAnd()
            {
                double left = ParseNot();
                while (Current.Type == TokenType.And)
                {
                    _pos++;
                    double right = ParseNot();
                    left = (left != 0 && right != 0) ? 1.0 : 0.0;
                }
                return left;
            }

            double ParseNot()
            {
                if (Current.Type == TokenType.Not)
                {
                    _pos++;
                    double val = ParseNot();
                    return val == 0 ? 1.0 : 0.0;
                }
                return ParseComparison();
            }

            double ParseComparison()
            {
                double left = ParseValue();
                TokenType op = Current.Type;

                if (op == TokenType.Equal || op == TokenType.NotEqual ||
                    op == TokenType.Greater || op == TokenType.Less ||
                    op == TokenType.GreaterEqual || op == TokenType.LessEqual)
                {
                    _pos++;
                    double right = ParseValue();

                    switch (op)
                    {
                        case TokenType.Equal: return Math.Abs(left - right) < 0.0001 ? 1.0 : 0.0;
                        case TokenType.NotEqual: return Math.Abs(left - right) >= 0.0001 ? 1.0 : 0.0;
                        case TokenType.Greater: return left > right ? 1.0 : 0.0;
                        case TokenType.Less: return left < right ? 1.0 : 0.0;
                        case TokenType.GreaterEqual: return left >= right ? 1.0 : 0.0;
                        case TokenType.LessEqual: return left <= right ? 1.0 : 0.0;
                    }
                }
                return left;
            }

            double ParseValue()
            {
                if (Current.Type == TokenType.Number)
                {
                    double val;
                    if (!double.TryParse(Current.Value, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out val))
                        throw new FormatException("Invalid number: " + Current.Value);
                    _pos++;
                    return val;
                }

                if (Current.Type == TokenType.StringLiteral)
                {
                    // 문자열 리터럴은 비교 시에만 의미 있음 (숫자 변환 시도)
                    string s = Current.Value;
                    _pos++;
                    double numVal;
                    if (double.TryParse(s, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out numVal))
                        return numVal;
                    // 문자열은 해시값으로 비교 (단순 수치 비교 불가)
                    return s.GetHashCode();
                }

                if (Current.Type == TokenType.TagRef)
                {
                    string tagName = Current.Value;
                    _pos++;

                    if (_validateOnly) return 0; // 검증 모드에서는 태그 읽기 안 함

                    // 태그 현재값 읽기
                    try
                    {
                        if (!TagLib.IsTagExist(tagName)) return 0;
                        int[] tagPos = new int[1];
                        TagPublicClass tp = TagLib.GetStructPublic(tagName, ref tagPos);
                        object currObj = tp.GetCurr();
                        if (currObj == null) return 0;

                        double dVal;
                        if (double.TryParse(currObj.ToString(), System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out dVal))
                            return dVal;

                        // bool 처리
                        bool bVal;
                        if (bool.TryParse(currObj.ToString(), out bVal))
                            return bVal ? 1.0 : 0.0;

                        return 0;
                    }
                    catch
                    {
                        return 0;
                    }
                }

                if (Current.Type == TokenType.LParen)
                {
                    _pos++;
                    double val = ParseOr();
                    Consume(TokenType.RParen);
                    return val;
                }

                throw new FormatException("Unexpected token: " + Current.Type + " ('" + Current.Value + "')");
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// 표현식 런타임 평가 — true(1.0)/false(0.0) 반환
        /// </summary>
        public static bool Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return true;
            var tokens = Tokenize(expression);
            var parser = new Parser(tokens, false);
            double result = parser.ParseExpression();
            return result != 0;
        }

        /// <summary>
        /// 표현식 문법 검증 — null이면 유효, 그 외 오류 메시지
        /// </summary>
        public static string Validate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;
            try
            {
                var tokens = Tokenize(expression);
                var parser = new Parser(tokens, true);
                parser.ParseExpression();
                return null; // 유효
            }
            catch (FormatException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return "Unexpected error: " + ex.Message;
            }
        }

        #endregion
    }
}
