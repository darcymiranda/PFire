using System;
using System.Collections.Generic;
using System.IO;
using PFire.Core.Protocol;
using PFire.Core.Protocol.XFireAttributes;
using PFire.Core.Util;
using Xunit;

namespace PFire.Tests.PFire.Core.Protocol
{
    public class XFireAttributeFactoryTests
    {
        [Fact]
        public void ReadValue_ReadsByte()
        {
            byte[] bytes =
            {
                0x08, 0x05
            };

            const byte expected = 5;

            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsInt32()
        {
            byte[] bytes =
            {
                0x02, 0x01, 0x01, 0x01, 0x01
            };

            const int expected = 16843009;
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsInt8KeyMap()
        {
            byte[] bytes =
            {
                0x09, 0x02, 0x01, 0x01, 0x01, 0x00, 0x30, 0x02, 0x01, 0x01, 0x00, 0x31
            };

            var expected = new Dictionary<byte, dynamic>
            {
                {
                    0x01, "0"
                },
                {
                    0x02, "1"
                }
            };
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsListInt32()
        {
            byte[] bytes =
            {
                0x04, 0x02, 0x02, 0x00, 0x8a, 0x61, 0x3f, 0x00, 0x8b, 0x61, 0x3f, 0x00
            };

            var expected = new List<dynamic>
            {
                4153738,
                4153739
            };
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsListString()
        {
            byte[] bytes =
            {
                0x04, 0x01, 0x02, 0x00, 0x05, 0x00, 0x74, 0x65, 0x73, 0x74, 0x31, 0x05, 0x00, 0x74, 0x65, 0x73, 0x74, 0x32
            };

            var expected = new List<dynamic>
            {
                "test1",
                "test2"
            };
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsMapMessage()
        {
            byte[] bytes =
            {
                0x05, 0x03, 0x07, 0x6d, 0x73, 0x67, 0x74, 0x79, 0x70, 0x65, 0x02, 0x00, 0x00, 0x00, 0x00, 0x07, 0x69, 0x6d, 0x69, 0x6e, 0x64, 0x65, 0x78, 0x02, 0x04, 0x00, 0x00, 0x00, 0x02, 0x69, 0x6d, 0x01, 0x05, 0x00, 0x64, 0x75, 0x64, 0x65
            };

            var expected = new Dictionary<string, dynamic>
            {
                {
                    "msgtype", 0
                },
                {
                    "imindex", 4
                },
                {
                    "im", "dude"
                }
            };
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsSessionId()
        {
            byte[] bytes =
            {
                0x03, 0x0c, 0xf0, 0x06, 0x15, 0xe4, 0x48, 0x95, 0xc5, 0x71, 0x6a, 0x06, 0x1c, 0xcc, 0x46, 0xee, 0xd1
            };

            var expected = new Guid("1506f00c-48e4-c595-716a-061ccc46eed1");
            
            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsString()
        {
            byte[] bytes =
            {
                0x01, 0x04, 0x00, 0x74, 0x65, 0x73, 0x74
            };

            const string expected = "test";

            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void ReadValue_ReadsStringKeyMap()
        {
            byte[] bytes =
            {
                0x05, 0x02, 0x04, 0x6b, 0x65, 0x79, 0x31, 0x02, 0x01, 0x00, 0x00, 0x00, 0x04, 0x6b, 0x65, 0x79, 0x32, 0x02, 0x02, 0x00, 0x00, 0x00
            };

            var expected = new Dictionary<string, dynamic>
            {
                {
                    "key1", 1
                },
                {
                    "key2", 2
                }
            };

            AssertReadValue(bytes, expected);
        }

        [Fact]
        public void WriteValue_WritesByte()
        {
            const byte value = 5;
            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x08, 0x05
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesInt32()
        {
            const int value = 3155;
            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x02, 0x53, 0x0c, 0x00, 0x00
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesInt8KeyMap()
        {
            const string name = "test";
            var value = new Dictionary<byte, dynamic>
            {
                {
                    1, "0"
                },
                {
                    2, "1"
                }
            };

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x09, 0x02, 0x01, 0x01, 0x01, 0x00, 0x30, 0x02, 0x01, 0x01, 0x00, 0x31
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesListInt32()
        {
            var value = new List<int>
            {
                3155,
                3155
            };

            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x04, 0x02, 0x02, 0x00, 0x53, 0x0c, 0x00, 0x00, 0x53, 0x0c, 0x00, 0x00
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesListString()
        {
            var value = new List<string>
            {
                "test",
                "test"
            };

            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x04, 0x01, 0x02, 0x00, 0x04, 0x00, 0x74, 0x65, 0x73, 0x74, 0x04, 0x00, 0x74, 0x65, 0x73, 0x74
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesSessionId()
        {
            var value = Guid.NewGuid();
            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var bytes = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x03
            };

            var expected = ByteHelper.CombineByteArray(bytes, value.ToByteArray());

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesString()
        {
            const string value = "test";
            const string name = "test";

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x01, 0x04, 0x00, 0x74, 0x65, 0x73, 0x74
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        [Fact]
        public void WriteValue_WritesStringKeyMap()
        {
            const string name = "test";
            var value = new Dictionary<string, dynamic>
            {
                {
                    "key1", 1
                },
                {
                    "key2", 2
                }
            };

            var attribute = XFireAttributeFactory.Instance.GetAttribute(value.GetType());

            var expected = new byte[]
            {
                0x04, 0x74, 0x65, 0x73, 0x74, 0x05, 0x02, 0x04, 0x6b, 0x65, 0x79, 0x31, 0x02, 0x01, 0x00, 0x00, 0x00, 0x04, 0x6b, 0x65, 0x79, 0x32, 0x02, 0x02, 0x00, 0x00, 0x00
            };

            AssertWrittenValue(attribute, name, value, expected);
        }

        private static void AssertReadValue<T>(byte[] bytes, T expected)
        {
            var attributeFactory = XFireAttributeFactory.Instance;

            using var reader = new BinaryReader(new MemoryStream(bytes));
            var type = reader.ReadByte();
            var attribute = attributeFactory.GetAttribute(type);

            var actual = (T)attribute.ReadValue(reader);
            
            Assert.Equal(expected, actual);
        }

        private void AssertWrittenValue(XFireAttribute attribute, string name, object value, byte[] expected)
        {
            byte[] actual;
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    attribute.WriteAll(writer, name, value);
                }

                actual = ms.ToArray();
            }

            Assert.Equal(expected, actual);
        }
    }
}
