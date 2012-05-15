using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public class Packet
    {
        public List<byte> data = new List<byte>();
        public int offset = 0;

        public byte[] getData()
        {
            return data.ToArray();
        }

        public short readShort()
        {
            short num = BitConverter.ToInt16(getData(), offset);
            offset += 2;
            return num;
        }

        public byte readByte()
        {
            byte b = data[offset];
            offset++;
            return b;
        }

        public sbyte readSByte()
        {
            sbyte b = (sbyte)data[offset];
            offset++;
            return b;
        }

        public bool readBool()
        {
            bool flag = BitConverter.ToBoolean(getData(), offset);
            offset++;
            return flag;
        }

        public float readFloat()
        {
            float num = BitConverter.ToSingle(getData(), offset);
            offset += 4;
            return num;
        }

        public int readInt()
        {
            int num = BitConverter.ToInt32(getData(), offset);
            offset += 4;
            return num;
        }

        public char readChar()
        {
            char c = BitConverter.ToChar(getData(), offset);
            offset += 2;
            return c;
        }

        public string readString()
        {
            string ret = String.Empty;
            short count = readShort();
            for (int i = 0; i < count; i++)
            {
                ret += readChar();
            }
            return ret;
        }

        public void writeChar(char toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeShort(short toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeSByte(sbyte toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeInt(int toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeByte(byte toWrite)
        {
            data.Add(toWrite);
        }

        public void writeBool(bool toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeFloat(float toWrite)
        {
            writeBytes(BitConverter.GetBytes(toWrite));
        }

        public void writeString(string toWrite)
        {
            writeShort((short)toWrite.Length);
            for (int i = 0; i < toWrite.Length; i++)
            {
                writeChar(toWrite[i]);
            }
        }

        public void writeBytes(byte[] toWrite)
        {
            data.AddRange(toWrite);
        }

        public void writeAll(object[] objects)
        {
            foreach (object o in objects)
            {
                if (o.GetType() == typeof(int))
                {
                    writeInt((int)o);
                }
                if (o.GetType() == typeof(string))
                {
                    writeString((string)o);
                }
                if (o.GetType() == typeof(float))
                {
                    writeFloat((float)o);
                }
                if (o.GetType() == typeof(bool))
                {
                    writeBool((bool)o);
                }
                if (o.GetType() == typeof(char))
                {
                    writeChar((char)o);
                }
                if (o.GetType() == typeof(byte))
                {
                    writeByte((byte)o);
                }
                if (o.GetType() == typeof(sbyte))
                {
                    writeSByte((sbyte)o);
                }
                if (o.GetType() == typeof(short))
                {
                    writeShort((short)o);
                }
            }
        }

        public Packet(params object[] objects)
        {
            this.offset = 0;
            this.data = new List<byte>();
            writeAll(objects);
        }

        public Packet(byte[] data)
        {
            this.offset = 0;
            this.data = data.ToList<byte>();
        }
    }
}
