using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ScriptLibRun
{
    public class ScriptReader
    {
        MemoryStream stream;
        BinaryReader reader;

        public ScriptReader(byte[] buffer)
        {
            stream = new MemoryStream(buffer);
            reader = new BinaryReader(stream);
        }

        public ScriptReaderBlock ReadBlock()
        {
            if (stream.Position >= stream.Length) return null;

            ScriptReaderBlock block = new ScriptReaderBlock();

            block.type = (EnumBlockType)reader.ReadByte();
            block.block_size = reader.ReadInt32();
            block.block_data = reader.ReadBytes(block.block_size);

            if (block.block_size != block.block_data.Length)
            {
                string msg = String.Format("BlockType={0}\nBlockSize={1}\nBlockData={2}", block.type, block.block_size, block.block_data);
                MessageBox.Show(msg, "ScriptReader error");
                return null;
            }

            return block;
        }

        /*
        public string ReadString()
        {
            int size = (int)(stream.Length / 2);
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                s.Append((char)reader.ReadUInt16());
            }

            return s.ToString();
        }*/

        public bool ReadBool()
        {
            return reader.ReadByte() == 1;
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public void Close()
        {
            stream.Close();
        }
    }

    public class ScriptReaderBlock
    {
        public EnumBlockType type;
        public int block_size;
        public byte[] block_data;

        public string ReadString()
        {
            // ScriptReader reader = new ScriptReader(block_data);
            MemoryStream ms = new MemoryStream(block_data);
            TextReader tr = new StreamReader(ms);
            string s = tr.ReadToEnd();
            tr.Close();
            return s;
            //return reader.ReadString();
        }

        public bool ReadBool()
        {
            ScriptReader reader = new ScriptReader(block_data);

            return reader.ReadBool();
        }

        public int ReadInt()
        {
            ScriptReader reader = new ScriptReader(block_data);

            return reader.ReadInt();
        }

        public byte ReadByte()
        {
            ScriptReader reader = new ScriptReader(block_data);

            return reader.ReadByte();
        }

        public void ReadProperty(out bool bProperty, out bool bPropertyGet, out bool bPropertySet)
        {
            ScriptReader reader = new ScriptReader(block_data);

            bProperty = reader.ReadBool();
            bPropertyGet = reader.ReadBool();
            bPropertySet = reader.ReadBool();
        }

        public void ReadColRow(out int column, out int row)
        {
            ScriptReader reader = new ScriptReader(block_data);

            column = reader.ReadInt();
            row = reader.ReadInt();
        }

        public void ReadLastValueProperty(out RecursiveValue.EnumCalcMethod eCalcMethod, out RecursiveValue.EnumLastValueType eValueType, out EnumVarType eConstantType)
        {
            ScriptReader reader = new ScriptReader(block_data);

            eCalcMethod = (RecursiveValue.EnumCalcMethod)reader.ReadByte();
            eValueType = (RecursiveValue.EnumLastValueType)reader.ReadByte();
            eConstantType = (EnumVarType)reader.ReadByte();
        }

        public int[] ReadArray()
        {
            ScriptReader reader = new ScriptReader(block_data);

            int count = block_size/4;
            int[] array = new int[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = reader.ReadInt();
            }

            return array;
        }

        public void ReadVersion(out int major, out int minor, out int build, out int revision)
        {
            ScriptReader reader = new ScriptReader(block_data);

            major = reader.ReadInt();
            minor = reader.ReadInt();
            build = reader.ReadInt();
            revision = reader.ReadInt();
        }
    }
}
