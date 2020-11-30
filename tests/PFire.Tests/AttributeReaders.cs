using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PFire.Core.Protocol;

namespace PFire.Tests
{
    public class AttributeReaders
    {

        [Test]
        public void TestReadMapMessage()
        {
            //0x5b, 0x00, 0x02, 0x00, 0x02, 0x03, 0x73, 0x69, 0x64, 0x03, 0x0b, 0x8e, 0xeb, 0x3e, 0x81, 0xf8, 0xd9, 0x4f, 0x6a, 0xed, 0x08, 0x54, 0x8f, 0x96, 0xd3, 0x93, 0x07, 0x70, 0x65, 0x65, 0x72, 0x6d, 0x73, 0x67, 
            byte[] map = {   0x05, 0x03, 
                             0x07, 0x6d, 0x73, 0x67, 0x74, 0x79, 0x70, 0x65, 0x02, 0x00, 0x00, 0x00, 0x00, 
                             0x07, 0x69, 0x6d, 0x69, 0x6e, 0x64, 0x65, 0x78, 0x02, 0x04, 0x00, 0x00, 0x00, 
                             0x02, 0x69, 0x6d, 0x01, 0x05, 0x00, 0x64, 0x75, 0x64, 0x65 
                         };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(map)))
            {
                byte type = reader.ReadByte();
                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                Dictionary<string, dynamic> values = attribute.ReadValue(reader);
                Assert.IsTrue(values["msgtype"] == 0 && values["imindex"] == 4 && values["im"] == "dude");
            }
        }

        [Test]
        public void TestReadStringKeyMap()
        {
            byte[] map = { 0x05, 0x02, 0x04, 0x6b, 0x65, 0x79, 0x31, 0x02, 0x01, 0x00, 0x00, 0x00,
                                       0x04, 0x6b, 0x65, 0x79, 0x32, 0x02, 0x02, 0x00, 0x00, 0x00 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(map)))
            {
                byte type = reader.ReadByte();

                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                Dictionary<string, dynamic> value = attribute.ReadValue(reader);

                Assert.IsTrue(value["key1"] == 1 && value["key2"] == 2);
            }

        }

        [Test]
        public void TestReadInt8KeyMap()
        {
            byte[] map = { 0x09, 0x02, 0x01, 0x01, 0x01, 0x00, 0x030, 0x02, 0x01, 0x01, 0x00, 0x31 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(map)))
            {
                byte type = reader.ReadByte();

                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                Dictionary<byte, dynamic> value = attribute.ReadValue(reader);

                Assert.IsTrue(value[0x01] == "0" && value[0x02] == "1");
            }

        }

        [Test]
        public void TestReadListInt32()
        {
            byte[] integer32List = { 0x04, 0x02, 0x02, 0x00, 0x8a, 0x61, 0x3f, 0x00, 0x8b, 0x61, 0x3f, 0x00 };
            byte[] stringList = { 0x04, 0x01, 0x02, 0x00, 0x03, 0x00, 0x49, 0x52, 0x4c, 0x09, 0x00, 0x45, 0x2d, 0x46, 0x72, 0x69, 0x65, 0x6e, 0x64, 0x73 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(integer32List)))
            {
                byte type = reader.ReadByte();

                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                List<dynamic> value = attribute.ReadValue(reader);

                Assert.IsTrue(value[0] == 4153738 && value[1] == 4153739);
            }
        }

        [Test]
        public void TestReadListString()
        {
            byte[] stringList = { 0x04, 0x01, 0x02, 0x00, 
                           0x05, 0x00,
                           0x74, 0x65, 0x73, 0x74, 0x31,
                           0x05, 0x00,
                           0x74, 0x65, 0x73, 0x74, 0x32};

            using (BinaryReader reader = new BinaryReader(new MemoryStream(stringList)))
            {
                byte type = reader.ReadByte();

                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                List<dynamic> value = attribute.ReadValue(reader);

                Assert.IsTrue(value[0] == "test1" && value[1] == "test2");
            }
        }

        [Test]
        public void TestReadByte()
        {
            byte[] byt = { 0x08, 0x05 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(byt)))
            {
                byte type = reader.ReadByte();
                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                var value = attribute.ReadValue(reader);

                Assert.AreEqual(value, 5);
            }
        }

        [Test]
        public void TestReadInt32()
        {
            byte[] int32 = { 0x02, 
                             0x01, 0x01, 0x01, 0x01 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(int32)))
            {
                byte type = reader.ReadByte();
                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                var value = attribute.ReadValue(reader);

                Assert.AreEqual(value, 16843009);
            }
            
        }

        [Test]
        public void TestReadString()
        {
            byte[] str = { 0x01, 0x04, 0x00,
                           0x74, 0x65, 0x73, 0x74 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(str)))
            {
                byte type = reader.ReadByte();
                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                var value = attribute.ReadValue(reader);

                Console.WriteLine(value);

                Assert.AreEqual(value, "test");
            }
        }

        [Test]
        public void TestReadSessionId()
        {
            byte[] sessionId = { 0x03,
                                  0x0c, 0xf0, 0x06, 0x15,
                                  0xe4, 0x48, 0x95, 0xc5, 
                                  0x71, 0x6a, 0x06, 0x1c,
                                  0xcc, 0x46, 0xee, 0xd1 };

            using (BinaryReader reader = new BinaryReader(new MemoryStream(sessionId)))
            {
                byte type = reader.ReadByte();
                var attribute = XFireAttributeFactory.Instance.GetAttribute(type);
                var value = attribute.ReadValue(reader);

                Assert.AreEqual(value, new Guid("1506f00c-48e4-c595-716a-061ccc46eed1"));
            }
        }
    }
}
