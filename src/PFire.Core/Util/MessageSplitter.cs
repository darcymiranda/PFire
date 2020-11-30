using System;
using System.Diagnostics;
using System.IO;

namespace PFire.Core.Util
{
    public static class DebugTools
    {
        public static void DebugMessages(byte[] data)
        {


            using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
            {
                var header = reader.ReadInt16();
                var message = reader.ReadBytes(header - 2);
                Debug.WriteLine("DebugTools.SplitMessages - Message[{0}]: {1}", 
                    BitConverter.ToInt16(message, 0),
                    BitConverter.ToString(message));

                if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    DebugMessages(reader.ReadBytes(data.Length));
                }
            }
        }
    }
}
