using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools.OldDefine;
using System.IO;
using NetTools;
using System.Collections.Generic;
using AutoLib;

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class VAR_STRUCT
    {
        public string name;		// 변수 이름
        public EnumVarType type;
        public int size;			// 크기
        public object val = null;
    }

    public enum EnumVarType
    {
        VAR_TYPE_unknown = -1,
        VAR_TYPE_sbyte,
        VAR_TYPE_byte,
        VAR_TYPE_char,
        VAR_TYPE_short,
        VAR_TYPE_ushort,
        VAR_TYPE_int,
        VAR_TYPE_uint,
        VAR_TYPE_long,
        VAR_TYPE_ulong,
        VAR_TYPE_float,
        VAR_TYPE_double,
        VAR_TYPE_string,
        VAR_TYPE_bool,
        VAR_TYPE_object,
    }

    /// <summary>
    /// Summary description for ScriptClass.
    /// </summary>
    /// 
    //[Serializable]
    public class ScriptClass
    {
        //[NonSerialized]
        public bool bHandOperation;	// 이 플래그가 ON이면 사용자가 버턴이나 키 등으로 스크립트를 실행했다.

        //[NonSerialized]
        public UserControl formParent;
        //string sFileName;
        string sDescription;
        int nScanTime;
        //[NonSerialized]
        object returnValue;
        //[NonSerialized]
        Color lColorBack;

        //[NonSerialized]
        bool bErrorFlag;	// 프로그램 이상 유무
        string sProgramm;	// 프로그램이 담겨있는 버퍼
        //[NonSerialized]
        int nTraceLine;
        //[NonSerialized]
        string sErrorMessage;

        public List<object> arrayVar;
        //[NonSerialized]
        public int nKeyValue;	// key 값.


        public string Script
        {
            get
            {
                if (sProgramm == null) return "";
                else return sProgramm;
            }
        }

        public bool IsError() { return bErrorFlag; }

        public void ClearError() { bErrorFlag = false; }

        public string Description
        {
            set
            {
                sDescription = value;
            }
            get
            {
                return sDescription;
            }
        }

        public int GetScanTime() { return nScanTime; }
        public void SetScanTime(int time) { nScanTime = time; }

        public void SetHandOperation()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                bHandOperation = true;
            }
        }

        public ScriptClass()
        {
            //
            // TODO: Add constructor logic here
            //
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                bHandOperation = false;	// 이플래그가 ON이면 사용자가 버턴이나 키등으로 스크립트를 실행했다.
            }
            nScanTime = 1;
            bErrorFlag = false;
            sProgramm = null;
            nTraceLine = 0;		// 현재 수행중인 라인
            arrayVar = new List<object>();
            returnValue = 0;
            lColorBack = Colors.White;
            formParent = null;
            sErrorMessage = null;
            nKeyValue = 0;
        }

        public void Run(UserControl form)
        {
            formParent = form;

            bErrorFlag = false;
            if (sProgramm == null) return;		// 할당한 프로그램이 없다.

            nTraceLine = 0;

            RecurseFunction(sProgramm, 1);	// 1 = pre_retn
        }

        static Type TypeString = typeof(string);//Type.GetType("System.String");
        static Type TypeChar = typeof(char);//Type.GetType("System.Char");
        static Type TypeSByte = typeof(sbyte);//Type.GetType("System.SByte");
        static Type TypeByte = typeof(byte);//Type.GetType("System.Byte");
        static Type TypeInt16 = typeof(short);//Type.GetType("System.Int16");
        static Type TypeUInt16 = typeof(ushort);//Type.GetType("System.UInt16");
        static Type TypeInt32 = typeof(int);//Type.GetType("System.Int32");
        static Type TypeUInt32 = typeof(uint);//Type.GetType("System.UInt32");
        static Type TypeInt64 = typeof(long);//Type.GetType("System.Int64");
        static Type TypeUInt64 = typeof(ulong);//Type.GetType("System.UInt64");
        static Type TypeDouble = typeof(double);//Type.GetType("System.Double");
        static Type TypeSingle = typeof(float);//Type.GetType("System.Single");
        static Type TypeBool = typeof(bool);

        EnumVarType GetObjectVarType(object obj)
        {
            if (obj.GetType() == TypeString)
                return EnumVarType.VAR_TYPE_string;
            else if (obj.GetType() == TypeChar)
                return EnumVarType.VAR_TYPE_char;
            else if (obj.GetType() == TypeSByte)
                return EnumVarType.VAR_TYPE_sbyte;
            else if (obj.GetType() == TypeByte)
                return EnumVarType.VAR_TYPE_byte;
            else if (obj.GetType() == TypeInt16)
                return EnumVarType.VAR_TYPE_short;
            else if (obj.GetType() == TypeUInt16)
                return EnumVarType.VAR_TYPE_ushort;
            else if (obj.GetType() == TypeInt32)
                return EnumVarType.VAR_TYPE_int;
            else if (obj.GetType() == TypeUInt32)
                return EnumVarType.VAR_TYPE_uint;
            else if (obj.GetType() == TypeInt64)
                return EnumVarType.VAR_TYPE_long;
            else if (obj.GetType() == TypeUInt64)
                return EnumVarType.VAR_TYPE_ulong;
            else if (obj.GetType() == TypeDouble)
                return EnumVarType.VAR_TYPE_double;
            else if (obj.GetType() == TypeSingle)
                return EnumVarType.VAR_TYPE_float;
            else if (obj.GetType() == TypeBool)
                return EnumVarType.VAR_TYPE_bool;

            else
                return EnumVarType.VAR_TYPE_unknown;
        }

        public double GetValueDouble(object obj)
        {
            if (obj == null) return 0;

            EnumVarType type = GetObjectVarType(obj);

            if (type == EnumVarType.VAR_TYPE_sbyte)
                return (sbyte)obj;
            else if (type == EnumVarType.VAR_TYPE_byte)
                return (byte)obj;
            else if (type == EnumVarType.VAR_TYPE_char)
                return (char)obj;
            else if (type == EnumVarType.VAR_TYPE_short)
                return (short)obj;
            else if (type == EnumVarType.VAR_TYPE_ushort)
                return (ushort)obj;
            else if (type == EnumVarType.VAR_TYPE_int)
                return (int)obj;
            else if (type == EnumVarType.VAR_TYPE_uint)
                return (uint)obj;
            else if (type == EnumVarType.VAR_TYPE_long)
                return (long)obj;
            else if (type == EnumVarType.VAR_TYPE_ulong)
                return (ulong)obj;
            else if (type == EnumVarType.VAR_TYPE_double)
                return (double)obj;
            else if (type == EnumVarType.VAR_TYPE_float)
                return (float)obj;
            else if (type == EnumVarType.VAR_TYPE_string)
                return ConvertTool.ToDouble((string)obj);
            else if (type == EnumVarType.VAR_TYPE_bool)
                return (bool)obj ? 1 : 0;

            return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
        }

        public int GetValueInt(object obj)
        {
            if (obj == null) return 0;

            EnumVarType type = GetObjectVarType(obj);

            if (type == EnumVarType.VAR_TYPE_sbyte)
                return (sbyte)obj;
            else if (type == EnumVarType.VAR_TYPE_byte)
                return (byte)obj;
            else if (type == EnumVarType.VAR_TYPE_char)
                return (char)obj;
            else if (type == EnumVarType.VAR_TYPE_short)
                return (short)obj;
            else if (type == EnumVarType.VAR_TYPE_ushort)
                return (ushort)obj;
            else if (type == EnumVarType.VAR_TYPE_int)
                return (int)obj;
            else if (type == EnumVarType.VAR_TYPE_uint)
                return (int)(uint)obj;
            else if (type == EnumVarType.VAR_TYPE_long)
                return (int)(uint)(long)obj;
            else if (type == EnumVarType.VAR_TYPE_ulong)
                return (int)(uint)(ulong)obj;
            else if (type == EnumVarType.VAR_TYPE_double)
                return (int)(uint)(double)obj;
            else if (type == EnumVarType.VAR_TYPE_float)
                return (int)(uint)(float)obj;
            else if (type == EnumVarType.VAR_TYPE_string)
                return ConvertTool.ToInt32((string)obj);
            else if (type == EnumVarType.VAR_TYPE_bool)
                return (bool)obj ? 1 : 0;

            return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
        }

        char GetValueChar(object obj)
        {
            return (char)GetValueDouble(obj);
        }

        sbyte GetValueSbyte(object obj)
        {
            return (sbyte)GetValueDouble(obj);
        }

        byte GetValueByte(object obj)
        {
            return (byte)GetValueDouble(obj);
        }

        short GetValueShort(object obj)
        {
            return (short)GetValueDouble(obj);
        }

        ushort GetValueUshort(object obj)
        {
            return (ushort)GetValueDouble(obj);
        }

        uint GetValueUint(object obj)
        {
            return (uint)GetValueDouble(obj);
        }

        long GetValueLong(object obj)
        {
            return (long)GetValueDouble(obj);
        }

        ulong GetValueUlong(object obj)
        {
            return (ulong)GetValueDouble(obj);
        }

        float GetValueFloat(object obj)
        {
            return (float)GetValueDouble(obj);
        }

        public string GetValueString(object obj)
        {
            EnumVarType type = GetObjectVarType(obj);

            if (type == EnumVarType.VAR_TYPE_string)
                return (string)obj;
            else if (type == EnumVarType.VAR_TYPE_short)
                return ((short)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_ushort)
                return ((ushort)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_int)
                return ((int)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_uint)
                return ((uint)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_long)
                return ((long)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_ulong)
                return ((ulong)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_double)
                return ((double)obj).ToString();
            else if (type == EnumVarType.VAR_TYPE_float)
                return ((float)obj).ToString();
            else
                return obj.ToString();	// 실제 이부분만 있어도 된다.
        }

        public double GetReturnValue()
        {
            return GetValueDouble(returnValue);
        }

        public object GetReturnValueObject()
        {
            return returnValue;
        }

        public Color GetReturnValueColor()
        {
            EnumVarType type = GetObjectVarType(returnValue);
            if (type == EnumVarType.VAR_TYPE_int)
                return NetFunction.FromArgb((int)returnValue);
            else if (type == EnumVarType.VAR_TYPE_uint)
                return NetFunction.FromArgb((int)((uint)returnValue));
            else if (type == EnumVarType.VAR_TYPE_long)
                return NetFunction.FromArgb((int)((long)returnValue));
            else if (type == EnumVarType.VAR_TYPE_ulong)
                return NetFunction.FromArgb((int)((ulong)returnValue));
            else if (type == EnumVarType.VAR_TYPE_double)
                return NetFunction.FromArgb((int)((double)returnValue));
            else if (type == EnumVarType.VAR_TYPE_float)
                return NetFunction.FromArgb((int)((float)returnValue));
            else
                return Colors.Black;
        }

        /*
        /// <summary>
        /// CONTROL 폴더에 있는 *.CTL 파일을 메모리로 불러들인다. 프로그램 시작할 때 한번만 불러준다.
        /// 이것은 9.0 이전의 버전을 불러올 때 사용한다.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// 

        int LoadFromMOD(string filename)
        {
            TextReader reader;
            string buf = "";
            CommaBlockString comma = new CommaBlockString();

            sDescription = "";
            //sFileName = filename;

            if (!File.Exists(filename)) return 0;

            reader = new StreamReader(filename, System.Text.Encoding, E System.Text.Encoding.Default);
            if (reader == null) return 0;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);

                if (buf == "Description")
                {
                    comma.GetString(ref sDescription);
                }
                else if (buf == "ScanTime")
                {
                    comma.GetInt(ref nScanTime);
                }
                else if (buf == "Data")
                {
                    string type = "";
                    string name = "";
                    ushort size = 0;

                    comma.GetString(ref type);
                    comma.GetString(ref name);
                    comma.GetWORD(ref size);
                    if (size < 1 || size >= 4096)
                    {
                        size = 1;
                    }
                    comma.GetStringTotalRemain(ref buf);
                    AddOneVarFromMod(type, name, size, buf);
                }
                else if (buf == "Programm")
                {
                    string prog;
                    int line = 0;

                    prog = "";

                    while (true)
                    {
                        buf = reader.ReadLine();
                        if (buf == null) break;

                        if (line > 0) prog += "\n";

                        prog += buf;

                        line++;
                    }

                    SetBuf(prog);
                }
                else
                {

                }
            }
            reader.Close();

            return 1;
        }

        
        public int LoadFromFile(string filename)
        {
            //string ext = System.IO.Path.GetExtension(filename);

            //if (String.Compare(ext, ".ctlx", StringComparison.CurrentCultureIgnoreCase) != 0)
            //{
//                return LoadFromMOD(filename);
  //          }
    //        else
      //      {
                TextReader reader;
                if (!File.Exists(filename)) return 0;
                reader = new StreamReader(filename);
                if (reader == null) return 0;
                LoadFromMODX(reader, "LoadFromModXFile");
                reader.Close();
                return 1;
        //    }
        }*/

        public string LoadProgramFromMODX(TextReader reader, string command)
        {
            string program = "";
            string one_line = "";
            string buf = "";
            CommaBlockString comma = new CommaBlockString();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref buf);

                if (buf == command)
                {
                    break;
                }
                else
                {
                    if (program.Length == 0)
                        program += one_line;
                    else
                    {
                        program += "\n" + one_line;
                    }
                }
            }

            return program;
        }

        public int LoadFromMODX(TextReader reader, string command)
        {
            string one_line = "";
            string buf = "";
            CommaTextReader comma = new CommaTextReader();

            sDescription = "";

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref buf);

                if (buf == "Description")
                {
                    comma.GetString(ref sDescription);
                }
                else if (buf == command) // END
                {
                    break;
                }
                else if (buf == "ScanTime")
                {
                    comma.GetInt(ref nScanTime);
                }
                else if (buf == "Data")
                {
                    string type = "";
                    string name = "";
                    ushort size = 0;

                    comma.GetString(ref type);
                    comma.GetString(ref name);
                    comma.GetWORD(ref size);
                    if (size < 1 || size >= 4096)
                    {
                        size = 1;
                    }
                    comma.GetString(ref buf);   //comma.GetStringTotalRemain(ref buf); //CommaTextReader를 사용하므로 GetString으로 해야한다.
                    AddOneVarFromModX(type, name, size, buf);
                }
                else if (buf == "Programm")
                {
                    SetBuf(LoadProgramFromMODX(reader, buf));
                }
                else
                {

                }
            }

            return 1;
        }

        int AddOneVar(EnumVarType type, string name, int size, string svalue)
        {
            VAR_STRUCT var;
            int l;
            CommaBlockString comma = new CommaBlockString();
            int i;

            comma.Set(svalue);

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == name) return 0;	// same name exist
            }

            var = new VAR_STRUCT();

            if (type == EnumVarType.VAR_TYPE_unknown) return 0;

            var.type = (EnumVarType)type;
            var.name = name;
            var.size = size;

            if (var.type == EnumVarType.VAR_TYPE_char)
            {
                ushort one = ' ';
                var.val = new char[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetWORD(ref one);
                    ((char[])var.val)[i] = (char)one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_sbyte)
            {
                sbyte one = 0;
                var.val = new sbyte[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetChar(ref one);
                    ((sbyte[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_byte)
            {
                byte one = 0;
                var.val = new byte[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetBYTE(ref one);
                    ((byte[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_short)
            {
                short one = 0;
                var.val = new short[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetInt(ref one);
                    ((short[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_ushort)
            {
                ushort one = 0;
                var.val = new ushort[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetWORD(ref one);
                    ((ushort[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_int)
            {
                int one = 0;
                var.val = new int[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetInt(ref one);
                    ((int[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_uint)
            {
                uint one = 0;
                var.val = new uint[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetDWORD(ref one);
                    ((uint[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_long)
            {
                long one = 0;
                var.val = new long[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetLong(ref one);
                    ((long[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_ulong)
            {
                ulong one = 0;
                var.val = new ulong[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetDWORD(ref one);
                    ((ulong[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_float)
            {
                float one = 0;
                var.val = new float[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetFloat(ref one);
                    ((float[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_double)
            {
                double one = 0;
                var.val = new double[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetDouble(ref one);
                    ((double[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_string)
            {
                string one = "";
                var.val = new string[var.size];
                for (i = 0; i < var.size; i++)
                {
                    comma.GetString(ref one);
                    ((string[])var.val)[i] = one;
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_object)
            {
                var.val = new object[var.size];
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("없는 변수 형식 {0} at AddOneVar", type));
                }
                else
                {
                    ErrorMessage(String.Format("Unknown Var type {0} at AddOneVar", type));
                }
                return 0;
            }

            arrayVar.Add(var);

            return 1;
        }

        int AddOneVarFromMod(string type, string name, int size, string svalue)
        {
            EnumVarType nType;

            nType = GetVarTypeFromMod(type, size);

            if (nType == EnumVarType.VAR_TYPE_unknown) return 0;

            return AddOneVar(nType, name, size, svalue);
        }

        public int AddOneVarFromModX(string type, string name, int size, string svalue)
        {
            EnumVarType nType;

            nType = GetVarTypeFromModX(type);

            if (nType == EnumVarType.VAR_TYPE_unknown) return 0;

            return AddOneVar(nType, name, size, svalue);
        }

        public void SetBuf(string buf)
        {
            sProgramm = buf.Trim();
        }

        // parameter를 argument로 사용하지 말자. 변환 과정 중 오류가 생길 수 있다.
        public void ErrorMessage(string s)
        {
            bErrorFlag = true;

            sErrorMessage = "Error: " + s;

            //EventSave.Error(this.sFileName + ":" + s, EnumEventID.ErrorScript, EnumEventCategory.SCRIPT);
        }

        // 이전의 에러메시지에 추가해 준다.
        public void AddErrorMessage(string s)
        {
            sErrorMessage += s;
        }

        public string GetError()
        {
            return sErrorMessage;
        }

        public EnumVarType GetVarTypeFromMod(string buf, int size)
        {
            if (buf == "char")
            {
                if (size == 1)	// char는 ushor이므로 부호가 없어진다.
                    return EnumVarType.VAR_TYPE_sbyte;
                else			// 사이즈가 여러개일때는 문자열으로 사용하므로 char로 변환
                    return EnumVarType.VAR_TYPE_char;
            }

            else if (buf == "sbyte") return EnumVarType.VAR_TYPE_sbyte;
            else if (buf == "byte") return EnumVarType.VAR_TYPE_byte;
            else if (buf == "short") return EnumVarType.VAR_TYPE_short;
            else if (buf == "ushort") return EnumVarType.VAR_TYPE_ushort;
            else if (buf == "int") return EnumVarType.VAR_TYPE_int;
            else if (buf == "uint") return EnumVarType.VAR_TYPE_uint;
            else if (buf == "long") return EnumVarType.VAR_TYPE_long;
            else if (buf == "ulong") return EnumVarType.VAR_TYPE_ulong;
            else if (buf == "float") return EnumVarType.VAR_TYPE_float;
            else if (buf == "double") return EnumVarType.VAR_TYPE_double;
            else if (buf == "string") return EnumVarType.VAR_TYPE_string;

            else if (buf == "BYTE") return EnumVarType.VAR_TYPE_char;
            else if (buf == "WORD") return EnumVarType.VAR_TYPE_ushort;
            else if (buf == "DWORD") return EnumVarType.VAR_TYPE_uint;

            else return EnumVarType.VAR_TYPE_unknown;
        }

        public EnumVarType GetVarTypeFromModX(string buf)
        {
            if (buf == "char") return EnumVarType.VAR_TYPE_char;
            else if (buf == "sbyte") return EnumVarType.VAR_TYPE_sbyte;
            else if (buf == "byte") return EnumVarType.VAR_TYPE_byte;
            else if (buf == "short") return EnumVarType.VAR_TYPE_short;
            else if (buf == "ushort") return EnumVarType.VAR_TYPE_ushort;
            else if (buf == "int") return EnumVarType.VAR_TYPE_int;
            else if (buf == "uint") return EnumVarType.VAR_TYPE_uint;
            else if (buf == "long") return EnumVarType.VAR_TYPE_long;
            else if (buf == "ulong") return EnumVarType.VAR_TYPE_ulong;
            else if (buf == "float") return EnumVarType.VAR_TYPE_float;
            else if (buf == "double") return EnumVarType.VAR_TYPE_double;
            else if (buf == "string") return EnumVarType.VAR_TYPE_string;
            else if (buf == "object") return EnumVarType.VAR_TYPE_object;
            else return EnumVarType.VAR_TYPE_unknown;
        }

        public string VarTypeToString(EnumVarType type)
        {
            string str;
            if (type == EnumVarType.VAR_TYPE_char) str = "char";
            else if (type == EnumVarType.VAR_TYPE_sbyte) str = "sbyte";
            else if (type == EnumVarType.VAR_TYPE_byte) str = "byte";
            else if (type == EnumVarType.VAR_TYPE_short) str = "short";
            else if (type == EnumVarType.VAR_TYPE_ushort) str = "ushort";
            else if (type == EnumVarType.VAR_TYPE_int) str = "int";
            else if (type == EnumVarType.VAR_TYPE_uint) str = "uint";
            else if (type == EnumVarType.VAR_TYPE_long) str = "long";
            else if (type == EnumVarType.VAR_TYPE_ulong) str = "ulong";
            else if (type == EnumVarType.VAR_TYPE_float) str = "float";
            else if (type == EnumVarType.VAR_TYPE_double) str = "double";
            else if (type == EnumVarType.VAR_TYPE_string) str = "string";
            else if (type == EnumVarType.VAR_TYPE_object) str = "object";
            else str = "none";

            return str;
        }

        //------------------------------------------------------------------------------
        //	처음의 블럭의 위치를 알아낸다.
        //------------------------------------------------------------------------------

        bool GetFirstBlockSize(string p, int total_size, out int size)
        {
            int CountBig = 0;	// {}의 +-
            int CountSmall = 0;	// ()의 +-
            bool comment = false;
            int string_count = 0;		// 한줄에서 모여진 글자수
            bool string_open = false;
            bool slash = false;

            size = 0;

            int w;

            for (w = 0; w < total_size; w++)
            {
                if (p[w] == '\r' || p[w] == '\n')
                {
                    if (CountBig == 0)
                    {
                        if (comment == false && string_count != 0)
                        {
                            string trim_p = p.Trim();	// else 문장 종류는 앞에 \n이 있다.

                            // if문이나 for문은 ;없이 다음 줄까지 연장이 가능하다.
                            if (String.Compare(trim_p, 0, "if", 0, 2) == 0)
                            {
                            }
                            else if (String.Compare(trim_p, 0, "for", 0, 3) == 0)
                            {
                            }
                            else if (String.Compare(trim_p, 0, "elseif", 0, 6) == 0)
                            {
                            }
                            else if (String.Compare(trim_p, 0, "else", 0, 4) == 0)
                            {
                            }
                            else
                            {
                                if (Tools.IsLangKorean())
                                    ErrorMessage(String.Format("줄의 끝에 ; 이 없습니다.\n{0}", p.Substring(0, w)));
                                else
                                    ErrorMessage(String.Format("; not found at line end.\n{0}", p.Substring(0, w)));

                                return false;
                            }
                        }
                    }

                    if (CountSmall != 0)
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("줄에서 () 의 개수가 맞지 않습니다.\n{0}", p.Substring(0, w)));
                        else
                            ErrorMessage(String.Format("'(' and ')' count mismatched.\n{0}", p.Substring(0, w)));

                        return false;
                    }

                    if (string_open)
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("줄에서 닫힘 \" 이 없습니다.\n{0}", p.Substring(0, w)));
                        else
                            ErrorMessage(String.Format("\" not fount at line.\n{0}", p.Substring(0, w)));

                        return false;
                    }

                    string_count = 0;
                    comment = false;
                    CountSmall = 0;
                    slash = false;
                    continue;
                }

                if (comment)
                {
                    string_count++;
                    continue;
                }

                if ((p[w] == ' ' || p[w] == '\t') && string_count == 0)
                {
                    continue;
                }

                if (p[w] == '"')
                {
                    if (!string_open) string_open = true;
                    else
                    {
                        if (!slash)
                            string_open = false;
                    }
                }

                if (string_open) // 스트링속의 문자열이다.
                {
                    string_count++;
                    continue;
                }

                if (p[w] == ';')
                {
                    if (CountBig == 0 && CountSmall == 0) goto out_loop;
                }
                else if (p[w] == '\\')
                {
                    if (!string_open)
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("slash (\\) 는 문자열 \"\" 안에서만 사용할 수 있습니다.\n{0}", p.Substring(0, w)));
                        else
                            ErrorMessage(String.Format("slash (\\) character is only use into \"\".\n{0}", p.Substring(0, w)));

                        return false;
                    }
                    if (slash)
                    {
                        slash = false;
                    }
                    else
                    {
                        slash = true;
                    }
                }
                else if (p[w] == '/')
                {
                    if (w != 0 && p[w - 1] == '/')
                    {
                        comment = true;

                    }
                }
                else if (p[w] == '}')
                {
                    CountBig--;
                    if (CountBig == 0 && CountSmall == 0) goto out_loop;
                }
                else if (p[w] == '{')
                {
                    CountBig++;
                }
                else if (p[w] == '(')
                {
                    CountSmall++;
                }
                else if (p[w] == ')')
                {
                    CountSmall--;
                }
                else
                {
                    slash = false;
                }

                string_count++;
            }

            if (CountBig != 0)
            {
                if (Tools.IsLangKorean())
                    ErrorMessage(String.Format("{{와 }}의 개수가 맞지 않습니다.\n{0}", p.Substring(0, w)));
                else
                    ErrorMessage(String.Format("'{{' and '}}' length is mismatched.\n{0}", p.Substring(0, w)));
            }

            size = total_size;
            return true;

        out_loop:
            size = w + 1;
            return true;
        }

        

        /// <summary>
        /// 시작부분에 빈공간이 있으면 공간을 삭제하고 // 와 같은 설명 문장도 함께 삭제한다.
        /// 실제 명령어가 시작되는 부분까지 찾는다. 한번에 연속해서 여러줄도 삭제된다.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <param name="block_size"></param>
        /// <returns></returns>

        bool KillFirstLineCommentAndSpace(string p, int start, int size, out int block_size)
        {
            block_size = 0;

            bool comment_flag = false;
            bool slash_flag = false;

            for (int i = 0; i < size; i++)
            {
                if (comment_flag)	// 설명문이면 개행문자까지 계속한다.
                {
                    if (p[start + i] == '\r' || p[start + i] == '\n')	// 개행시작
                    {
                        comment_flag = false;
                        slash_flag = false;
                    }


                }
                else
                {
                    if (p[start + i] == '/')
                    {
                        if (slash_flag)
                        {
                            comment_flag = true;
                            slash_flag = false;
                        }
                        else
                        {
                            slash_flag = true;
                        }
                    }
                    else if (p[start + i] == ' ' || p[start + i] == '\t' ||
                        p[start + i] == '\r' || p[start + i] == '\n')
                    {
                        if (slash_flag)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        block_size = i;
                        if (block_size > 0) return true;
                        else return false;
                    }
                }
            }

            block_size = size;	// 전체가 설명문이나 공간입니다.
            return true;
        }

        //------------------------------------------------------------------------------
        //	주어진 블럭을 해석한다. 이것은 recursive function
        //	하나의 블럭은 ; 로 끝나거나 { } 로 끝나는 경우이다.
        // return value
        // -1 error 발생
        // 0  return command return
        // 1  o.k
        //------------------------------------------------------------------------------

        int RecurseFunction(string org, int pre_retn)
        {
            int block_size = 0;
            int pos = 0;
            string p;
            int buf_size = org.Length;

            // 좌우 빈칸을 줄인다.
            p = org.Trim();
            buf_size = p.Length;

            // 설명문은 모조리 잘라낸다. 
            // 이 부분은 for나 if사용시 
            //		if() //설명문 
            //		{ 
            //			...문장...	
            //		} 
            //		
            //		식으로 사용하면 설명문 뒤는 해석을 못하는 문제점 때문에 추가되었다.
            while (true)
            {
            next:
                ;
                if (buf_size == 0) return 1;		// 내용이 없으므로 해석할 필요가 없다.

                if (buf_size >= 2 && p[0] == '/' && p[1] == '/') // 설명문
                {
                    for (int i = 2; i < buf_size; i++)
                    {
                        if (p[i] == '\r' || p[i] == '\n') // 개행
                        {
                            p = p.Substring(i + 1);
                            p = p.Trim();

                            buf_size = p.Length;
                            goto next;
                        }
                    }
                    // 개행이 되지 않으면 그 문장은 무시한다.
                    return 1;	// 설명문이다.
                }
                else
                {
                    break;
                }
            }

            // { } 로 쌓여져 있으면 없앤다.
            if (p[0] == '{' && p[buf_size - 1] == '}')
            {
                p = p.Substring(1, buf_size - 2);
                buf_size = p.Length;
            }

            int retn = pre_retn;

            while (true)
            {
                if (!KillFirstLineCommentAndSpace(p, pos, buf_size - pos, out block_size))
                {
                    if (!GetFirstBlockSize(p.Substring(pos), buf_size - pos, out block_size))
                        return -1;

                    retn = ExecuteOneBlock(p.Substring(pos, block_size), block_size, retn);
                    if (retn == -1) return -1;
                    if (retn == 0) return 0;		// return command
                }

                pos += block_size;
                if (pos >= buf_size) return 1;		// 모든 블럭을 해석했다.
            }
        }

        void KillLeftRightSpace(string buf, int total_size, out int start, out int end)
        {
            int i;

            start = 0;
            end = total_size - 1;

            for (i = 0; i < total_size; i++)
            {
                if (buf[i] == 32 || buf[i] == '\t' || buf[i] == '\n')
                {
                    continue;
                }
                start = i;
                break;
            }

            for (i = total_size - 1; i >= 0; i--)
            {
                if (buf[i] == 32 || buf[i] == '\t' || buf[i] == '\n' || buf[i] == ';')
                {
                    continue;
                }
                end = i;
                break;
            }

            if (start > end)
            {
                start = 0;
                end = total_size - 1;
            }
        }

        //------------------------------------------------------------------------------
        //	블럭을 해석 한다.
        //	1) if문
        // 2) 함수.
        // 3) for문.
        // 4) 선언문.
        // 5) 대입문.
        // argument 에서 pre_retn은 이전의 블럭이 if이고 그블럭을 만족하지 못했을 때.
        //	다음이 else일때 실행하기 위해서 존재한다.
        //------------------------------------------------------------------------------

        int ExecuteOneBlock(string p, int total_size, int pre_retn)
        {
            int w = 0;
            string command;
            int retn;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
        
            }

            if (!GetCommand(p, total_size, ref w, out command)) return -1;

            if (command.Length == 0) return 1;	// 빈칸이나 설명문

            retn = CheckReturn(p.Substring(w), total_size - w, command);
            if (retn != -2) return retn;	// 0 - return, -1 = error, -2는 다음문장을 검색하라는 뜻

            retn = CheckIF(p.Substring(w, total_size - w), command, pre_retn);      // if문일 때.
            if (retn != -2) return retn;	// -2 는 다음문장을 검색하라는 뜻

            retn = CheckFOR(p.Substring(w), total_size - w, command, pre_retn);
            if (retn != -2) return retn;

            retn = CheckFunction(p.Substring(w), total_size - w, command);	 		// 함수일때.
            if (retn != -2) return retn;

            // 대입문의 검사는 맨뒤에 한다. 대입문이 아닐때는 오류발생.
            retn = CheckMoveCommand(p, total_size, ref w, command);			        // 대입문.
            if (retn == 1) return 1;

            return retn;
        }

        bool GetCommand(string p, int total_size, ref int w, out string command)
        {
            command = "";

            while (true)
            {
                if (p[w] == '\n' || p[w] == 32 || p[w] == '\t')
                {
                    if (command.Length > 0)
                        return true;
                }
                else if (p[w] == '(' || p[w] == ';' || p[w] == '=' || p[w] == '{')
                {
                    return true;
                }
                else
                {
                    command += p[w];
                }

                w++;

                if (command.Length == 2 && command[0] == '/' && command[1] == '/')
                {
                    SeekNextLine(p, total_size, ref w);
                    nTraceLine++;
                    command = "";
                }

                if (w >= total_size) break;
            }

            if (command.Length == 0) return true;

            if (command == "return") return true;

            if (Tools.IsLangKorean())
            {
                ErrorMessage(String.Format("모르는 명령어 {0}", command));
            }
            else
            {
                ErrorMessage(String.Format("Unknown command {0}", command));
            }
            return true;

        }

        //------------------------------------------------------------------------------
        //	return문을 검사한다.
        // return = -2	: return 문이 아니므로 다음문장을 해석한다.
        //        = -1 : error 발생
        //        =  0 : return 문이므로 프로그램을 빠져나간다.
        //------------------------------------------------------------------------------

        int CheckReturn(string p, int total_size, string command)
        {
            if (String.Compare(command, 0, "return", 0, 6) != 0) return -2;

            int start, end;

            KillLeftRightSpace(p, total_size, out start, out end);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (end == start && p[start] == ';') return 0;
            }
            else
            {
                if (end == start && p[start] == ';') return 1;	// 편집모드에서는 의미없는 문장으로 한다.
            }

            if (!GetValueRecurse(p.Substring(start), end - start + 1, out returnValue)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                return 0;	// return
            else
                return 1;	// 편집모드에서는 의미없는 문장으로 한다.
        }

        //------------------------------------------------------------------------------
        //	다음 줄을 찾는다.
        //------------------------------------------------------------------------------

        int SeekNextLine(string p, int total_size, ref int w)
        {
            while (true)
            {
                if (w >= total_size) return 0;	 // 파일의 끝이다.
                if (p[w] == '\n')
                {
                    w++;
                    return 1;
                }
                w++;
            }
        }

        public bool GetValueRecurse(string org_s, out object val)
        {
            return GetValueRecurse(org_s, org_s.Length, out val);
        }

        public bool GetValueRecurse(string org_s, out float val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueFloat(obj);
            return flag;
        }

        public bool GetValueRecurse(string org_s, out double val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueDouble(obj);
            return flag;
        }

        public bool GetValueRecurse(string org_s, out int val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueInt(obj);
            return flag;
        }

        public bool GetValueRecurse(string org_s, out sbyte val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueSbyte(obj);
            return flag;
        }

        public bool GetValueRecurse(string org_s, out ushort val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueUshort(obj);
            return flag;
        }

        public bool GetValueRecurse(string org_s, int org_size, out double val)
        {
            object obj = "";
            bool flag = GetValueRecurse(org_s, org_size, out obj);
            if (!flag)
            {
                val = 0;
                return false;
            }
            val = GetValueDouble(obj);
            return flag;
        }

        // 뒤에서 부터 해석하는 것이 키포인트
        public bool GetValueRecurse(string org_s, int org_size, out object val)
        {
            // 먼저 앞뒤의 Space를 없애준다.
            int start, end;
            KillLeftRightSpace(org_s, org_size, out start, out end);
            int size = end - start + 1;
            string s = org_s.Substring(start, size);

            if (size <= 0)
            {
                val = 0.0;
                return true;
            }

            int open_close1 = 0, open_close2 = 0;
            int pos = size - 1;
            object value1, value2;

            //--------------------
            // 명령어 @cell(field)
            //--------------------

            val = 0;

            while (true)
            {
                if (s[pos] == '(')
                {
                    open_close1++;
                }
                else if (s[pos] == ')')
                {
                    open_close1--;
                }
                else if (s[pos] == '[')
                {
                    open_close2++;
                }
                else if (s[pos] == ']')
                {
                    open_close2--;
                }
                else if (s[pos] == '+')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            return false;
                        }
                        else
                        {
                            if (!GetValueRecurse(s, pos, out value1)) return false;
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;
                            val = GetValueDouble(value1) + GetValueDouble(value2);
                            return true;
                        }
                    }
                }
                else if (s[pos] == '-')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos == 0)
                        {
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value1)) return false;
                            val = 0 - GetValueDouble(value1);
                            return true;
                        }
                        else if (pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(s, pos, out value1)) return false;
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;

                            val = GetValueDouble(value1) - GetValueDouble(value2);
                            return true;
                        }
                    }
                }
                else if (s[pos] == '%')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos == 0)
                        {
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value1)) return false;
                            val = 0 - GetValueDouble(value1);
                            return true;
                        }
                        else if (pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(s, pos, out value1)) return false;
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;

                            if (GetValueDouble(value2) == 0)
                                val = 0;
                            else
                                val = (uint)(GetValueDouble(value1)) % (uint)(GetValueDouble(value2));
                            return true;
                        }
                    }
                }
                else if (s[pos] == '|' || s[pos] == '&' || s[pos] == '^')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos == 0)
                        {
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value1)) return false;
                            val = value1;
                            return true;
                        }
                        else if (pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(s, pos, out value1)) return false;
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;

                            if (s[pos] == '|')
                                val = GetValueUint(value1) | GetValueUint(value2);
                            else if (s[pos] == '&')
                                val = GetValueUint(value1) & GetValueUint(value2);
                            else
                                val = GetValueUint(value1) ^ GetValueUint(value2);

                            return true;
                        }
                    }
                }
                else { };

                pos--;

                if (pos < 0)
                {
                    break;
                }
            }

            open_close1 = 0;
            open_close2 = 0;
            pos = size - 1;

            while (true)
            {
                if (s[pos] == '(')
                {
                    open_close1++;
                }
                else if (s[pos] == ')')
                {
                    open_close1--;
                }
                else if (s[pos] == '[')
                {
                    open_close2++;
                }
                else if (s[pos] == ']')
                {
                    open_close2--;
                }
                else if (s[pos] == '*')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(s, pos, out value1)) return false;
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;
                            val = GetValueDouble(value1) * GetValueDouble(value2);
                            return true;
                        }
                    }
                }
                else if (s[pos] == '/')
                {
                    if (open_close1 == 0 && open_close2 == 0)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            val = 0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(s.Substring(pos + 1), size - pos - 1, out value2)) return false;

                            if (GetValueDouble(value2) == 0)
                            {	// protected divice by 0
                                val = 0.0;
                                return true;
                            }
                            else
                            {
                                if (!GetValueRecurse(s, pos, out value1)) return false;
                                val = GetValueDouble(value1) / GetValueDouble(value2);
                                return true;
                            }
                        }
                    }
                }
                else { }

                pos--;

                if (pos < 0)
                {
                    if (s[0] == '(' && s[size - 1] == ')')
                    {
                        return GetValueRecurse(s.Substring(1), size - 2, out val);
                    }
                    else
                    {
                        return GetValueElseFunction(s, size, out val);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------
        //	하나의 요소만 남았을 때 숫자이거나 함수일 때.
        //	(예) @cell(ex), 12345, 12.3, @average 등 등
        //------------------------------------------------------------------------------

        bool GetValueElseFunction(string s, int size, out object val)
        {
            if (s[0] == '@')
            {	// 함수.

                int start_pos = 0, end_pos = 0;
                if (!SeekStartEnd(s, size, ref start_pos, ref end_pos))
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage("함수가 ( ) 로 닫혀있지 않음");
                    }
                    else
                    {
                        ErrorMessage("() count mismathed of function");
                    }
                    val = 0;
                    return false;
                }

                int name_size;

                name_size = start_pos - 1;
                if (name_size < 1)
                {
                    val = 0;
                    return false;
                }
                if (name_size > 900)
                {
                    val = 0;
                    return false;
                }

                int arg_size = end_pos - start_pos - 1;	// ()를 뺀 argument size
                string func_name;
                string func_arg;

                func_name = s.Substring(1, name_size);
                func_arg = s.Substring(start_pos + 1, arg_size);

                int retn;

                retn = ScriptFunctionCheck.Function_Check(this, func_name, func_arg, out val, true);
                if (retn == -1) return false;
                if (retn == 1) return true;

                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("모르는 함수{0}", func_name));
                }
                else
                {
                    ErrorMessage(String.Format("Unknown function {0}", func_name));
                }

                return false;
            }
            else if (size >= 2 && s[0] == '"' && s[size - 1] == '"') // 확실한 문자열
            {
                val = s.Substring(1, size - 2);
                return true;
            }
            else
            {
                if (IsNumberString(s, size, out val))
                {
                    return true;
                }
                else
                {
                    int name_size;
                    int var_pos;
                    bool var_array_flag;
                    if (!GetVarPos(s, size, out name_size, out var_pos, out var_array_flag)) return false;
                    if (!GetVarValue(s, name_size, out val, var_pos, var_array_flag)) return false;
                    return true;
                }
            }
        }

        bool IsNumberString(string buf, int size, out object val)
        {
            int i;

            // 0x 로 시작하는 16진수인가를 검사한다.
            if (size >= 3)
            {
                if (buf[0] == '0')
                {
                    if (buf[1] == 'x' || buf[1] == 'X')
                    {
                        double val_imsi = 0;
                        for (i = 2; i < size; i++)
                        {
                            if (buf[i] >= '0' && buf[i] <= '9')
                            {
                                val_imsi *= 16;
                                val_imsi += buf[i] - '0';
                                continue;
                            }
                            if (buf[i] >= 'a' && buf[i] <= 'f')
                            {
                                val_imsi *= 16;
                                val_imsi += buf[i] - 'a' + 10;
                                continue;
                            }
                            if (buf[i] >= 'A' && buf[i] <= 'F')
                            {
                                val_imsi *= 16;
                                val_imsi += buf[i] - 'A' + 10;
                                continue;
                            }
                            val = 0;
                            return false;
                        }
                        val = val_imsi;
                        return true;
                    }
                }
            }

            // 소수점을 포함하는 십진수인가를 검사한다.
            for (i = 0; i < size; i++)
            {
                if (buf[i] >= '0' && buf[i] <= '9') continue;
                if (buf[i] == '.') continue;
                if (buf[i] == 32) continue;

                val = 0;
                return false;
            }

            string stack;

            stack = buf.Substring(0, size);

            val = ConvertTool.ToDouble(stack);

            return true;
        }

        //------------------------------------------------------------------------------
        //	주어진 스트링이 변수라고 가정하고 배열의 이름과 위치를 얻어온다.
        //------------------------------------------------------------------------------

        bool GetVarPos(string p, int total_size, out int name_size, out int pos, out bool var_array_flag)
        {
            int open = 0, close = 0;
            int start_pos = 0, end_pos = 0;
            int w;
            var_array_flag = false;

            for (w = 0; w < total_size; w++)
            {
                if (p[w] == '[')
                {
                    open++;
                    if (open == 1) start_pos = w;
                }
                else if (p[w] == ']')
                {
                    close++;
                    if (open == close) end_pos = w;
                    break;
                }
                else { }
            }

            if (open != close)
            {
                string buf;

                buf = p.Substring(0, start_pos);
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("{0}의 [] 개수가 맞지 않음", buf));
                }
                else
                {
                    ErrorMessage(String.Format("mismatched count of [] ({0})", buf));
                }

                name_size = 0;
                pos = 0;
                return false;
            }

            if (open == 0)
            {
                pos = 0;
                name_size = total_size;
                return true;
            }

            var_array_flag = true;

            int size = end_pos - start_pos - 1;
            object val;
            if (!GetValueRecurse(p.Substring(start_pos + 1), size, out val))
            {
                name_size = 0;
                pos = 0;
                return false;
            }

            pos = (int)GetValueDouble(val);
            name_size = start_pos;

            return true;
        }

        public object GetVarValue(VAR_STRUCT var, int pos, bool var_array_flag)
        {
            object val;

            if (var.type == EnumVarType.VAR_TYPE_char)
            {
                if (var_array_flag)
                    val = ((char[])var.val)[pos];
                else
                {
                    if (var.size == 1)
                        val = ((char[])var.val)[pos];
                    else // char[] 배열은 문자열로 바꿔준다.
                    {
                        string s = "";
                        char[] vp = (char[])var.val;

                        for (int i = 0; i < vp.Length; i++)
                        {
                            if (vp[i] == 0) break;
                            s += vp[i];
                        }
                        val = s;
                    }
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_sbyte) val = ((sbyte[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_byte) val = ((byte[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_short) val = ((short[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_ushort) val = ((ushort[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_int) val = ((int[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_uint) val = ((uint[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_long) val = ((long[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_ulong) val = ((ulong[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_float) val = ((float[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_double) val = ((double[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_string) val = ((string[])var.val)[pos];
            else if (var.type == EnumVarType.VAR_TYPE_object)
            {
                val = ((object[])var.val)[pos];
            }
            else
            {
                val = 0;
            }
            return val;
        }

        public bool GetVarValue(string buf, int total_size, out object val, int pos, bool var_array_flag)
        {
            VAR_STRUCT var;
            int l;
            int start, end;
            int size;

            if (total_size == 0)
            {
                ErrorMessage("buf size 0");
                val = 0;
                return false;
            }

            KillLeftRightSpace(buf, total_size, out start, out end);

            size = end - start + 1;

            if (IsDefinedValue(buf, start, size, out val)) return true;

            string stack;
            stack = buf.Substring(start, size);

            if (stack[0] == '$')
            {	// 태그 변수
                return GetValueByTagName(stack.Substring(1), out val);
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == stack)
                {
                    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    {
                        if (pos >= var.size)
                        {
                            if (Tools.IsLangKorean())
                            {
                                ErrorMessage(String.Format("GetValue에서 배열범위 초과\n{0}[{1}]", stack, pos));
                            }
                            else
                            {
                                ErrorMessage(String.Format("array pos over at GetValue\n{0}[{1}]", stack, pos));
                            }
                            return false;
                        }
                    }
                    else
                    {
                        pos = 0;
                    }


                    val = GetVarValue(var, pos, var_array_flag);

                    return true;
                }
            }

            if (Tools.IsLangKorean())
            {
                ErrorMessage(String.Format("없는 변수 사용 {0}", stack));
            }
            else
            {
                ErrorMessage(String.Format("Undefined varible used {0}", stack));
            }

            return false;
        }

        //------------------------------------------------------------------------------
        //	미리 정의된 define은 이것을 사용한다.
        //------------------------------------------------------------------------------

        //[NonSerialized]
        static string[] pre_define = { "ON", "OFF", "TRUE", "FALSE", "MB_OK", "MB_YESNO", "MB_YESNOCANCEL", "IDYES", "IDNO", "IDCANCEL", "IDOK" };
        //[NonSerialized]
        static double[] pre_value = { 1, 0, 1, 0, (double)MessageBoxButton.OK, (double)MessageBoxButton.OKCancel, (double)MessageBoxButton.OKCancel, (double)MessageBoxResult.OK, (double)MessageBoxResult.No, (double)MessageBoxResult.Cancel, (double)MessageBoxResult.OK };

        // MessageBoxResult.YEs는 실버라이트에서 사용하지 않는다. 2009.7.10

        bool IsDefinedValue(string buf, int start_index, int total_size, out object val)
        {
            if (String.Compare("null", 0, buf, start_index, 4) == 0)
            {
                val = null;
                return true;
            }

            int i;

            for (i = 0; i < pre_define.Length; i++)
            {
                if (pre_define[i].Length == total_size)
                {
                    if (String.Compare(pre_define[i], 0, buf, start_index, total_size) == 0)
                    {
                        val = pre_value[i];
                        return true;
                    }
                }
            }

            val = 0;
            return false;
        }

        //------------------------------------------------------------------------------
        //	IF 조건문을 검사한다.
        //------------------------------------------------------------------------------

        int CheckIF(string p, string command, int pre_retn)
        {

            int retn;

            // else일 때는 앞의 문장이 if이고 그 조건을 만족하지 못했을 때만 유효한 문장이다.
            if (command == "else")
            {
                if (pre_retn == 2)
                {	// 앞의 if문이 조건문을 만족 했으므로 else는 return 한다.
                    return 2;
                }
                else if (pre_retn != 3)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage("else 의 위치가 잘못되어 있습니다.");
                    }
                    else
                    {
                        ErrorMessage("else position wrong.");
                    }

                    return -1;
                }
                else { }

                retn = RecurseFunction(p, pre_retn);
                if (retn == -1) return -1;

                return retn;
            }

            if (command == "elseif")
            {
                if (pre_retn == 2)
                {	// 앞의 if문이 조건문을 만족 했으므로 elseif 는 계속 return 한다.
                    return 2;
                }
                else if (pre_retn != 3)
                {
                    ErrorMessage("else if position is wrong.");
                    return -1;
                }
                else { }

                goto if_loop;
            }

            if (command != "if") return -2;		// if 문이 아니다.

            if_loop:

            int start_pos = 0, end_pos = 0;
            int compare;

            if (!SeekStartEnd(p, p.Length, ref start_pos, ref end_pos))
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("{0} 다음에 ()가 없습니다", command));
                }
                else
                {
                    ErrorMessage("() not found after [if]");
                }
                return -1;
            }


            compare = Compare(p.Substring(start_pos + 1), end_pos - start_pos - 1);	// 문장을 비교한다.
            if (compare == -1) return -1;	// 문장 오류.

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (compare != 0)
                {	// 조건문을 만족한다.
                    retn = RecurseFunction(p.Substring(end_pos + 1, p.Length - (end_pos + 1)), pre_retn);
                    if (retn == -1) return -1;
                    if (retn == 0) return 0;	// return 문이 포함 됨
                    return 2;
                }
                else
                {
                    return 3; 	// 조건문울 만족하지 않고 다음 블럭으로 넘어간다.
                }
            }	// edit mode 일때는 compile 해야하므로 조건문과 상관없이 그줄을 행한다.
            else
            {
                // if {} 블럭을 해석한다.
                retn = RecurseFunction(p.Substring(end_pos + 1, p.Length - (end_pos + 1)), pre_retn);
                if (retn == -1) return -1;

                return 3; 	// 조건문울 만족하지 않고 다음 블럭으로 넘어간다.
            }
        }

        //------------------------------------------------------------------------------
        //	FOR loop를 검사한다.
        //------------------------------------------------------------------------------

        int CheckFOR(string p, int total_size, string command, int pre_retn)
        {
            if (command != "for") return -2;		// for 문이 아니다.

            int lgal = 0, rgal = 0;
            int semi1, semi2;

            int compare;
            int arg_size;

            int block1_size;
            int block2_size;
            int block3_size;
            int retn;



            if (!SeekStartEnd(p, total_size, ref lgal, ref rgal))
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage("for 다음에 ()가 없습니다");
                }
                else
                {
                    ErrorMessage("() not found after [for]");
                }
                return -1;
            }

            arg_size = rgal - lgal - 1;

            // 초기화 문 위치를 찾는다.
            if (!SeekSemiColon(p.Substring(lgal + 1), arg_size, out semi1)) return -1;

            block1_size = semi1 + 1;

            // 두번째 비교문 위치를 찾는다.
            if (!SeekSemiColon(p.Substring(lgal + 1 + block1_size), arg_size - block1_size, out semi2)) return -1;

            block2_size = semi2 + 1;
            block3_size = arg_size - block1_size - block2_size;

            // 초기화를 처리한다.
            // 1이면 ; 밖에 없는 경우 이 때는 초기화가 필요 없다.
            if (block1_size > 1)
            {
                if (RecurseFunction(p.Substring(lgal + 1, block1_size), pre_retn) == -1) return -1;
            }

            DateTime t;
            int old_sec;

            int sec_curr = 0;

            t = DateTime.Now;
            old_sec = t.Second;

            while (true)
            {
                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {
                    t = DateTime.Now;
                    if (t.Second != old_sec)
                    {
                        old_sec = t.Second;
                        sec_curr++;
                        if (sec_curr >= ConfigViewMain.nForLoopTimeout)
                        {
                            if (Tools.IsLangKorean())
                            {
                                ErrorMessage(String.Format("for loop가 {0}초가 흐르도록 끝나지 않음. 프로그램 수정 필요", ConfigViewMain.nForLoopTimeout));
                            }
                            else
                            {
                                ErrorMessage(String.Format("for loop {0} sec time out", ConfigViewMain.nForLoopTimeout));
                            }
                            return -1;
                        }
                    }
                }

                if (block2_size > 1)
                { 	// 1이면 ; 밖에 없는 경우 이때는 초기화가 필요없다.
                    compare = Compare(p.Substring(lgal + 1 + block1_size), block2_size);	// 문장을 비교한다.
                    if (compare == -1) return -1;	// 문장 오류.

                    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    {		// run mode 에서만 조건문에서 break 한다.
                        if (compare == 0) break;
                    }

                    // 조건문을 만족하면 블럭을 해석한다.
                    retn = RecurseFunction(p.Substring(rgal + 1, total_size - (rgal + 1)), pre_retn);
                    if (retn == -1) return -1;
                    if (retn == 0) return 0;	// for loop 속에 return문
                }

                if (block3_size > 0)
                { 	// 0 이면 next 문장이 없다.
                    if (RecurseFunction(p.Substring(lgal + 1 + block1_size + block2_size, block3_size), pre_retn) == -1) return -1;
                }
                if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
                {	// edit mode 에서는 초기화, 조건, next 와 블럭을 해석했으므로 break 한다. 
                    break;
                }
            }

            return 1;
        }

        bool GetRemainStringAtMoveCommand(string p, int total_size, ref int w, out string str)
        {
            str = "";

            for (; w < total_size; w++)
            {
                if (str.Length == 0 && (p[w] == 32 || p[w] == '\t'))
                {

                }
                else if (p[w] == ';')
                {
                    if (str.Length == 0)
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("= 다음에 대입할 문장이 없습니다.\n{0}", p.Substring(0, total_size)));
                        else
                            ErrorMessage(String.Format("Sentence not found after equal(=).\n{0}", p.Substring(0, total_size)));

                        return false;
                    }
                    return true;
                }
                else if (p[w] == '\r' || p[w] == '\n')
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("문장에 ; 이 빠져 있습니다.\n{0}", p.Substring(0, total_size)));
                    else
                        ErrorMessage(String.Format("Semicolon (;) not found at move command.\n{0}", p.Substring(0, total_size)));

                    return false;
                }
                else
                {
                    str += p[w];
                }
            }

            if (str.Length == 0)
            {
                if (Tools.IsLangKorean())
                    ErrorMessage(String.Format("= 다음에 대입할 문장이 없습니다.\n{0}", p.Substring(0, total_size)));
                else
                    ErrorMessage(String.Format("Sentence not found after equal(=).\n{0}", p.Substring(0, total_size)));

                return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------
        //	대입문 인가를 검사한다.
        //------------------------------------------------------------------------------

        int CheckMoveCommand(string p, int total_size, ref int w, string command)
        {
            int l;
            VAR_STRUCT var;

            int name_size;
            int var_pos;
            bool var_array_flag;

            if (!GetVarPos(command, command.Length, out name_size, out var_pos, out var_array_flag)) return -1;

            string name;

            name = command.Substring(0, name_size);

            if (!SeekEqual(p, total_size, ref w))
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("변수 다음에 = 이 없습니다.({0})", name));
                }
                else
                {
                    ErrorMessage(String.Format("not found = after variable({0})", name));
                }

                return -1;		// 대입문이 아니다.
            }

            string value_string;

            if (!GetRemainStringAtMoveCommand(p, total_size, ref w, out value_string))
            {
                return -1;
            }

            if (name[0] == '$')
            {
                SaveTagPos stp = GetSaveTagPos(name.Substring(1));

                if (stp == null)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage(String.Format("존재하지 않는 태그(${0})", name));
                    }
                    else
                    {
                        ErrorMessage(String.Format("(${0}) tag not found", name));
                    }
                    return -1;		// 대입문이 아니다.
                }

                object val;

                if (!GetValueRecurse(value_string, value_string.Length, out val)) return -1;

                if (stp.member_var == EnumTagMemberVar.MEMBER_VAR_string && GetObjectVarType(val) != EnumVarType.VAR_TYPE_string)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("{0}은(는) 문자열 태그멤버이므로 문자열 값이 대입되어야 합니다.", name));
                    else
                        ErrorMessage(String.Format("String value needed because {0} is string Tag.", name));

                    return -1;
                }

                if (stp.member_var != EnumTagMemberVar.MEMBER_VAR_string && GetObjectVarType(val) == EnumVarType.VAR_TYPE_string)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("{0}은(는) 숫자형 태그멤버이므로 숫자형 값이 대입되어야 합니다.", name));
                    else
                        ErrorMessage(String.Format("numeric value needed because {0} is Numeric Tag.", name));

                    return -1;
                }

                if (stp.member_var == EnumTagMemberVar.MEMBER_VAR_string)
                    SetTagMemberString(stp.tag, stp.type, ref stp.pos, stp.member, GetValueString(val));
                else
                    SetTagMemberValue(stp.tag, stp.type, ref stp.pos, stp.member, GetValueDouble(val));

                return 1;
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == name)
                {	// 선언된 변수중에 있다.

                    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    {
                        if (var_pos >= var.size)
                        {
                            if (Tools.IsLangKorean())
                            {
                                ErrorMessage(String.Format("변수의 배열 크기를 벗어나는 대입문\n{0}", name));
                            }
                            else
                            {
                                ErrorMessage(String.Format("array index range over\n{0}", name));
                            }
                            return -1;		// 대입문이 맞다.
                        }
                    }
                    else
                    {
                        var_pos = 0;	// 편집 모드일때는 배열 위치를 알 수 없다.
                    }

                    object val;

                    if (!GetValueRecurse(value_string, value_string.Length, out val)) return -1;

                    if (var.type == EnumVarType.VAR_TYPE_string && GetObjectVarType(val) != EnumVarType.VAR_TYPE_string)
                    {
                        val = val.ToString();
                        /*
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("{0}은(는) 문자열 변수이므로 문자열 값이 대입되어야 합니다.", var.name));
                        else
                            ErrorMessage(String.Format("String value needed because {0} is string Tag.", name));

                        return -1;*/
                    }

                    if (var.type != EnumVarType.VAR_TYPE_string && GetObjectVarType(val) == EnumVarType.VAR_TYPE_string)
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("{0}은(는) 숫자형 변수이므로 숫자형 값이 대입되어야 합니다.", var.name));
                        else
                            ErrorMessage(String.Format("numeric value needed because {0} is Numeric Tag.", name));

                        return -1;
                    }

                    if (!SetVarValue(l, val, var_pos)) return -1;

                    return 1;
                }
            }

            if (Tools.IsLangKorean())
            {
                ErrorMessage(String.Format("선언되지 않은 변수 ({0})", name));
            }
            else
            {
                ErrorMessage(String.Format("Undefined varibale ({0})", name));
            }

            return -1;
        }

        //------------------------------------------------------------------------------
        //	( 가 시작하는 위치와 ) 가 닫히는 위치를 찾는다.
        //------------------------------------------------------------------------------

        bool SeekStartEnd(string s, int size, ref int start_pos, ref int end_pos)
        {
            int i;
            int open = 0;
            int close = 0;

            for (i = 0; i < size; i++)
            {
                if (s[i] == '(')
                {
                    if (open == 0)
                    {
                        start_pos = i;
                    }
                    open++;
                }
                else if (s[i] == ')')
                {
                    end_pos = i;
                    close++;

                    if (open < close)
                    {
                        ErrorMessage("can't find start (");
                        return false;	// 시작하는 ( 를 찾지 못했다.
                    }
                    else if (open == close)
                    {
                        return true;
                    }
                }
                else { }
            }

            ErrorMessage("can't find end )");
            return false;	// 끝나는 ')'를 찾지 못했다.
        }

        //------------------------------------------------------------------------------
        // 문장을 비교한다.
        // 들어올수 있는 문장은 ex
        // ex < 100   ,   100 > 10 등 for loop의 두번째 비교문이나. if의 비교문을 처리한다.
        // return -1	= 문장 오류
        // return 0    = 거짓.
        // return 1    = 참.
        //------------------------------------------------------------------------------

        int Compare(string p, int size)
        {
            if (size == 0)
            {
                ErrorMessage(String.Format("비교문에서 비교할 문장이 없습니다."));
                return -1;
            }

            if (p[size - 1] == ';') size--;

            int start_pos, end_pos;

            if (!KillStartEndSpace(p, size, out start_pos, out end_pos))
            {
                string msg;
                msg = p.Substring(0, size);

                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("비교문의 문장이 이상합니다.[{0}]", msg));
                }
                else
                {
                    ErrorMessage(String.Format("Invalid context in Compare[{0}]", msg));
                }

                return -1;
            }
            p = p.Substring(start_pos);
            size = end_pos - start_pos + 1;

            int open = 0, close = 0;
            int w;

            for (w = 0; w < size; w++)
            {
                if (p[w] == '(')
                {
                    open++;
                }
                else if (p[w] == ')')
                {
                    close++;
                }
                else
                {
                    if (open == close)
                    {
                        if (p[w] == '&' && p[w + 1] == '&')
                        {
                            int compare_1 = Compare(p, w);
                            if (compare_1 == -1) return -1;
                            int compare_2 = Compare(p.Substring(w + 2), size - (w + 2));
                            if (compare_2 == -1) return -1;

                            if (compare_1 != 0 && compare_2 != 0) return 1;
                            else return 0;
                        }
                        else if (p[w] == '|' && p[w + 1] == '|')
                        {
                            int compare_1 = Compare(p, w);
                            if (compare_1 == -1) return -1;
                            int compare_2 = Compare(p.Substring(w + 2), size - (w + 2));
                            if (compare_2 == -1) return -1;
                            if (compare_1 != 0 || compare_2 != 0) return 1;
                            else return 0;
                        }
                        else { }
                    }
                }
            }

            open = 0;
            close = 0;

            int left_pos = 0, right_pos = 0;
            int compare = -1;

            for (w = 0; w < size; w++)
            {
                if (p[w] == '(')
                {
                    open++;
                }
                else if (p[w] == ')')
                {
                    close++;
                }
                else
                {
                    if (open == close)
                    {
                        if (p[w] == '=' && p[w + 1] == '=')
                        {
                            compare = 0;
                            left_pos = w - 1;
                            right_pos = w + 2;
                        }
                        else if (p[w] == '<' && p[w + 1] == '=')
                        {
                            compare = 3;
                            left_pos = w - 1;
                            right_pos = w + 2;
                        }
                        else if (p[w] == '>' && p[w + 1] == '=')
                        {
                            compare = 4;
                            left_pos = w - 1;
                            right_pos = w + 2;
                        }
                        else if (p[w] == '!' && p[w + 1] == '=')
                        {
                            compare = 5;
                            left_pos = w - 1;
                            right_pos = w + 2;
                        }
                        else if (p[w] == '<')
                        {
                            compare = 1;
                            left_pos = w - 1;
                            right_pos = w + 1;
                        }
                        else if (p[w] == '>')
                        {
                            compare = 2;
                            left_pos = w - 1;
                            right_pos = w + 1;
                        }
                        else { }

                        if (compare != -1) break;
                    }
                }
            }

            if (compare == -1)
            {
                if (p[0] == '(' && p[size - 1] == ')')
                {
                    return Compare(p.Substring(1), size - 2);
                }

                string msg;
                msg = p.Substring(0, size);
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("비교문이 없습니다. [{0}]", msg));
                }
                else
                {
                    ErrorMessage(String.Format("compare command not found.[{0}]", msg));
                }
                return -1;
            }

            object value1, value2;

            if (!GetValueRecurse(p, left_pos + 1, out value1)) return -1;
            if (!GetValueRecurse(p.Substring(right_pos), size - right_pos, out value2)) return -1;

            int retn = 0;

            EnumVarType t1 = GetObjectVarType(value1);
            EnumVarType t2 = GetObjectVarType(value2);

            if (t1 == EnumVarType.VAR_TYPE_string && t2 == EnumVarType.VAR_TYPE_string)
            {
                if (compare == 0)
                {
                    string v1 = GetValueString(value1);
                    string v2 = GetValueString(value2);
                    if (v1 == v2) return 1;
                    else return 0;
                }
                else if (compare == 5)
                {
                    string v1 = GetValueString(value1);
                    string v2 = GetValueString(value2);
                    if (v1 != v2) return 1;
                    else return 0;
                }
                else
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("문자열 비교는 == 과 != 만 사용할 수 있습니다.\n{0}", p.Substring(0, size)));
                    else
                        ErrorMessage(String.Format("String compare can contain only == or !=\n{0}", p.Substring(0, size)));

                    return -1;
                }
            }
            else if (t1 != EnumVarType.VAR_TYPE_string && t2 != EnumVarType.VAR_TYPE_string)
            {
                double v1 = GetValueDouble(value1);
                double v2 = GetValueDouble(value2);

                switch (compare)
                {
                    case 0: if (v1 == v2) retn = 1; break;
                    case 1: if (v1 < v2) retn = 1; break;
                    case 2: if (v1 > v2) retn = 1; break;
                    case 3: if (v1 <= v2) retn = 1; break;
                    case 4: if (v1 >= v2) retn = 1; break;
                    case 5: if (v1 != v2) retn = 1; break;
                }

                return retn;
            }
            else
            {
                if (Tools.IsLangKorean())
                    ErrorMessage(String.Format("문자열과 숫자는 서로 비교할 수 없습니다.\n{0}", p.Substring(0, size)));
                else
                    ErrorMessage(String.Format("Cannot compare String and Number.\n{0}", p.Substring(0, size)));

                return -1;
            }
        }

        //------------------------------------------------------------------------------
        //	문장이 ; 으로 끝나면 1
        //------------------------------------------------------------------------------

        bool SeekSemiColon(string p, int total_size, out int seek_pos)
        {
            int w;

            for (w = 0; w < total_size; w++)
            {
                if (p[w] == ';')
                {
                    seek_pos = w;
                    return true;
                }
            }

            if (Tools.IsLangKorean())
            {
                ErrorMessage(";을 찾을 수 없습니다.");
            }
            else
            {
                ErrorMessage("Can't seek semicolon(;)");
            }

            seek_pos = 0;
            return false;
        }

        bool SeekEqual(string p, int total_size, ref int w)
        {
            for (; w < total_size; w++)
            {
                if (p[w] == '=')
                {
                    w++;
                    return true;
                }
            }

            return false;
        }

        public bool SetVarValue(int l, object val, int pos)
        {
            if (l >= arrayVar.Count)
            {
                ErrorMessage("ChangeValue block error");
                return false;
            }

            VAR_STRUCT var;

            var = (VAR_STRUCT)arrayVar[l];

            if (pos >= var.size)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage("배열범위 초과");
                }
                else
                {
                    ErrorMessage("array pos over");
                }

                return false;
            }

            if (var.type == EnumVarType.VAR_TYPE_char) ((char[])var.val)[pos] = GetValueChar(val);
            else if (var.type == EnumVarType.VAR_TYPE_sbyte) ((sbyte[])var.val)[pos] = GetValueSbyte(val);
            else if (var.type == EnumVarType.VAR_TYPE_byte) ((byte[])var.val)[pos] = GetValueByte(val);
            else if (var.type == EnumVarType.VAR_TYPE_short) ((short[])var.val)[pos] = GetValueShort(val);
            else if (var.type == EnumVarType.VAR_TYPE_ushort) ((ushort[])var.val)[pos] = GetValueUshort(val);
            else if (var.type == EnumVarType.VAR_TYPE_int) ((int[])var.val)[pos] = GetValueInt(val);
            else if (var.type == EnumVarType.VAR_TYPE_uint) ((uint[])var.val)[pos] = GetValueUint(val);
            else if (var.type == EnumVarType.VAR_TYPE_long) ((long[])var.val)[pos] = GetValueLong(val);
            else if (var.type == EnumVarType.VAR_TYPE_ulong) ((ulong[])var.val)[pos] = GetValueUlong(val);
            else if (var.type == EnumVarType.VAR_TYPE_float) ((float[])var.val)[pos] = GetValueFloat(val);
            else if (var.type == EnumVarType.VAR_TYPE_double) ((double[])var.val)[pos] = GetValueDouble(val);
            else if (var.type == EnumVarType.VAR_TYPE_string)
            {
                ((string[])var.val)[pos] = GetValueString(val);
            }
            else if (var.type == EnumVarType.VAR_TYPE_object)
            {
                ((object[])var.val)[pos] = val;
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("없는 변수 형식 ({0})", var.name));
                }
                else
                {
                    ErrorMessage(String.Format("Undefined variable ({0})", var.name));
                }

                return false;
            }
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
        
            }

            return true;
        }

        bool KillStartEndSpace(string p, int size, out int start_pos, out int end_pos)
        {
            int w;
            start_pos = 0;
            end_pos = 0;

            for (w = 0; w < size; w++)
            {
                if (p[w] == 32) continue;
                if (p[w] == '\t') continue;
                start_pos = w;
                goto seek_end;
            }
            return false;	// 마지막이 되도록 찾지 못했다.
        seek_end:
            for (w = size - 1; w > 0; w--)
            {
                if (p[w] == 32) continue;
                if (p[w] == '\t') continue;
                end_pos = w;
                goto seek_ok;
            }
            return false;	// 마지막이 되도록 찾지 못했다.
        seek_ok:
            if (start_pos > end_pos) return false;
            return true;
        }

        // 스크립트에서 나오는 $태그는 스크립트 실행시마다 태그위치를 계속 찾게 된다.
        // 이것을 찾지 않게 하기위해서 아래의 SaveTagPos 클래스에 담아서 사용한다.
        List<object> arraySaveTagPos = new List<object>();

        //[Serializable]
        class SaveTagPos
        {
            public string tag_full;
            public string tag;
            public EnumTagType type;
            public int[] pos = new int[1];
            public EnumTagMember member;
            public EnumTagMemberVar member_var;
            public TagPublicClass tp;
        }

        SaveTagPos GetSaveTagPos(string tag_full)
        {
            SaveTagPos stp;

            for (int i = 0; i < arraySaveTagPos.Count; i++)
            {
                stp = (SaveTagPos)arraySaveTagPos[i];
                if (tag_full == stp.tag_full) return stp;
            }

            stp = new SaveTagPos();
            stp.tag_full = tag_full;
            if (!TagLib.GetTagTypePosMember(stp.tag_full, out stp.tag, out stp.type, ref stp.pos, out stp.member, out stp.member_var, out stp.tp))
            {
                return null;
            }

            arraySaveTagPos.Add(stp);
            return stp;
        }

        bool GetValueByTagName(string tag_full, out object val)
        {
            SaveTagPos stp = GetSaveTagPos(tag_full);

            if (stp == null)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("존재하지 않는 태그 이름 (${0})", tag_full));
                }
                else
                {
                    ErrorMessage(String.Format("(${0}) tag not found", tag_full));
                }

                val = 0;

                return false;
            }

            val = GetTagMemberValue(stp.tag, stp.type, stp.tp, stp.member);

            return true;
        }

        public object GetTagMemberValue(string tag, EnumTagType type, TagPublicClass tp, EnumTagMember member)
        {
            switch (member)
            {
                case EnumTagMember.TAG_MEMBER_tag: return tp.tag;
                case EnumTagMember.TAG_MEMBER_name: return tp.name;
                case EnumTagMember.TAG_MEMBER_description: return tp.description;
                case EnumTagMember.TAG_MEMBER_act: return tp.act;
                case EnumTagMember.TAG_MEMBER_assign:
                    {
                        if (tp.assign == null)
                            return "";
                        else
                            return tp.assign.tag;
                    }
            }

            object val = 0;

            if (type == EnumTagType.AI)
            {
                TagAiClass ai = (TagAiClass)tp;
                ai.bNeedDataCurr = true;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr: val = ai.curr; break;
                    case EnumTagMember.TAG_MEMBER_port: val = ai.port; break;
                    case EnumTagMember.TAG_MEMBER_address: val = ai.address; break;
                    case EnumTagMember.TAG_MEMBER_hihi: val = ai.hihi; break;
                    case EnumTagMember.TAG_MEMBER_high: val = ai.high; break;
                    case EnumTagMember.TAG_MEMBER_low: val = ai.low; break;
                    case EnumTagMember.TAG_MEMBER_lolo: val = ai.lolo; break;
                    case EnumTagMember.TAG_MEMBER_full: val = ai.fFull; break;
                    case EnumTagMember.TAG_MEMBER_base: val = ai.fBase; break;
                    case EnumTagMember.TAG_MEMBER_plc_full: val = ai.fPlcFull; break;
                    case EnumTagMember.TAG_MEMBER_plc_base: val = ai.fPlcBase; break;
                    case EnumTagMember.TAG_MEMBER_viewfull: val = ai.view_full; break;
                    case EnumTagMember.TAG_MEMBER_viewbase: val = ai.view_base; break;
                    case EnumTagMember.TAG_MEMBER_NeedAlarmConfirm: val = ai.bNeedAlarmConfirm; break;
                    case EnumTagMember.TAG_MEMBER_ProtectScan:
                        val = (ai.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectControl:
                        val = (ai.wProtectFlags & EnumProtectFlag.CONTROL) == EnumProtectFlag.CONTROL ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmEvent:
                        val = (ai.wProtectFlags & EnumProtectFlag.ALARM_EVENT) == EnumProtectFlag.ALARM_EVENT ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmData:
                        val = (ai.wProtectFlags & EnumProtectFlag.ALARM_DATA) == EnumProtectFlag.ALARM_DATA ? 1 : 0;
                        break;

                    case EnumTagMember.TAG_MEMBER_fSumTotal: val = ai.fSumTotal; break;
                    case EnumTagMember.TAG_MEMBER_fSumPart: val = ai.fSumPart; break;
                    case EnumTagMember.TAG_MEMBER_cAlarmLevelStatus: val = (int)ai.cAlarmLevelStatus; break;
                    case EnumTagMember.TAG_MEMBER_fDisplayFormat: val = ai.fDisplayFormat; break;
                    case EnumTagMember.TAG_MEMBER_unit: val = ai.unit; break;
                    case EnumTagMember.TAG_MEMBER_alarm: val = ai.alarm; break;
                }
            }
            else if (type == EnumTagType.AO)
            {
                TagAoClass ao = (TagAoClass)tp;
                ao.bNeedDataCurr = true;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr: val = ao.curr; break;
                    case EnumTagMember.TAG_MEMBER_port: val = ao.port; break;
                    case EnumTagMember.TAG_MEMBER_station: val = ao.station; break;
                    case EnumTagMember.TAG_MEMBER_address: val = ao.address; break;
                    case EnumTagMember.TAG_MEMBER_extra2: val = ao.wExtraAddr; break;
                    case EnumTagMember.TAG_MEMBER_extra1: val = ao.sExtraAddr; break;

                    case EnumTagMember.TAG_MEMBER_full: val = ao.fFull; break;
                    case EnumTagMember.TAG_MEMBER_base: val = ao.fBase; break;
                    case EnumTagMember.TAG_MEMBER_plc_full: val = ao.plc_full; break;
                    case EnumTagMember.TAG_MEMBER_plc_base: val = ao.plc_base; break;
                }
            }
            else if (type == EnumTagType.DI)
            {
                TagDiClass di = (TagDiClass)tp;
                di.bNeedDataCurr = true;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr: val = di.curr; break;
                    case EnumTagMember.TAG_MEMBER_port: val = di.port; break;
                    case EnumTagMember.TAG_MEMBER_address: val = di.address_word * 16 + di.address_bit; break;
                    case EnumTagMember.TAG_MEMBER_NeedAlarmConfirm: val = di.bNeedAlarmConfirm; break;
                    case EnumTagMember.TAG_MEMBER_ProtectScan:
                        val = (di.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectControl:
                        val = (di.wProtectFlags & EnumProtectFlag.CONTROL) == EnumProtectFlag.CONTROL ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmEvent:
                        val = (di.wProtectFlags & EnumProtectFlag.ALARM_EVENT) == EnumProtectFlag.ALARM_EVENT ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmData:
                        val = (di.wProtectFlags & EnumProtectFlag.ALARM_DATA) == EnumProtectFlag.ALARM_DATA ? 1 : 0;
                        break;
                    case EnumTagMember.TAG_MEMBER_desON:
                        val = di.desON;
                        break;
                    case EnumTagMember.TAG_MEMBER_desOFF:
                        val = di.desOFF;
                        break;
                    case EnumTagMember.TAG_MEMBER_alarm: val = di.alarm; break;
                }
            }
            else if (type == EnumTagType.DO)
            {
                TagDoClass dout = (TagDoClass)tp;
                dout.bNeedDataCurr = true;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr: val = dout.curr; break;
                    case EnumTagMember.TAG_MEMBER_port: val = dout.port; break;
                    case EnumTagMember.TAG_MEMBER_station: val = dout.station; break;
                    case EnumTagMember.TAG_MEMBER_address: val = dout.address; break;
                    case EnumTagMember.TAG_MEMBER_extra2: val = dout.wExtraAddr; break;
                    case EnumTagMember.TAG_MEMBER_desON:
                        val = dout.desON;
                        break;
                    case EnumTagMember.TAG_MEMBER_desOFF:
                        val = dout.desOFF;
                        break;
                    case EnumTagMember.TAG_MEMBER_extra1:
                        val = dout.sExtraAddr;
                        break;
                }
            }
            else if (type == EnumTagType.ST)
            {
                TagStClass st = (TagStClass)tp;
                st.bNeedDataCurr = true;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        val = st.curr;
                        break;
                    case EnumTagMember.TAG_MEMBER_port: val = st.port; break;
                    case EnumTagMember.TAG_MEMBER_address: val = st.address; break;
                }
            }

            return val;
        }

        public bool HaveRightsHandOperationAndMsgAtScript(string tag, string description)
        {
            return true;
            /*
            if (SharedData.userInfo.HaveRightsHandOperation(tag)) return true;

            string msg;

            if (Tools.IsLangKorean())
            {
                msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}\n설명={1}", tag, description);
            }
            else if (Tools.IsLangChinese())
            {
                msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}\n描述={1}", tag, description);
            }
            else
            {
                msg = String.Format("You have not the right to control this tag.\nTag={0}\nDes={1}", tag, description);
            }

            MessageBox.Show(msg);
            //MessageDisplay.Show(msg);

            return false;*/
        }

        public void SetTagMemberValue(string tag, EnumTagType type, ref int[] tag_pos, EnumTagMember member, double val)
        {
            TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

            switch (member)
            {
                case EnumTagMember.TAG_MEMBER_act: tp.act = (sbyte)val; return;
            }

            if (tp.enumTagType == 0)
            {
                TagAiClass ai = (TagAiClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            if (bHandOperation && !HaveRightsHandOperationAndMsgAtScript(ai.tag, ai.description))
                                break;

                            TagWrite.WriteCurrAI(tag, ai, val, bHandOperation);
                        }
                        break;
                    case EnumTagMember.TAG_MEMBER_port:
                        ai.port = (short)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_address:
                        ai.address = (ushort)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;

                    case EnumTagMember.TAG_MEMBER_hihi:
                        ai.hihi = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_high: ai.high = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_low: ai.low = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_lolo: ai.lolo = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_full: ai.fFull = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_base: ai.fBase = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_plc_full: ai.fPlcFull = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_plc_base: ai.fPlcBase = (float)val;
                        ai.bTagChangeFlag = true;	// Tag의 속성이 바뀌었으므로 계산을 다시 해야 한다.
                        break;
                    case EnumTagMember.TAG_MEMBER_viewfull: ai.view_full = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_viewbase: ai.view_base = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_NeedAlarmConfirm: ai.bNeedAlarmConfirm = (val == 1); break;
                    case EnumTagMember.TAG_MEMBER_ProtectScan:
                        SetProtectFlag(ref ai.wProtectFlags, EnumProtectFlag.SCAN, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectControl:
                        SetProtectFlag(ref ai.wProtectFlags, EnumProtectFlag.CONTROL, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmEvent:
                        SetProtectFlag(ref ai.wProtectFlags, EnumProtectFlag.ALARM_EVENT, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmData:
                        SetProtectFlag(ref ai.wProtectFlags, EnumProtectFlag.ALARM_DATA, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_fSumTotal: ai.fSumTotal = val; break;
                    case EnumTagMember.TAG_MEMBER_fSumPart: ai.fSumPart = val; break;
                    case EnumTagMember.TAG_MEMBER_fDisplayFormat: ai.fDisplayFormat = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_alarm: ai.alarm = (byte)val; break;
                }
            }
            else if (type == EnumTagType.AO)
            {
                TagAoClass ao = (TagAoClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            if (bHandOperation && !HaveRightsHandOperationAndMsgAtScript(ao.tag, ao.description))
                                break;
                            TagWrite.WriteCurrAO(tag, ao, val, bHandOperation);
                        }
                        break;
                    case EnumTagMember.TAG_MEMBER_port: ao.port = (short)val; break;
                    case EnumTagMember.TAG_MEMBER_station: ao.station = (short)val; break;
                    case EnumTagMember.TAG_MEMBER_address: ao.address = (ushort)val; break;
                    case EnumTagMember.TAG_MEMBER_extra2: ao.wExtraAddr = (ushort)val; break;

                    case EnumTagMember.TAG_MEMBER_full: ao.fFull = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_base: ao.fBase = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_plc_full: ao.plc_full = (float)val; break;
                    case EnumTagMember.TAG_MEMBER_plc_base: ao.plc_base = (float)val; break;
                }
            }
            else if (type == EnumTagType.DI)
            {
                TagDiClass di = (TagDiClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            if (bHandOperation && !HaveRightsHandOperationAndMsgAtScript(di.tag, di.description))
                                break;
                            TagWrite.WriteCurrDI(tag, di, (sbyte)val, bHandOperation);
                        }
                        break;
                    case EnumTagMember.TAG_MEMBER_port:
                        di.port = (short)val;
                        break;
                    case EnumTagMember.TAG_MEMBER_address:
                        di.address_word = (uint)val / 16;
                        di.address_bit = (sbyte)(val % 16);
                        break;

                    case EnumTagMember.TAG_MEMBER_NeedAlarmConfirm:
                        di.bNeedAlarmConfirm = (val == 1); break;
                    case EnumTagMember.TAG_MEMBER_ProtectScan:
                        SetProtectFlag(ref di.wProtectFlags, EnumProtectFlag.SCAN, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectControl:
                        SetProtectFlag(ref di.wProtectFlags, EnumProtectFlag.CONTROL, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmEvent:
                        SetProtectFlag(ref di.wProtectFlags, EnumProtectFlag.ALARM_EVENT, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_ProtectAlarmData:
                        SetProtectFlag(ref di.wProtectFlags, EnumProtectFlag.ALARM_DATA, (sbyte)val);
                        break;
                    case EnumTagMember.TAG_MEMBER_alarm: di.alarm = (sbyte)val; break;
                }
            }
            else if (type == EnumTagType.DO)
            {
                TagDoClass dout = (TagDoClass)tp;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            if (bHandOperation && !HaveRightsHandOperationAndMsgAtScript(dout.tag, dout.description))
                                break;
                            TagWrite.WriteCurrDO(tag, dout, (sbyte)val, bHandOperation);
                        }
                        break;
                    case EnumTagMember.TAG_MEMBER_port: dout.port = (short)val; break;
                    case EnumTagMember.TAG_MEMBER_station: dout.station = (short)val; break;
                    case EnumTagMember.TAG_MEMBER_address: dout.address = (ushort)val; break;
                    case EnumTagMember.TAG_MEMBER_extra2: dout.wExtraAddr = (ushort)val; break;
                }
            }
            else if (type == EnumTagType.ST)
            {
                TagStClass st = (TagStClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_port:
                        st.port = (short)val;
                        break;
                    case EnumTagMember.TAG_MEMBER_address: st.address = (ushort)val;
                        break;
                }
            }
        }

        void SetProtectFlag(ref EnumProtectFlag flags, EnumProtectFlag mask, sbyte val)
        {
            if (val == 1) flags &= ~mask;
            else flags |= mask;
        }

        //------------------------------------------------------------------------------
        //	()가 빠진 argument에서 스트링을 취한다. ""로 쌓여 있을것이다.
        //------------------------------------------------------------------------------

        public bool GetArgumentString(string p, out string buf)
        {
            int total_size = p.Length;
            buf = "";

            if (total_size == 0)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage("문자열은 \" \"로 구분하거나 string 태그 또는 변수일 것.\n문자열 길이가 0임");
                }
                else
                {
                    ErrorMessage("string must include start\" end\" or string TAG or Var\nString string=0");
                }

                return false;
            }

            if (p[0] == '"' && p[total_size - 1] == '"')
            {	// 문자열 임
                int i;
                bool flag_slash = false;

                for (i = 1; i <= total_size - 2; i++)
                {
                    if (flag_slash)
                    {
                        if (p[i] == 'n') buf += '\n';
                        else if (p[i] == 'r') buf += '\r';
                        else if (p[i] == 't') buf += '\t';
                        else if (p[i] == '\\') buf += '\\';
                        else if (p[i] == '"') buf += '"';
                        else buf += p[i];

                        flag_slash = false;
                        continue;
                    }
                    else if (p[i] == '\\')
                    {
                        flag_slash = true;
                    }
                    else
                    {
                        buf += p[i];
                    }
                }

                return true;
            }

            object imsi;
            bool retn = GetValueRecurse(p, out imsi);

            if (!retn) return false;

            if (imsi.GetType() != typeof(string))
            {
                string msg;

                if (Tools.IsLangKorean())
                {
                    msg = String.Format("주어진 값이 문자열이 아닙니다.\n{0}={1}({2})", p, imsi, imsi.GetType().ToString());
                }
                else
                {
                    msg = String.Format("Value is not a string value.\n{0}={1}({2})", p, imsi, imsi.GetType().ToString());
                }

                ErrorMessage(msg);

                return false;
            }

            buf = (string)imsi;

            return true;
            /*
            buf = "";
            int total_size = p.Length;

            if (total_size == 0)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage("문자열은 \" \"로 구분하거나 string 태그 또는 변수일 것.\n문자열 길이가 0임");
                }
                else
                {
                    ErrorMessage("string must include start\" end\" or string TAG or Var\nString string=0");
                }
                return false;
            }

            if (p[0] == '"' && p[total_size - 1] == '"')
            {	// 문자열 임
                int i;
                bool flag_slash = false;

                for (i = 1; i <= total_size - 2; i++)
                {
                    if (flag_slash)
                    {
                        if (p[i] == 'n') buf += '\n';
                        else if (p[i] == 'r') buf += '\r';
                        else if (p[i] == 't') buf += '\t';
                        else if (p[i] == '\\') buf += '\\';
                        else if (p[i] == '"') buf += '"';
                        else buf += p[i];

                        flag_slash = false;
                        continue;
                    }
                    else if (p[i] == '\\')
                    {
                        flag_slash = true;
                    }
                    else
                    {
                        buf += p[i];
                    }
                }

                return true;
            }

            int l;
            VAR_STRUCT var;

            if (p[0] == '$')
            {
                SaveTagPos stp = GetSaveTagPos(p.Substring(1));

                if (stp == null)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("[{0}] 태그를 찾을 수 없습니다.", p));
                    else
                        ErrorMessage(String.Format("Cannot find the Tag [{0}].", p));

                    return false;
                }

                if (stp.member_var != EnumTagMemberVar.MEMBER_VAR_string)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage(String.Format("[{0}] 는 string tag나 member가 아님.", p));
                    }
                    else
                    {
                        ErrorMessage(String.Format("[{0}] is not string tag or member.", p));
                    }
                    return false;
                }

                buf = GetValueString(GetTagMemberValue(stp.tag, stp.type, stp.tp, stp.member));

                return true;
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == p)
                {	// 선언된 변수중에 있다.
                    if (var.type == EnumVarType.VAR_TYPE_char)
                    {
                        if (var.size < 2)
                        {
                            if (Tools.IsLangKorean())
                                ErrorMessage(String.Format("문자열로 사용하려면 char 배열이 2개 이상이 되어야 합니다.\n[{0}]", p));
                            else
                                ErrorMessage(String.Format("It is cannot use string because char array length is 1.\n[{0}]", p));

                            return false;
                        }

                        buf = "";
                        char[] vp = (char[])var.val;

                        for (int i = 0; i < vp.Length; i++)
                        {
                            if (vp[i] == 0) break;
                            buf += vp[i];
                        }

                        return true;
                    }
                    else if (var.type == EnumVarType.VAR_TYPE_string)
                    {
                        buf = ((string[])var.val)[0];

                        return true;
                    }
                    else
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("스트링이 아닙니다.char나 string을 사용할 것\n[{0}]", p));
                        else
                            ErrorMessage(String.Format("It is not a string. Use char or string variable\n[{0}]", p));

                        return false;
                    }
                }
            }

            if (Tools.IsLangKorean())
            {
                ErrorMessage(String.Format("문자열은 \" \"로 구분하거나 string 태그 또는 변수일 것.\n[{0}]", p));
            }
            else
            {
                ErrorMessage(String.Format("string must include start\" end\" or string TAG, Var\n[{0}]", p));
            }
            return false;*/
        }

        int CheckFunction(string p, int total_size, string command)
        {
            if (command[0] != '@') return -2;

            int lgal = 0, rgal = 0;

            if (!SeekStartEnd(p, total_size, ref lgal, ref rgal))
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("{0} 함수 다음에 ()가 없습니다", command));
                }
                else
                {
                    ErrorMessage(String.Format("() not found after [{0} function]", command));
                }
                return -1;
            }

            int arg_size = rgal - lgal - 1;
            string arg;

            arg = p.Substring(lgal + 1, arg_size);

            CommaBlockString comma = new CommaBlockString();

            comma.Set(arg);

            object val;
            int retn;

            retn = ScriptFunctionCheck.Function_Check(this, command.Substring(1), arg, out val, false);
            if (retn != 0) return retn;	// if retn == 0 해당 함수가 없다.

            if (Tools.IsLangKorean())
            {
                ErrorMessage(String.Format("지원되지 않는 함수입니다.({0})", command));
            }
            else
            {
                ErrorMessage(String.Format("Undefined function ({0})", command));
            }
            return -1;
        }

        public object ExecuteClassName(List<object> arrayClassName, string classname, string command, params object[] args)
        {
            int i;
            ObjectExpand obj;
            string name;
            object retn = 0;
            bool this_flag = false;

            if (String.Compare(classname, 0, "this.", 0, 5) == 0)
            {
                this_flag = true;
            }

            for (i = 0; i < arrayClassName.Count; i++)
            {
                obj = (ObjectExpand)arrayClassName[i];
                obj.objGeneral.GetClassName(out name);

                if (this_flag && this.formParent == obj.objCommonProperty.rootPage &&
                    String.Compare(classname, 5, name, 0, name.Length) == 0)
                {
                    retn = obj.ExecuteClassName(bHandOperation, command, args);
                    continue;
                }

                if (name == classname)
                    retn = obj.ExecuteClassName(bHandOperation, command, args);
            }

            return retn;
        }

        public string ExecuteClassNameStringReturn(List<object> arrayClassName, string classname, string command, params object[] args)
        {
            int i;
            ObjectExpand obj;
            string name;
            string retn = "";
            bool this_flag = false;

            if (String.Compare(classname, 0, "this.", 0, 5) == 0)
            {
                this_flag = true;
            }

            for (i = 0; i < arrayClassName.Count; i++)
            {
                obj = (ObjectExpand)arrayClassName[i];
                obj.objGeneral.GetClassName(out name);

                if (this_flag && this.formParent == obj.objCommonProperty.rootPage &&
                    String.Compare(classname, 5, name, 0, name.Length) == 0)
                {
                    retn = obj.ExecuteClassNameStringReturn(command, args);
                    continue;
                }

                if (name == classname)
                    retn = obj.ExecuteClassNameStringReturn(command, args);
            }

            return retn;
        }

        public void SetTagMemberString(string tag, EnumTagType type, ref int[] tag_pos, EnumTagMember member, string source)
        {
            TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

            switch (member)
            {
                case EnumTagMember.TAG_MEMBER_description:
                    tp.description = source;
                    return;
                case EnumTagMember.TAG_MEMBER_assign:
                    {
                        EnumTagType assign_type = 0;

                        tp.assign = new ASSIGN_TAG_STRUCT();	// assign이 null이거나 pos가 태그없을 일 수 있으므로
                        tp.assign.tag = source;
                        TagLib.GetTagTypeAndPos(source, ref assign_type, ref tp.assign.pos);

                        if (assign_type != type) tp.assign.pos[0] = TagLib.TAG_NOT_FOUND;   // 같은 태그 type이 아니면 곤란.

                        return;
                    }
            }

            if (type == 0)
            {
                TagAiClass ai = (TagAiClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_unit:
                        ai.unit = source;
                        break;
                }
            }
            else if (type == EnumTagType.AO)
            {
                TagAoClass ao = (TagAoClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_extra1:
                        ao.sExtraAddr = source;
                        break;
                }
            }
            else if (type == EnumTagType.DI)
            {
                TagDiClass di = (TagDiClass)tp;

                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_desON:
                        di.desON = source;
                        break;
                    case EnumTagMember.TAG_MEMBER_desOFF:
                        di.desOFF = source;
                        break;
                }
            }
            else if (type == EnumTagType.DO)
            {
                TagDoClass dout = (TagDoClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_desON:
                        dout.desON = source;
                        break;
                    case EnumTagMember.TAG_MEMBER_desOFF:
                        dout.desOFF = source;
                        break;
                    case EnumTagMember.TAG_MEMBER_extra1:
                        dout.sExtraAddr = source;
                        break;
                }
            }
            else if (type == EnumTagType.ST)
            {
                TagStClass st = (TagStClass)tp;
                switch (member)
                {
                    case EnumTagMember.TAG_MEMBER_curr:
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            if (bHandOperation && !HaveRightsHandOperationAndMsgAtScript(st.tag, st.description))
                                break;

                            TagWrite.WriteCurrST(tag, st, source, bHandOperation);
                        }
                        break;
                }
            }
        }

        public bool IsStringVar(string target, string error_title)
        {
            int l;
            VAR_STRUCT var;

            if (target.Length == 0)
            {
                if (Tools.IsLangKorean())
                    ErrorMessage(String.Format("{0}:문자열을 사용해야 합니다.", error_title));
                else
                    ErrorMessage(String.Format("{0}:You must use the string var.", error_title));
                return false;
            }

            if (target[0] == '$')
            {
                SaveTagPos stp = GetSaveTagPos(target.Substring(1));

                if (stp == null)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("{1}:[{0}]태그를 찾을 수 없습니다.", target, error_title));
                    else
                        ErrorMessage(String.Format("{1}:Cannot find a [{0}] tag.", target, error_title));

                    return false;
                }

                if (stp.member_var != EnumTagMemberVar.MEMBER_VAR_string)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage(String.Format("{1}:[{0}]는 문자열 태그가 아니거나 String Member가 아닙니다.", target, error_title));
                    }
                    else
                    {
                        ErrorMessage(String.Format("{1}:[{0}] Tag is not string.", target, error_title));
                    }
                    return false;
                }
                return true;
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == target)
                {	// 선언된 변수중에 있다.
                    if (var.type == EnumVarType.VAR_TYPE_char)
                    {
                        if (var.size < 2)
                        {
                            if (Tools.IsLangKorean())
                                ErrorMessage(String.Format("{1}:char형을 문자열로 사용하려면 크기가 2 이상이 되어야 합니다.\n{0}", var.name, error_title));
                            else
                                ErrorMessage(String.Format("{1}:Cannot use as string because char array length is 1.\n{0}", var.name, error_title));

                            return false;
                        }
                        return true;
                    }
                    else if (var.type == EnumVarType.VAR_TYPE_string)
                    {
                        return true;
                    }
                    else
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("{0}:스트링이 아닙니다.char 배열이나 string을 사용할 것", error_title));
                        else
                            ErrorMessage(String.Format("{0}:It is not a string. Use char or string variable.", error_title));

                        return false;
                    }
                }
            }

            if (Tools.IsLangKorean())
                ErrorMessage(String.Format("{1}:선언되지 않는 문자열 변수입니다.\n변수명={0}", target, error_title));
            else
                ErrorMessage(String.Format("{1}:It is not defined string variable.\nVariable={0}", target, error_title));

            return false;
        }

        public bool ChangeStringVar(string target, string str)
        {
            int l;
            VAR_STRUCT var;

            if (target[0] == '$')
            {
                SaveTagPos stp = GetSaveTagPos(target.Substring(1));

                if (stp == null)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("[{0}]태그를 찾을 수 없습니다.", target));
                    else
                        ErrorMessage(String.Format("Cannot find a [{0}] tag.", target));

                    return false;
                }
                if (stp.member_var != EnumTagMemberVar.MEMBER_VAR_string)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage(String.Format("[{0}]는 문자열 태그가 아니거나 String Member가 아닙니다.", target));
                    }
                    else
                    {
                        ErrorMessage(String.Format("[{0}] is not string tag or member.", target));
                    }

                    return false;
                }

                SetTagMemberString(stp.tag, stp.type, ref stp.pos, stp.member, str);

                return true;
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == target)
                {	// 선언된 변수중에 있다.
                    if (var.type == EnumVarType.VAR_TYPE_char)
                    {
                        if (var.size < 2)
                        {
                            if (Tools.IsLangKorean())
                                ErrorMessage(String.Format("문자열로 사용하기 위해서는 char 크기가 2 이상이 되어야 합니다.\n[{0}]", target));
                            else
                                ErrorMessage(String.Format("Cannot use as string because char array length is 1.\n{0}", target));

                            return false;
                        }

                        int i;

                        for (i = 0; i < str.Length; i++)
                        {
                            if (i < var.size)
                                ((char[])var.val)[i] = str[i];
                        }
                        if (i < var.size)
                            ((char[])var.val)[i] = (char)0;

                        return true;
                    }
                    else if (var.type == EnumVarType.VAR_TYPE_string)
                    {
                        ((string[])var.val)[0] = str;
                        return true;
                    }

                    else
                    {
                        if (Tools.IsLangKorean())
                            ErrorMessage(String.Format("문자열이 아닙니다.char나 string을 사용할 것\nat ChangeStringVar var={0}", target));
                        else
                            ErrorMessage(String.Format("It is not a string. Use char or string variable\nat ChangeStringVar var={0}", target));

                        return false;
                    }
                }
            }

            if (Tools.IsLangKorean())
                ErrorMessage(String.Format("선언되지 않는 문자열 변수입니다.\n변수명={0}", target));
            else
                ErrorMessage(String.Format("It is not defined string variable.\nVariable={0}", target));

            return false;
        }



        public bool ChangeNumberVar(string target, double val, string error_title)
        {
            int l;
            VAR_STRUCT var;

            if (target.Length == 0)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("{0}: 변수명을 입력하지 않았습니다.", error_title));
                }
                else
                {
                    ErrorMessage(String.Format("{0}: You must input the variable.", error_title));
                }
                return false;
            }

            if (target[0] == '$')
            {
                SaveTagPos stp = GetSaveTagPos(target.Substring(1));

                if (stp == null)
                {
                    if (Tools.IsLangKorean())
                        ErrorMessage(String.Format("{1}:[{0}] 태그를 찾을 수 없습니다.", target, error_title));
                    else
                        ErrorMessage(String.Format("{1}:Cannot find a [{0}] Tag.", target, error_title));

                    return false;
                }

                if (stp.member_var == EnumTagMemberVar.MEMBER_VAR_string)
                {
                    if (Tools.IsLangKorean())
                    {
                        ErrorMessage(String.Format("{1}:[{0}]는 문자열 태그이거나 멤버입니다.", target, error_title));
                    }
                    else
                    {
                        ErrorMessage(String.Format("{1}:[{0}] is a string tag or member.", target, error_title));
                    }
                    return false;
                }

                SetTagMemberValue(stp.tag, stp.type, ref stp.pos, stp.member, val);

                return true;
            }

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                if (var.name == target)
                {	// 선언된 변수중에 있다.
                    if (!SetVarValue(l, val, 0)) return false;
                    return true;
                }
            }

            if (Tools.IsLangKorean())
                ErrorMessage(String.Format("{1}:{0} 는 변수나 태그가 아닙니다", target, error_title));
            else
                ErrorMessage(String.Format("{1}:{0} is not a variable or tag", target, error_title));

            return false;
        }

        public bool GetAnalogInputPos(string tag, ref int[] pos)
        {
            EnumTagType type = 0;

            if (!TagLib.GetTagTypeAndPos(tag, ref type, ref pos))
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("없는 AI 태그 ({0})", tag));
                }
                else
                {
                    ErrorMessage(String.Format("Undefined AI({0}) Tag.", tag));
                }
                return false;
            }

            if (type != EnumTagType.AI)
            {
                if (Tools.IsLangKorean())
                {
                    ErrorMessage(String.Format("AI 태그가 아닙니다.({0})", tag));
                }
                else
                {
                    ErrorMessage(String.Format("It is not a AI tag.({0})", tag));
                }

                return false;
            }

            return true;
        }

        public TagAiClass FunctionAnalog_GetAnalogInputPoint(string command, string argument)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string tag;
            int[] pos = new int[1];

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!GetArgumentString(buf, out tag)) return null;

            if (!GetAnalogInputPos(tag, ref pos)) return null;

            return TagLib.GetStructAI(tag, ref pos);
        }

        void SaveCodeBuf(CommaTextWriter writer, string buf)
        {
            for (int i = 0; i < buf.Length; i++)
            {
                if (buf[i] == '\n') writer.WriteLine();
                else writer.Write(buf[i]);
            }
        }

        public static string MakeVarValueString(VAR_STRUCT var)
        {
            string result = "";
            if (var.type == EnumVarType.VAR_TYPE_char)
            {
                char[] values = (char[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", (ushort)(values[j]));
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_sbyte)
            {
                sbyte[] values = (sbyte[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_byte)
            {
                byte[] values = (byte[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_short)
            {
                short[] values = (short[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_ushort)
            {
                ushort[] values = (ushort[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_int)
            {
                int[] values = (int[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_uint)
            {
                uint[] values = (uint[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_long)
            {
                long[] values = (long[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_ulong)
            {
                ulong[] values = (ulong[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_float)
            {
                float[] values = (float[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_double)
            {
                double[] values = (double[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else if (var.type == EnumVarType.VAR_TYPE_string)
            {
                string[] values = (string[])var.val;
                for (int j = 0; j < var.size; j++)
                {
                    if (j != 0) result += ",";
                    result += String.Format("{0}", values[j]);
                }
            }
            else
            {

            }

            return result;
        }

        public void SaveFile(CommaTextWriter writer)
        {
            VAR_STRUCT var;
            string buf;
            int l;

            writer.WriteLine("Description,{0},", sDescription);
            writer.WriteLine("ScanTime,{0},", nScanTime);

            for (l = 0; l < arrayVar.Count; l++)
            {
                var = (VAR_STRUCT)arrayVar[l];
                buf = VarTypeToString(var.type);
                writer.WriteLine("Data,{0},{1},{2},{3}", buf, var.name, var.size, MakeVarValueString(var));
            }

            writer.WriteLine("Programm,BEGIN,");
            if (sProgramm != null)
            {
                SaveCodeBuf(writer, sProgramm.TrimEnd());
                writer.WriteLine();
            }
            writer.WriteLine("Programm,END,");
        }

        /*
        public void SaveFileToMODX(string filename)
        {
            string path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(filename)) Directory.CreateDirectory(path);

            CommaTextWriter writer = new CommaTextWriter(filename);
            if (writer == null)
            {
                MessageBox.Show(filename, "Can't write file");
                return;
            }
            SaveFile(writer);
            writer.Close();
        }*/

        bool IsTagEndChar(char ch)
        {
            if (ch == '\n') return true;
            if (ch == '\r') return true;
            if (ch == '\t') return true;
        
            if (ch == 0x20) return true;
            if (ch == '=') return true;
            if (ch == ';') return true;
            if (ch == ',') return true;
            if (ch == '*') return true;
            if (ch == '/') return true;
            if (ch == '+') return true;
            if (ch == '-') return true;
            if (ch == ')') return true;

            return false;
        }


        public void GetMultiSelectTagList(List<object> block, string used_filename, EnumTagUsedType used_type, object used_obj, string used_position)
        {
            int i;
            bool start = false;
            string tag = "";
            int wFileSize = sProgramm.Length;

            for (i = 0; i < wFileSize; i++)
            {
                if (start == false)
                {
                    if (sProgramm[i] == '$')
                    {
                        start = true;
                        tag = "";
                    }
                }
                else
                {
                    if (IsTagEndChar(sProgramm[i]))
                    {
                        if (tag.Length > 0)
                        {
                            // 태그 멤버가 포함되었을 때는 잘라낸다.
                            int index = tag.LastIndexOf('.');
                            if (index != -1)
                            {
                                if (TagUtil.IsTagMember(tag.Substring(index + 1)))
                                {
                                    tag = tag.Substring(0, index);
                                }
                            }

                            int[] pos = new int[1];
                            EnumTagType type = EnumTagType.none;
                            TagLib.GetTagTypeAndPos(tag, ref type, ref pos);
                            TagUtil.AddTagList(block, tag, type, used_filename, used_type, used_obj, used_position);
                        }
                        start = false;
                    }
                    else
                    {
                        tag += sProgramm[i];
                    }
                }
            }
        }
        
        public void SetMultiSelectTagList(List<object> block)
        {
            int i;
            bool start = false;
            string tag = "";
            string t = "";
            bool change_flag = false;
            int wFileSize = sProgramm.Length;

            for (i = 0; i < wFileSize; i++)
            {
                if (start == false)
                {
                    if (sProgramm[i] == '$')
                    {
                        start = true;
                        tag = "";

                    }
                    t += sProgramm[i];
                }
                else
                {
                    if (IsTagEndChar(sProgramm[i]))
                    {
                        if (tag.Length > 0)
                        {
                            // 태그 멤버가 포함되었을 때는 잘라낸다.
                            int index = tag.LastIndexOf('.');
                            string member = "";
                            if (index != -1)
                            {
                                if (TagUtil.IsTagMember(tag.Substring(index + 1)))
                                {
                                    member = tag.Substring(index);
                                    tag = tag.Substring(0, index);
                                }
                            }

                            if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                            {
                                change_flag = true;
                            }

                            t += tag;
                            t += member;
                        }
                        start = false;

                        t += sProgramm[i];
                    }
                    else
                    {
                        tag += sProgramm[i];
                    }
                }
            }

            if (change_flag)
            {
                SetBuf(t);
            }
        }

    }
}
