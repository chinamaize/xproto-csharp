using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    public class PhoneNumber : Message
    {
        public int PhoneType { get; set; }
        public String Number { get; set; }

        public override void Decode(ProtoStream stream)
        {
            base.Decode(stream);

            int tag = 0;
            while (--fieldCount >= 0)
            {
                tag = stream.ReadInt();

                switch ((tag >> ProtoDefine.TAG_TYPE_BITS))
                {
                    case 1:
                        {
                            PhoneType = stream.ReadInt();
                            break;
                        }
                    case 2:
                        {
                            Number = stream.ReadString();
                            break;
                        }
                    default:
                        stream.ReadUnknow(tag);
                        return;
                }
            }
        }

        public override void Encode(ProtoStream buffer)
        {
            base.Encode(buffer);

            fieldCount += buffer.Write(1, PhoneType);
            fieldCount += buffer.Write(2, Number);

            Close(buffer);
        }

        public override string ToString()
        {
            return "PhoneNumber [PhoneType=" + PhoneType + ", Number=" + Number
                    + "]";
        }
    }


    public class Person : Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<PhoneNumber> Phones { get; set; }

        public long Money { get; set; }

        public PhoneNumber Mobile { get; set; }

        public List<int> Codes { get; set; }

        public List<long> Datas { get; set; }

        public List<string> Cards { get; set; }

        public override void Encode(ProtoStream stream)
        {
            base.Encode(stream);

            fieldCount += stream.Write(1, Id);
            fieldCount += stream.Write(2, Name);
            fieldCount += stream.Write(3, Email);
            fieldCount += stream.Write(10, Money);
            fieldCount += stream.Write(11, Mobile);

            fieldCount += stream.WriteIntList(12, Codes);
            fieldCount += stream.WriteStringList(13, Cards);
            fieldCount += stream.WriteLongList(14, Datas);
            fieldCount += stream.WriteObjectList(4, Phones);

            Close(stream);
        }

        public override void Decode(ProtoStream stream)
        {
            base.Decode(stream);

            int tag = 0;

            while (fieldCount-- > 0)
            {
                tag = stream.ReadInt();
                switch (tag >> ProtoDefine.TAG_TYPE_BITS)
                {
                    case 1:
                        {
                            Id = stream.ReadInt();
                            break;
                        }
                    case 2:
                        {
                            Name = stream.ReadString();
                            break;
                        }
                    case 3:
                        {
                            Email = stream.ReadString();
                            break;
                        }
                    case 4:
                        {
                            Phones = stream.ReadObjectList<PhoneNumber>();
                            break;
                        }
                    case 10:
                        {
                            Money = stream.ReadLong();
                            break;
                        }
                    case 11:
                        {
                            Mobile = stream.ReadObject<PhoneNumber>();
                            break;
                        }
                    case 12:
                        {
                            Codes = stream.ReadIntList();
                            break;
                        }
                    case 13:
                        {
                            Cards = stream.ReadStringList();
                            break;
                        }
                    case 14:
                        {
                            Datas = stream.ReadLongList();
                            break;
                        }

                    default:

                        stream.ReadUnknow(tag);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return "Person [Id=" + Id + ", Name=" + Name + ", Email=" + Email
                + ", Phones=" + Phones + ", Money=" + Money + ", Mobile="
                + Mobile + ", Codes=" + Codes + ", Cards=" + Cards + ", Datas="
                + Datas + "]";
        }
    }
}
