using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProto
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProtoStream stream = new ProtoStream();

            //for (int i = 0; i < 200; i++)
            //{
            //    stream.WriteVarInt64(i);
            //}
            //stream.Position = 0;
            //for (int i = 0; i < 200; i++)
            //{
            //    System.Console.WriteLine(stream.ReadLong());
            //}

            //System.Console.Read();


            Person person = new Person();
            person.Id = 1;
            person.Name = "中文";
            person.Email = "sunlu@foxmail.com";
            person.Money = 1000L;
            person.Mobile = new PhoneNumber();
            person.Mobile.PhoneType = 1;
            person.Mobile.Number = "13800138000";

            person.Phones = new List<PhoneNumber>();

            PhoneNumber num = new PhoneNumber();
            num.PhoneType = 2;
            num.Number = "123456";
            person.Phones.Add(num);

            num = new PhoneNumber();
            num.PhoneType = 3;
            num.Number = "6543dfsdf21";
            person.Phones.Add(num);

            ProtoStream buffer = new ProtoStream();
            person.Encode(buffer);

            byte[] data = buffer.ToArray();

            System.Console.WriteLine("Person pack size=" + data.Length + "，Hex="
                    + buffer.Hex());

            ProtoStream buffer2 = new ProtoStream(data);
            Person person2 = new Person();
            person2.Decode(buffer2);

            System.Console.WriteLine("Person is =" + person2.ToString());

            buffer2.Position = 0;
            ProtoUtils.dump(buffer2);

            System.Console.Read();

        }
    }
}
