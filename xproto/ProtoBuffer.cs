using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    public class ProtoBuffer
    {
        protected byte[] data;
        protected int size;
        protected int position;

        public ProtoBuffer(byte[] bytes)
        {
            position = 0;
            size = bytes.Length;
            data = bytes;
        }

        public ProtoBuffer()
            : this(new byte[ProtoDefine.DEFAULT_BUFFER_SIZE])
        {
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Available
        {
            get { return size - position; }
        }

        private void expand(int sz)
        {
            if (Available < sz)
            {
                sz = (int)Math.Ceiling((double)sz / ProtoDefine.DEFAULT_BUFFER_SIZE) * ProtoDefine.DEFAULT_BUFFER_SIZE;
                size += sz;
                byte[] temp = new byte[size];
                Array.Copy(data, 0, temp, 0, position);
                data = temp;
            }
        }

        public void Put(byte b)
        {
            expand(1);
            data[position] = b;
            position++;
        }

        public void Put(byte b, int pos)
        {
            if (pos < size)
            {
                data[pos] = b;
            }
            else
            {
                throw new ArgumentOutOfRangeException("pos");
            }
        }

        public void Put(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            expand(bytes.Length);
            Array.Copy(bytes, 0, data, position, bytes.Length);
            position += bytes.Length;
        }

        public byte Get()
        {
            return Get(position++);
        }

        public byte Get(int pos)
        {
            if (pos > position)
                throw new ArgumentOutOfRangeException("pos");

            return data[pos];
        }

        public byte[] ToArray()
        {
            byte[] temp = new byte[position];
            Array.Copy(data, 0, temp, 0, position);
            return temp;
        }

        public string Hex()
        {
            return ProtoUtils.hex(ToArray());
        }
    }
}
