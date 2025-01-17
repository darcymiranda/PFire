using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClientPreferences : XFireMessage
    {
        public ClientPreferences() : base(XFireMessageType.ClientPreferences) {}

        [XMessageField(0x4c)]
        public Dictionary<byte, string> Preferences { get; set; }

        public override Task Process(IXFireClient context)
        {
            var user = context.User;
            Preferences = new Dictionary<byte, string>
            {
                {1, user.ShowGameStatusToFriends ? "1" : "0"},
                {2, user.ShowGameServerData ? "1" : "0"},
                {3, user.ShowGameDataOnProfile ? "1" : "0"},
                {4, user.PlaySoundOnNewMessages ? "1" : "0"},
                {5, user.PlaySoundsOnNewMessagesInGame ? "1" : "0"},
                {6, user.ShowTimeStampInChat ? "1" : "0"},
                {7, user.PlaySoundsOnLogOn ? "1" : "0"},
                {8, user.ShowFriendsOfFriends ? "1" : "0"},
                {9, user.ShowOfflineFriends ? "1" : "0"},
                {10, user.ShowNicknames ? "1" : "0"},
                {11, user.ShowVoiceChatServer ? "1" : "0"},
                {12, user.ShowTyping? "1" : "0"},
                {16, user.ShowTooltipOnLogOn ? "1" : "0"},
                {17, user.ShowTooltipOnDownload ? "1" : "0"},
                {18, user.PlaySoundInChatrooms ? "1" : "0"},
                {19, user.PlaySoundOnVoicecalls ? "1" : "0"},
                {20, user.PlaySoundOnScreenshots ? "1" : "0"},
                {21, "0"}
            };

            return Task.CompletedTask;
        }
    }
}
