using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetTools;

namespace ScriptLibRun
{
    public enum EnumBlockType : byte {
        Known = 0,

        Version = 1,

        // Method까지의 Block
        LibraryBlock = 3,
        NamespaceBlock = 4,
        ClassBlock = 5,
        EnumBlock = 6,
        MethodBlock = 7,
        Param = 8,
        InOutType = 9,

        ClassVariable = 10,
        Variable = 11,
        sVarType = 12,
        eVarType = 13,
        VarDimension = 14,

        // 
        Name = 15,
        SourceFilename=16,
        EnumMember = 17,
        EnumValue = 18,

        NameSplitNamespace = 20,
        NameSplitClass = 21,
        NameSplitMethod = 22,

        bStatic = 23,
        eAccessLevel = 24,
        Property = 25,
        bStruct = 26,
        bConst = 27,
        bReadonly = 28,

        CommandBlock = 65,
        CommandBlock2 = 66,
        CommandBlock3 = 67,
        
        CommandBreak=69,
        CommandContinue=70,
        CommandDeclaration=71,
        CommandElse=72,
        CommandFor=73,
        CommandGoto=74,
        CommandIf=75,
        CommandLabel=76,
        CommandMethod=77,
        CommandMethodArg = 78,
        CommandReturn=79,
        CommandSubstitution=80,
        CommandWhile=81,
        CommandVoidValue = 82,
        CommandLastLine = 83,

        Condition = 90,
        MethodType = 91,
        DimensinalArray = 92,
        DimensionalPosition = 93,
        InitialValue = 94,
        RecursiveValue = 95,
        ColRow = 96,
        ReturnDataType = 97,

        LastValueProperty = 98,
        //eCalcMethod = 98,
        //eValueType = 99,
        
        sLastValue = 100,
        SourceString = 101,
        eCompare = 102, // 기본 비교
        eCompareMulti = 103,  // && || 등의 조건

        BaseClassBlock = 110,   // class a : b {} 와 같이 base class의 b선언부분
    }

    public class ScriptWriter
    {
        MemoryStream stream;
        BinaryWriter writer;

        public ScriptWriter()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        /*
        public void Close()
        {
            stream.Close();
        }*/

        byte[] GetUtf8FromString(string value)
        {
            MemoryStream ms = new MemoryStream();
            TextWriter br = new StreamWriter(ms);
            br.Write(value);
            br.Flush();
            byte[] buffer = ms.ToArray();
            br.Close();

            return buffer;
        }
        /*
        void LocalWriteString(string value)
        {
            // 문자를 하나하나 저장해야 유니코드로 저장된다. 기본 모드가 UTF8이다. UTF8은 계산하기 어려우므로
            for (int i = 0; i < value.Length; i++)
            {
                writer.Write((ushort)value[i]);
            }
        }*/

        void LocalWriteType(EnumBlockType type)
        {
            writer.Write((byte)type);
        }

        void LocalWriteBool(bool value)
        {
            writer.Write(value ? (byte)1 : (byte)0);
        }

        public void WriteString(EnumBlockType type, string value)
        {
            if (value == null) return;

            LocalWriteType(type);
            byte[] buffer = GetUtf8FromString(value);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            //writer.Write(value.Length * 2);
            //LocalWriteString(value);
        }

        public void WriteByte(EnumBlockType type, byte value)
        {
            LocalWriteType(type);
            writer.Write((int)1);
            writer.Write(value);
        }

        public void WriteBool(EnumBlockType type, bool value)
        {
            LocalWriteType(type);
            writer.Write((int)1);
            LocalWriteBool(value);
        }

        public void WriteInt(EnumBlockType type, int value)
        {
            LocalWriteType(type);
            writer.Write((int)4);
            writer.Write(value);
        }

        public void WriteArray(EnumBlockType type, int[] value)
        {
            LocalWriteType(type);
            writer.Write(value.Length*4);
            for (int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }

        public void WriteColRow(int column, int row)
        {
            LocalWriteType(EnumBlockType.ColRow);
            writer.Write(8);
            writer.Write(column);
            writer.Write(row);
        }

        public void WriteNameSplit(string sNamespace, string sClass)
        {
            if(sNamespace != null)
                WriteString(EnumBlockType.NameSplitNamespace, sNamespace);
            if (sClass != null)
                WriteString(EnumBlockType.NameSplitClass, sClass);
        }

        public void WriteNameSplit(string sNamespace, string sClass, string sMethod)
        {
            if (sNamespace != null)
                WriteString(EnumBlockType.NameSplitNamespace, sNamespace);
            if (sClass != null)
                WriteString(EnumBlockType.NameSplitClass, sClass);
            if (sMethod != null)
                WriteString(EnumBlockType.NameSplitMethod, sMethod);
        }

        public void WriteName(string value)
        {
            WriteString(EnumBlockType.Name, value);
        }

        public void WriteStatic(bool value)
        {
            WriteBool(EnumBlockType.bStatic, value);
        }

        public void WriteConst(bool value)
        {
            WriteBool(EnumBlockType.bConst, value);
        }

        public void WriteReadonly(bool value)
        {
            WriteBool(EnumBlockType.bReadonly, value);
        }

        public void WriteAccessLevel(EnumAccessLevel value)
        {
            WriteByte(EnumBlockType.eAccessLevel, (byte)value);
        }

        public void WriteProperty(bool bProperty, bool bGet, bool bSet)
        {
            LocalWriteType(EnumBlockType.Property);
            writer.Write(3);
            LocalWriteBool(bProperty);
            LocalWriteBool(bGet);
            LocalWriteBool(bSet);
        }

        public void WriteLastValueProperty(RecursiveValue.EnumCalcMethod eCalcMethod, RecursiveValue.EnumLastValueType eValueType, EnumVarType eConstantType)
        {
            LocalWriteType(EnumBlockType.LastValueProperty);
            writer.Write(3);
            writer.Write((byte)eCalcMethod);
            writer.Write((byte)eValueType);
            writer.Write((byte)eConstantType);
        }

        public void WriteSourceFilename(string value)
        {
            WriteString(EnumBlockType.SourceFilename, value);
        }

        public byte[] ToArray()
        {
            writer.Flush();
            writer.Close();
            return stream.ToArray();
        }
        
        public void WriteBlock(EnumBlockType block_type, ScriptWriter wc)
        {
            byte[] buffer = wc.ToArray();
            int block_size = buffer.Length;

            LocalWriteType(block_type);
            writer.Write(block_size);
            writer.Write(buffer);
        }

        public const int nVersionMajor = 1;
        public const int nVersionMinor = 0;
        public const int nVersionBuild = 0;
        public const int nVersionRevision = 0;

        public void WriteVersion()
        {
            LocalWriteType(EnumBlockType.Version);
            writer.Write(4 * 4);
            writer.Write(nVersionMajor);
            writer.Write(nVersionMinor);
            writer.Write(nVersionBuild);
            writer.Write(nVersionRevision);
        }
    }
}
