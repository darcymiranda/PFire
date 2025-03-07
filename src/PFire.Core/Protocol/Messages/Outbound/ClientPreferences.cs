using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClientPreferences : XFireMessage
    {
        public ClientPreferences() : base(XFireMessageType.ClientPreferences) {}

        [XMessageField(0x4c)]
        public Dictionary<byte, string> preferences { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var clientPreferences = await context.Server.Database.GetClientPreferences(context.User);

            preferences = new Dictionary<byte, string>
            {
                { (int)Enums.ClientPreferences.GameStatusShowMyFriends, clientPreferences.GameStatusShowMyFriends ? "1": "0" },
                { (int)Enums.ClientPreferences.GameStatusShowMyGameServer, clientPreferences.GameStatusShowMyGameServer ? "1": "0" },
                { (int)Enums.ClientPreferences.GameStatusShowMyProfile, clientPreferences.GameStatusShowMyProfile ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundSendOrReceiveMessage, clientPreferences.PlaySoundSendOrReceiveMessage ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundReceiveMessageWhileGaming, clientPreferences.PlaySoundReceiveMessageWhileGaming ? "1": "0" },
                { (int)Enums.ClientPreferences.ChatShowTimestamps, clientPreferences.ChatShowTimestamps ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundFriendLogsOnOff, clientPreferences.PlaySoundFriendLogsOnOff ? "1": "0" },
                { (int)Enums.ClientPreferences.GameStatusShowFriendOfFriends, clientPreferences.GameStatusShowFriendOfFriends ? "1": "0" },
                { (int)Enums.ClientPreferences.ShowOfflineFriends, clientPreferences.ShowOfflineFriends ? "1": "0" },
                { (int)Enums.ClientPreferences.ShowNicknames, clientPreferences.ShowNicknames ? "1": "0" },
                { (int)Enums.ClientPreferences.ShowVoiceChatServerToFriends, clientPreferences.ShowVoiceChatServerToFriends ? "1": "0" },
                { (int)Enums.ClientPreferences.ShowWhenTyping, clientPreferences.ShowWhenTyping ? "1": "0" },
                { (int)Enums.ClientPreferences.NotificationFriendLogsOnOff, clientPreferences.NotificationFriendLogsOnOff ? "1": "0" },
                { (int)Enums.ClientPreferences.NotificationDownloadStartsFinishes, clientPreferences.NotificationDownloadStartsFinishes ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundSomeoneJoinsLeaveChatroom, clientPreferences.PlaySoundSomeoneJoinsLeaveChatroom ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundSendReceiveVoiceChatRequest, clientPreferences.PlaySoundSendReceiveVoiceChatRequest ? "1": "0" },
                { (int)Enums.ClientPreferences.PlaySoundScreenshotWhileGaming, clientPreferences.PlaySoundScreenshotWhileGaming ? "1": "0" },
                { (int)Enums.ClientPreferences.NotificationConnectionStateChanges, clientPreferences.NotificationConnectionStateChanges ? "1": "0" }
            };
        }
    }
}
