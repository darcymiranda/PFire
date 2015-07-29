using PFire.Session;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages
{
    public interface IMessage
    {
        short MessageTypeId { get; }

        void Process(Context context);
    }
}
