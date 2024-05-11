using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal class LoggedInElseWhere : XFireMessage
    {
        public LoggedInElseWhere(int reason, int minrect, int maxrect) : base(XFireMessageType.LoggedInElseWhere) {
            Reason = reason;
            MinRect = minrect;
            MaxRect = maxrect;
        }

        /*
         * Reason is interesting because this can be any other reason other than being logged out.
         * 0 = Lost Connection to Server
         * 1 = Logged in Elsewhere
         * 2 = Xfire system is being restarted for maintenance
         * 3 = Xfire system is being restarted for upgrade
         */

        [XMessageField("reason")]
        public int Reason { get; set; }

        //These two have to deal with how many seconds before the client tries to autolog back in.
        [XMessageField("minrect")]
        public int MinRect { get; set; }

        [XMessageField("maxrect")]
        public int MaxRect { get; set; }
    }
}
