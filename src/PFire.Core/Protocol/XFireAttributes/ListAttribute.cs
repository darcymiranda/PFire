using System;
using System.Collections.Generic;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class ListAttribute : XFireAttribute
    {

        public override byte AttributeTypeId => 0x04;

        public override dynamic ReadValue(BinaryReader reader)
        {
            var listItemType = reader.ReadByte();
            var listLength = reader.ReadInt16();
            var itemAttribute = XFireAttributeFactory.Instance.GetAttribute(listItemType);

            var values = new List<dynamic>();
            for (int i = 0; i < listLength; i++)
            {
                values.Add(itemAttribute.ReadValue(reader));
            }
            return values;
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            var listLength = (short)data.Count;
            var dataType = data.GetType();
            var itemType = dataType.GetGenericArguments()[0];
            var attribute = XFireAttributeFactory.Instance.GetAttribute(itemType);

            attribute.WriteType(writer);
            writer.Write(listLength);

            for(int i = 0; i < listLength; i++)
            {
                attribute.WriteValue(writer, data[i]);
            }
        }

        // TODO: Refactor must be able to generic
        public override Type AttributeType => typeof(List<>);
    }
}
