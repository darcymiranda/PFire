using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PFire.Protocol;
using PFire.Util;

namespace PFireTest
{
    public class AttributeWriters
    {
        [Test]
        public void TestWriteStringKeyMap()
        {
            var name = "test";
            var value = new Dictionary<string, dynamic>()
            {
                {"key1", 1},
                {"key2", 2}
            };
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                ConsoleByteOut(ms.ToArray());

                var expected = new byte[] { 0x04, 0x74, 0x65, 0x73, 0x74, 
                                            0x05, 0x02, 0x04, 0x6b, 0x65, 0x79, 0x31, 0x02, 0x01, 0x00, 0x00, 0x00,
                                                        0x04, 0x6b, 0x65, 0x79, 0x32, 0x02, 0x02, 0x00, 0x00, 0x00 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteInt8KeyMap()
        {
            var name = "test";
            var value = new Dictionary<byte, dynamic>()
            {
                {1, "0"},
                {2, "1"}
            };
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[] { 0x04, 0x74, 0x65, 0x73, 0x74, 0x09, 0x02, 0x01, 0x01, 0x01, 0x00, 0x030,
                                                                                      0x02, 0x01, 0x01, 0x00, 0x31 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteByte()
        {
            byte value = 5;
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[]{ 0x04, 0x74, 0x65, 0x73, 0x74, 0x08,
                                           0x05 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteInt32()
        {
            int value = 3155;
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[]{ 0x04, 0x74, 0x65, 0x73, 0x74, 0x02,
                                           0x53, 0x0c, 0x00, 0x00 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteSessionId()
        {
            Guid value = new Guid();
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = ByteHelper.CombineByteArray(new byte[]{ 0x04, 0x74, 0x65, 0x73, 0x74, 0x03 }, value.ToByteArray());
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteString()
        {
            string value = "test";
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[] { 0x04, 0x74, 0x65, 0x73, 0x74, 0x01, 
                                            0x04, 0x00, 0x74, 0x65, 0x73, 0x74 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteListInt32()
        {
            List<int> value = new List<int>() { 3155, 3155 };
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[] { 0x04, 0x74, 0x65, 0x73, 0x74, 0x04, 0x02, 0x02, 0x00, 
                                            0x53, 0x0c, 0x00, 0x00, 0x53, 0x0c, 0x00, 0x00 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        [Test]
        public void TestWriteListString()
        {
            List<string> value = new List<string>() { "test", "test" };
            var name = "test";
            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                var expected = new byte[] { 0x04, 0x74, 0x65, 0x73, 0x74, 0x04, 0x01, 0x02, 0x00, 
                                            0x04, 0x00, 0x74, 0x65, 0x73, 0x74, 0x04, 0x00, 0x74, 0x65, 0x73, 0x74 };
                Assert.IsTrue(ms.ToArray().SequenceEqual(expected));
            }
        }

        private void ConsoleByteOut(byte[] data)
        {
            Console.WriteLine(BitConverter.ToString(data));
        }
    }
}

