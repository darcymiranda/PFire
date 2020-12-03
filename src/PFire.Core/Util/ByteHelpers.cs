using System;

namespace PFire.Core.Util
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
    }
}
