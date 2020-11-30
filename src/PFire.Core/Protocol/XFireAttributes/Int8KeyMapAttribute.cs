namespace PFire.Core.Protocol.XFireAttributes
{
    public class Int8KeyMapAttribute : MapAttribute<byte>
    {
        public override byte AttributeTypeId
        {
            get { return 0x09; }
        }
    }
}
