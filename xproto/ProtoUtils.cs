using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    public class ProtoUtils
    {
        static readonly char[] HEX_DIGITS = { '0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static String hex(byte[] bytes)
        {
            char[] result = new char[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                result[i * 2] = HEX_DIGITS[(bytes[i] >> 4) & 0xf];
                result[i * 2 + 1] = HEX_DIGITS[bytes[i] & 0xf];
            }
            return new String(result);
        }

        public static ProtoBuffer decodeHex(String hex)
        {
            if (hex == null)
                throw new ArgumentException("hex == null");
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Unexpected hex string: " + hex);

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                int d1 = decodeHexDigit(hex[i * 2]) << 4;
                int d2 = decodeHexDigit(hex[i * 2 + 1]);
                result[i] = (byte)(d1 + d2);
            }
            return new ProtoBuffer(result);
        }

        public static int decodeHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            throw new ArgumentException("Unexpected hex digit: " + c);
        }

        public static void dump(ProtoStream stream)
        {
            dump(stream, 0);
        }

        public static void dump(ProtoStream stream, int level)
        {
            System.Console.WriteLine();
            short fieldCount = stream.ReadFixedShort();

            while (fieldCount-- > 0)
            {
                dumpField(stream, level);
            }
        }

        private static void dumpField(ProtoStream stream, int level)
        {
            int tagAndType = stream.ReadInt();
            int tag = (tagAndType >> ProtoDefine.TAG_TYPE_BITS);
            ProtoType type = (ProtoType)(tagAndType & ProtoDefine.TAG_TYPE_MASK);

            for (int i = 0; i < level; i++)
            {
                System.Console.Write("\t");
            }

            System.Console.Write("T:{0}\tTYPE:{1} = ", tag, type);

            switch (type)
            {
                case ProtoType.VarInt:
                    {
                        System.Console.Write(stream.ReadInt());
                        break;
                    }
                case ProtoType.VarLong:
                    {
                        System.Console.Write(stream.ReadLong());
                        break;
                    }
                case ProtoType.String:
                    {
                        System.Console.Write(stream.ReadString());
                        break;
                    }
                case ProtoType.Object:
                    {

                        dump(stream, level + 1);
                        break;
                    }

                case ProtoType.VarIntList:
                    {
                        int count = stream.ReadInt();
                        System.Console.Write(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            System.Console.Write(stream.ReadInt());
                            if (i < count - 1)
                                System.Console.Write(",");
                        }

                        System.Console.Write("]");

                        break;
                    }
                case ProtoType.VarLongList:
                    {
                        int count = stream.ReadInt();
                        System.Console.Write(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            System.Console.Write(stream.ReadLong());
                            if (i < count - 1)
                                System.Console.Write(",");
                        }

                        System.Console.Write("]");

                        break;
                    }
                case ProtoType.StringList:
                    {
                        int count = stream.ReadInt();
                        System.Console.Write(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            System.Console.Write(stream.ReadString());
                            if (i < count - 1)
                                System.Console.Write(",");
                        }

                        System.Console.Write("]");

                        break;
                    }
                case ProtoType.ObjectList:
                    {
                        int count = stream.ReadInt();
                        System.Console.Write(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            dump(stream, level + 1);
                        }

                        System.Console.Write("\t]");

                        break;
                    }
                default:
                    break;
            }
            System.Console.WriteLine();
        }
    }
}
