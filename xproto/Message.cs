using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    public abstract class Message
    {
        private static readonly byte[] HEADER = new byte[] { 0, 0 };

        protected int fieldCount = 0;

        private int startPos = 0;

        public virtual void Encode(ProtoStream stream)
        {
            startPos = stream.Position;
            stream.Put(HEADER);
        }

        public virtual void Decode(ProtoStream stream)
        {
            fieldCount = stream.ReadFixedShort();
        }

        protected void Close(ProtoStream stream)
        {
            if (fieldCount > 0)
            {
                stream.WriteFixedShort((short)fieldCount, startPos);
            }
        }
    }
}
