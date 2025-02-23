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
            //context.User.ShowGameStatusToFriends = !Prefs.ContainsKey(1);
            //context.User.ShowGameServerData = !Prefs.ContainsKey(2);
            //context.User.ShowGameDataOnProfile = !Prefs.ContainsKey(3);
            //context.User.PlaySoundOnNewMessages = !Prefs.ContainsKey(4);
            //context.User.PlaySoundsOnNewMessagesInGame = !Prefs.ContainsKey(5);
            //context.User.ShowTimeStampInChat = Prefs.ContainsKey(6); //Per Zelaron's observations, this one has flipped true/false logic.
            //context.User.PlaySoundsOnLogOn = !Prefs.ContainsKey(7);
            //context.User.ShowFriendsOfFriends = !Prefs.ContainsKey(8);
            //context.User.ShowOfflineFriends = !Prefs.ContainsKey(9);
            //context.User.ShowNicknames = !Prefs.ContainsKey(10);
            //context.User.ShowVoiceChatServer = !Prefs.ContainsKey(11);
            //context.User.ShowTyping = !Prefs.ContainsKey(12);
            //context.User.ShowTooltipOnLogOn = !Prefs.ContainsKey(16);
            //context.User.ShowTooltipOnDownload = !Prefs.ContainsKey(17);
            //context.User.PlaySoundInChatrooms = !Prefs.ContainsKey(18);
            //context.User.PlaySoundOnVoicecalls = !Prefs.ContainsKey(19);
            //context.User.PlaySoundOnScreenshots = !Prefs.ContainsKey(20);

            //await context.Server.Database.SaveUserPrefs(context.User);
        }
    }
}
