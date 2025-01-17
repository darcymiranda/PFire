
using System.Text;
using System;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendInviteResponse : XFireMessage
    {
        public FriendInviteResponse(string nick, int result) : base(XFireMessageType.FriendInviteResponse)
        {
            Username = nick;
            Result = result;
        }

        [XMessageField("name")]
        public string Username { get; set; }

        /*
         * Result of invite:
         * 0 = OK
         * 1 = No such user found.
         * 2 = You are already friends with them.
         * 3 = Your invitation message for %s has been updated.
         * 4 = You have too many invitations and friends.  %s was not invited to be your friend.
         * 5 = %s has too many invitations and friends.  They were not invited to be your friend.
         * 6 = Your invitation to add %s as a friend was unable to be sent due to a system error.
         * 
         */
        [XMessageField("result")]
        public int Result { get; set; }
    }
}
