using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OPCUA.Client.Core
{

//double[] {1,2,3}	[1, 2, 3]
//큰 배열	[1, 2, 3, 4, … (128)]
//Matrix	Matrix[3×3]: [[1,2,3],[4,5,6],[7,8,9]]
//깨진 Matrix	<Matrix: dimension mismatch (3×3 != 8)>
//Bad Status	BadNotConnected (0x80350000)
//ExtensionObject	<ExtensionObject: ns=2;i=3001, Encoding=Binary>double[] {1,2,3}	[1, 2, 3]
//큰 배열	[1, 2, 3, 4, … (128)]
//Matrix	Matrix[3×3]: [[1,2,3],[4,5,6],[7,8,9]]
//깨진 Matrix	<Matrix: dimension mismatch (3×3 != 8)>
//Bad Status	BadNotConnected (0x80350000)
//ExtensionObject	<ExtensionObject: ns=2;i=3001, Encoding=Binary>

    public static class OpcUaValueFormatter
    {
        private const int MaxArrayPreview = 8;
        private const int MaxBytePreview = 32;

        // =========================
        // Entry Point (절대 안 터짐)
        // =========================
        public static string FormatSafe(
            object value,
            StatusCode status,
            TypeInfo typeInfo)
        {
            try
            {
                // StatusCode 먼저
                if (StatusCode.IsBad(status))
                {
                    return $"{status} (0x{status.Code:X8})";
                }

                return FormatValueSafe(value, typeInfo);
            }
            catch
            {
                return "<Value: format error>";
            }
        }

        // =========================
        // IPC 전송용: Variant를 직렬화 가능한 값으로 변환
        // =========================
        public static object ConvertToSerializable(object value)
        {
            try
            {
                if (value == null)
                    return null;

                // 기본 타입은 그대로
                var type = value.GetType();
                if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                    return value;

                // DateTime은 그대로 (JSON 직렬화 가능)
                if (value is DateTime)
                    return value;

                // 배열 처리
                if (value is Array arr && !(value is byte[]))
                {
                    // 작은 배열만 직렬화, 큰 배열은 문자열로
                    if (arr.Length <= MaxArrayPreview)
                    {
                        var list = new object[arr.Length];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            list[i] = ConvertToSerializable(arr.GetValue(i));
                        }
                        return list;
                    }
                    else
                    {
                        return FormatArraySafe(arr);
                    }
                }

                // 복잡한 타입은 문자열로 변환
                if (value is ExtensionObject || value is Matrix || value is QualifiedName ||
                    value is LocalizedText || value is StatusCode)
                {
                    return FormatValueSafe(value, null);
                }

                // ByteString
                if (value is byte[] bytes)
                    return FormatByteString(bytes);

                // Enum
                if (type.IsEnum)
                    return FormatEnumSafe(value);

                // 기타는 문자열로
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return "<Value: conversion error>";
            }
        }

        // =========================
        // Value Dispatcher
        // =========================
        private static string FormatValueSafe(object value, TypeInfo typeInfo)
        {
            try
            {
                if (value == null)
                    return string.Empty;

                if (value is Matrix m)
                    return FormatMatrixSafe(m);

                if (value is Array arr)
                    return FormatArraySafe(arr);

                if (value is byte[] bytes)
                    return FormatByteString(bytes);

                if (value is StatusCode sc)
                    return $"{sc} (0x{sc.Code:X8})";

                if (value is DateTime dt)
                    return dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                if (value is QualifiedName qn)
                    return $"[ns={qn.NamespaceIndex}, name=\"{qn.Name}\"]";

                if (value is LocalizedText lt)
                    return $"\"{lt.Text}\" ({lt.Locale})";

                if (value is ExtensionObject eo)
                    return FormatExtensionObjectSafe(eo);

                if (value.GetType().IsEnum)
                    return FormatEnumSafe(value);

                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return "<Value: invalid>";
            }
        }

        // =========================
        // Array
        // =========================
        private static string FormatArraySafe(Array arr)
        {
            try
            {
                int count = Math.Min(arr.Length, MaxArrayPreview);
                var sb = new StringBuilder();

                sb.Append('[');

                for (int i = 0; i < count; i++)
                {
                    if (i > 0) sb.Append(", ");

                    object v = arr.GetValue(i);
                    sb.Append(v != null ? FormatValueSafe(v, null) : "null");
                }

                if (arr.Length > MaxArrayPreview)
                    sb.Append($", … ({arr.Length})");

                sb.Append(']');
                return sb.ToString();
            }
            catch
            {
                return "<Array: format error>";
            }
        }

        // =========================
        // Matrix
        // =========================
        private static string FormatMatrixSafe(Matrix m)
        {
            try
            {
                if (m == null)
                    return "<Matrix: null>";

                if (m.Elements == null || m.Dimensions == null || m.Dimensions.Length == 0)
                    return "<Matrix: invalid structure>";

                // N차원
                if (m.Dimensions.Length != 2)
                {
                    var dims = string.Join("×", m.Dimensions);
                    return $"<Matrix: {m.Dimensions.Length}D [{dims}]>";
                }

                int rows = m.Dimensions[0];
                int cols = m.Dimensions[1];
                int expected = rows * cols;

                if (expected != m.Elements.Length)
                    return $"<Matrix: dimension mismatch ({rows}×{cols} != {m.Elements.Length})>";

                var sb = new StringBuilder();
                sb.Append($"Matrix[{rows}×{cols}]: ");

                int maxRows = Math.Min(rows, 3);
                int maxCols = Math.Min(cols, 4);

                sb.Append('[');

                for (int r = 0; r < maxRows; r++)
                {
                    if (r > 0) sb.Append(", ");
                    sb.Append('[');

                    for (int c = 0; c < maxCols; c++)
                    {
                        if (c > 0) sb.Append(", ");

                        int idx = r * cols + c;
                        object v = m.Elements.GetValue(idx);
                        sb.Append(v != null ? FormatValueSafe(v, null) : "null");
                    }

                    if (cols > maxCols)
                        sb.Append(", …");

                    sb.Append(']');
                }

                if (rows > maxRows)
                    sb.Append(", …");

                sb.Append(']');
                return sb.ToString();
            }
            catch
            {
                return "<Matrix: format error>";
            }
        }

        // =========================
        // ByteString
        // =========================
        private static string FormatByteString(byte[] bytes)
        {
            try
            {
                int len = Math.Min(bytes.Length, MaxBytePreview);
                var hex = BitConverter.ToString(bytes, 0, len).Replace("-", "");

                if (bytes.Length > MaxBytePreview)
                    return $"0x{hex}… ({bytes.Length} bytes)";

                return $"0x{hex}";
            }
            catch
            {
                return "<ByteString: format error>";
            }
        }

        // =========================
        // ExtensionObject
        // =========================
        private static string FormatExtensionObjectSafe(ExtensionObject eo)
        {
            try
            {
                if (eo == null)
                    return "null";

                try
                {
                    if (eo.Body != null)
                        return FormatDecodedBody(eo.Body);

                    // Body가 없으면 의미 있는 정보 없음
                    return "(empty)";
                }
                catch (Exception ex)
                {
                    return $"<invalid: {ex.GetType().Name}>";
                }
            }
            catch
            {
                return "<ExtensionObject: invalid>";
            }
        }

        private static string FormatDecodedBody(object body)
        {
            var type = body.GetType();

            var sb = new StringBuilder();
            sb.Append("");
            sb.Append(type.FullName);

            var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (props.Length > 0)
            {
                sb.Append(" | ");

                foreach (var p in props)
                {
                    object val;
                    try
                    {
                        val = p.GetValue(body, null);
                    }
                    catch
                    {
                        val = "<error>";
                    }

                    sb.Append(p.Name);
                    sb.Append("=");
                    sb.Append(val ?? "null");
                    sb.Append(" | ");
                }
            }

            sb.Append("");

            return sb.ToString();
        }

        // =========================
        // Enum
        // =========================
        private static string FormatEnumSafe(object enumVal)
        {
            try
            {
                int iv = Convert.ToInt32(enumVal);
                string name = Enum.GetName(enumVal.GetType(), enumVal);

                return name != null ? $"{name} ({iv})" : $"Unknown({iv})";
            }
            catch
            {
                return "<Enum: invalid>";
            }
        }
    }
}