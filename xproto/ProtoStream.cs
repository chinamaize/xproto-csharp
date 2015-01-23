using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    public class ProtoStream : ProtoBuffer
    {
        static readonly Encoding CHARSET = Encoding.UTF8;

        public ProtoStream() : base() { }

        public ProtoStream(byte[] bytes) : base(bytes) { }

        public void WriteFixedShort(short value, int pos)
        {
            Put((byte)(value & 0xff), pos++);
            Put((byte)((value >> 8) & 0xff), pos);
        }

        public short ReadFixedShort()
        {
            short value = (short)(Get() & 0xff);
            value += (short)((Get() & 0xff) << 8);
            return value;
        }

        public void WriteVarInt32(int value)
        {
            while ((value & 0xFFFFFF80) != 0L)
            {
                Put((byte)(value & 0x7F | 0x80));
                value = value >> 7;
            }
            Put((byte)(value & 0x7F));
        }

        public void WriteVarInt64(long value)
        {
            while (((ulong)value & 0xFFFFFFFFFFFFFF80L) != 0L)
            {
                Put((byte)(value & 0x7F | 0x80));
                value = value >> 7;
            }
            Put((byte)(value & 0x7F));
        }

        public void WriteString(String val)
        {
            if (string.IsNullOrEmpty(val))
            {
                WriteVarInt32(0);
                return;
            }
            byte[] bytes = CHARSET.GetBytes(val);
            WriteVarInt32(bytes.Length);
            Put(bytes);
        }

        public int ReadInt()
        {
            int value = 0;
            int i = 0;
            int b;
            while (((b = Get()) & 0x80) != 0)
            {
                value |= (b & 0x7F) << i;
                i += 7;
                if (i > 35)
                {
                    throw new ArgumentOutOfRangeException("Variable length quantity is too long");
                }
            }
            return value | (b << i);
        }

        public long ReadLong()
        {
            long value = 0L;
            int i = 0;
            long b;
            while (((b = Get()) & 0x80) != 0)
            {
                value |= (b & 0x7F) << i;
                i += 7;
                if (i > 63)
                {
                    throw new ArgumentOutOfRangeException("Variable length quantity is too long");
                }
            }
            return value | (b << i);
        }

        public string ReadString()
        {
            int len = ReadInt();

            if (len > 0)
            {
                byte[] temp = new byte[len];
                Array.Copy(data, position, temp, 0, temp.Length);
                position += temp.Length;
                return CHARSET.GetString(temp);
            }
            return null;
        }

        public List<int> ReadIntList()
        {
            int count = ReadInt();
            List<int> list = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadInt());
            }

            return list;
        }

        public List<long> ReadLongList()
        {
            int count = ReadInt();
            List<long> list = new List<long>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadLong());
            }

            return list;
        }

        public List<string> ReadStringList()
        {
            int count = ReadInt();
            List<string> list = new List<string>();

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadString());
            }

            return list;
        }

        public T ReadObject<T>() where T : Message
        {
            T item = Activator.CreateInstance<T>();
            item.Decode(this);
            return item;
        }

        public List<T> ReadObjectList<T>() where T : Message
        {
            int size = ReadInt();
            if (size <= 0)
                return null;

            List<T> list = new List<T>();

            for (int i = 0; i < size; i++)
            {
                list.Add(ReadObject<T>());
            }

            return list;
        }

        public int Write(int tag, int val)
        {
            if (val != 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarInt);
                WriteVarInt32(tag);
                WriteVarInt32(val);
                return 1;
            }
            return 0;
        }

        public int Write(int tag, String val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.String);

                WriteVarInt32(tag);
                WriteString(val);
                return 1;
            }
            return 0;
        }

        public int Write(int tag, Int64 val)
        {
            if (val != 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarLong);

                WriteVarInt32(tag);
                WriteVarInt64(val);
                return 1;
            }
            return 0;
        }

        public int Write<T>(int tag, T val) where T : Message
        {
            if (val != null)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.Object);

                WriteVarInt32(tag);
                val.Encode(this);
                return 1;
            }
            return 0;
        }
        public int WriteIntList(int tag, List<int> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarIntList);

                WriteVarInt32(tag);
                WriteVarInt32(list.Count);

                foreach (var i in list)
                {
                    WriteVarInt32(i);
                }
                return 1;
            }
            return 0;
        }

        public int WriteLongList(int tag, List<long> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarLongList);

                WriteVarInt32(tag);
                WriteVarInt32(list.Count);

                foreach (var i in list)
                {
                    WriteVarInt64(i);
                }
                return 1;
            }
            return 0;
        }   

        public int WriteStringList(int tag, List<string> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.StringList);

                WriteVarInt32(tag);
                WriteVarInt32(list.Count);

                foreach (var i in list)
                {
                    WriteString(i);
                }
                return 1;
            }
            return 0;
        }

        public int WriteObjectList<T>(int tag, List<T> list) where T : Message
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.ObjectList);

                WriteVarInt32(tag);
                WriteVarInt32(list.Count);

                foreach (var i in list)
                {
                    i.Encode(this);
                }
                return 1;
            }
            return 0;
        }

        public void ReadUnknow(int tag)
        {
            ReadUnknow((ProtoType)(tag & ProtoDefine.TAG_TYPE_MASK));
        }

        public void ReadUnknow(ProtoType type)
        {
            switch (type)
            {
                case ProtoType.VarInt:
                    {
                        ReadInt();
                        break;
                    }
                case ProtoType.VarLong:
                    {
                        ReadLong();
                        break;
                    }
                case ProtoType.String:
                    {
                        ReadString();
                        break;
                    }
                case ProtoType.VarIntList:
                    {
                        ReadIntList();
                        break;
                    }
                case ProtoType.VarLongList:
                    {
                        ReadLongList();
                        break;
                    }
                case ProtoType.StringList:
                    {
                        ReadStringList();
                        break;
                    }
                case ProtoType.Object:
                    {
                        ReadUnknowObject();
                        break;
                    }
                case ProtoType.ObjectList:
                    {
                        ReadUnknowObjectList();
                        break;
                    }
                default:

                    break;
            }
        }

        private void ReadUnknowObject()
        {
            int fields = Get() & 0xff;
            fields += (Get() & 0xff) << 8;

            int tag = 0;
            while (fields-- > 0)
            {
                tag = ReadInt();
                ReadUnknow((ProtoType)tag);
            }
        }

        private void ReadUnknowObjectList()
        {
            int count = ReadInt();

            for (int i = 0; i < count; i++)
            {
                ReadUnknowObject();
            }
        }
    }
}
