using System.Collections.Generic;

namespace PFire.Core.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public GameModel Game { get; set; } = new GameModel();
        public bool ShowGameStatusToFriends { get; set; }
        public bool ShowGameServerData { get; set; }
        public bool ShowGameDataOnProfile { get; set; }
        public bool ShowTimeStampInChat { get; set; }
        public bool ShowVoiceChatServer { get; set; }
        public bool ShowTyping { get; set; }
        public bool ShowFriendsOfFriends { get; set; }
        public bool PlaySoundOnNewMessages { get; set; }
        public bool PlaySoundsOnNewMessagesInGame { get; set; }
        public bool PlaySoundsOnLogOn { get; set; }
        public bool ShowOfflineFriends { get; set; }
        public bool ShowNicknames { get; set; }
        public bool ShowTooltipOnLogOn { get; set; }
        public bool ShowTooltipOnDownload { get; set; }
        public bool PlaySoundInChatrooms { get; set; }
        public bool PlaySoundOnVoicecalls { get; set; }
        public bool PlaySoundOnScreenshots { get; set; }
    }
}
