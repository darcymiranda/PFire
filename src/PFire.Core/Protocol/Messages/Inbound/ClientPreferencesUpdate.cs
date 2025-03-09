using PFire.Core.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class ClientPreferencesUpdate : XFireMessage
    {
        public ClientPreferencesUpdate() : base(XFireMessageType.ClientPreferencesUpdate) {}

        [XMessageField("prefs")]
        public Dictionary<byte, object> Prefs { get; set; }

        public override async Task Process(IXFireClient context)
        {
            //when "checked" in the client: Prefs does not contain the key
            //when "unchecked" in the client: Prefs contains the key the value will be "0"
            
            //Exception key 6: 
            //when "checked" in the client: Prefs contains the key with the value "1"
            //when "unchecked" in the client: Prefs does not contain the key

            context.User.ClientPreferences.GameStatusShowMyFriends = !Prefs.ContainsKey((int)Enums.ClientPreferences.GameStatusShowMyFriends);
            context.User.ClientPreferences.GameStatusShowMyGameServer = !Prefs.ContainsKey((int)Enums.ClientPreferences.GameStatusShowMyGameServer);
            context.User.ClientPreferences.GameStatusShowMyProfile = !Prefs.ContainsKey((int)Enums.ClientPreferences.GameStatusShowMyProfile);
            context.User.ClientPreferences.PlaySoundSendOrReceiveMessage = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundSendOrReceiveMessage);
            context.User.ClientPreferences.PlaySoundReceiveMessageWhileGaming = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundReceiveMessageWhileGaming);
            context.User.ClientPreferences.ChatShowTimestamps = Prefs.ContainsKey((int)Enums.ClientPreferences.ChatShowTimestamps); 
            context.User.ClientPreferences.PlaySoundFriendLogsOnOff = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundFriendLogsOnOff);
            context.User.ClientPreferences.GameStatusShowFriendOfFriends = !Prefs.ContainsKey((int)Enums.ClientPreferences.GameStatusShowFriendOfFriends);
            context.User.ClientPreferences.ShowOfflineFriends = !Prefs.ContainsKey((int)Enums.ClientPreferences.ShowOfflineFriends);
            context.User.ClientPreferences.ShowNicknames = !Prefs.ContainsKey((int)Enums.ClientPreferences.ShowNicknames);
            context.User.ClientPreferences.ShowVoiceChatServerToFriends = !Prefs.ContainsKey((int)Enums.ClientPreferences.ShowVoiceChatServerToFriends);
            context.User.ClientPreferences.ShowWhenTyping = !Prefs.ContainsKey((int)Enums.ClientPreferences.ShowWhenTyping);
            context.User.ClientPreferences.NotificationFriendLogsOnOff = !Prefs.ContainsKey((int)Enums.ClientPreferences.NotificationFriendLogsOnOff);
            context.User.ClientPreferences.NotificationDownloadStartsFinishes = !Prefs.ContainsKey((int)Enums.ClientPreferences.NotificationDownloadStartsFinishes);
            context.User.ClientPreferences.PlaySoundSomeoneJoinsLeaveChatroom = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundSomeoneJoinsLeaveChatroom);
            context.User.ClientPreferences.PlaySoundSendReceiveVoiceChatRequest = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundSendReceiveVoiceChatRequest);
            context.User.ClientPreferences.PlaySoundScreenshotWhileGaming = !Prefs.ContainsKey((int)Enums.ClientPreferences.PlaySoundScreenshotWhileGaming);
            context.User.ClientPreferences.NotificationConnectionStateChanges = !Prefs.ContainsKey((int)Enums.ClientPreferences.NotificationConnectionStateChanges);

            await context.Server.Database.SaveClientPreferences(context.User);
        }
    }
}
