using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Util
{
    public static class ByteHelper
    {
        public static byte[] CombineByteArray(byte[] a1, byte[] a2)
        {
            var combined = new byte[a1.Length + a2.Length];
            Buffer.BlockCopy(a1, 0, combined, 0, a1.Length);
            Buffer.BlockCopy(a2, 0, combined, a1.Length, a2.Length);
            return combined;
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            var shb = new SoapHexBinary(bytes);
            return shb.ToString();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return SoapHexBinary.Parse(hex).Value;
        }
    }
}
