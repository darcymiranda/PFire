using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Util
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
