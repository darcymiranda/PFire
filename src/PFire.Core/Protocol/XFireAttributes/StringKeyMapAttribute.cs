namespace PFire.Core.Protocol.XFireAttributes
{
    public class StringKeyMapAttribute : MapAttribute<string>
    {
        public override byte AttributeTypeId
        {
            get { return 0x5; }
        }
    }
}
