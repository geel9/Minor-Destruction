using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MiningGameServer.Interfaces;

namespace MiningGameServer.Packets
{
    public class Packet
    {
        public List<byte> data = new List<byte>();
        public int offset = 0;

        public byte[] getData()
        {
            return data.ToArray();
        }

        public T ReadNT<T>() where T : INetTransferable<T>, new()
        {
            return (new T()).Read(this);
        }

        public void WriteNT<T>(INetTransferable<T> trans)
        {
            trans.Write(this);
        } 

        public short ReadShort()
        {
            short num = BitConverter.ToInt16(getData(), offset);
            offset += 2;
            return num;
        }

        public Vector2 ReadVector()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector2 ReadVectorS()
        {
            return new Vector2(ReadShort(), ReadShort());
        }

        public Vector2 ReadVectorB()
        {
            return new Vector2(ReadByte(), ReadByte());
        }

        public Vector2 ReadVectorSB()
        {
            return new Vector2(ReadSByte(), ReadSByte());
        }

        public byte ReadByte()
        {
            byte b = data[offset];
            offset++;
            return b;
        }

        public sbyte ReadSByte()
        {
            sbyte b = (sbyte)data[offset];
            offset++;
            return b;
        }

        public bool ReadBool()
        {
            bool flag = BitConverter.ToBoolean(getData(), offset);
            offset++;
            return flag;
        }

        public float ReadFloat()
        {
            float num = BitConverter.ToSingle(getData(), offset);
            offset += 4;
            return num;
        }

        public int ReadInt()
        {
            int num = BitConverter.ToInt32(getData(), offset);
            offset += 4;
            return num;
        }

        public char ReadChar()
        {
            char c = BitConverter.ToChar(getData(), offset);
            offset += 2;
            return c;
        }

        public string ReadString()
        {
            string ret = String.Empty;
            short count = ReadShort();
            for (int i = 0; i < count; i++)
            {
                ret += ReadChar();
            }
            return ret;
        }

        public void WriteChar(char toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteVector(Vector2 toWrite)
        {
            WriteFloat(toWrite.X);
            WriteFloat(toWrite.Y);
        }

        public void WriteVectorS(Vector2 toWrite)
        {
            WriteShort((short)toWrite.X);
            WriteShort((short)toWrite.Y);
        }

        public void WriteVectorB(Vector2 toWrite)
        {
            WriteByte((byte)toWrite.X);
            WriteByte((byte)toWrite.Y);
        }

        public void WriteVectorSB(Vector2 toWrite)
        {
            WriteSByte((sbyte)toWrite.X);
            WriteSByte((sbyte)toWrite.Y);
        }

        public void WriteShort(short toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteSByte(sbyte toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteInt(int toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteByte(byte toWrite)
        {
            data.Add(toWrite);
        }

        public void WriteBool(bool toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteFloat(float toWrite)
        {
            WriteBytes(BitConverter.GetBytes(toWrite));
        }

        public void WriteString(string toWrite)
        {
            WriteShort((short)toWrite.Length);
            for (int i = 0; i < toWrite.Length; i++)
            {
                WriteChar(toWrite[i]);
            }
        }

        public void WriteBytes(byte[] toWrite)
        {
            data.AddRange(toWrite);
        }

        public void WriteBytes(byte[] toWrite, int offset, int length)
        {
            for(int i = offset; i < offset + length; i++)
            {
                data.Add(toWrite[i]);
            }
        }

        public void WriteAll(object[] objects)
        {
            foreach (object o in objects)
            {
                if (o is int)
                {
                    WriteInt((int)o);
                }
                else if (o is string)
                {
                    WriteString((string)o);
                }
                else if (o is float)
                {
                    WriteFloat((float)o);
                }
                else if (o is bool)
                {
                    WriteBool((bool)o);
                }
                else if (o is char)
                {
                    WriteChar((char)o);
                }
                else if (o is byte)
                {
                    WriteByte((byte)o);
                }
                else if (o is sbyte)
                {
                    WriteSByte((sbyte)o);
                }
                else if (o is short)
                {
                    WriteShort((short)o);
                }
            }
        }

        public Packet(params object[] objects)
        {
            this.offset = 0;
            this.data = new List<byte>();
            WriteAll(objects);
        }

        public Packet(byte[] data)
        {
            this.offset = 0;
            this.data = data.ToList<byte>();
        }
    }
}
