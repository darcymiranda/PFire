namespace PFire.Core.Models
{
    public class ClientPreferencesModel
    {
        public bool GameStatusShowMyFriends { get; set; }
        public bool GameStatusShowMyGameServer { get; set; }
        public bool GameStatusShowMyProfile { get; set; }
        public bool ChatShowTimestamps { get; set; }
        public bool ShowVoiceChatServerToFriends { get; set; }
        public bool ShowWhenTyping { get; set; }
        public bool GameStatusShowFriendOfFriends { get; set; }
        public bool PlaySoundSendOrReceiveMessage { get; set; }
        public bool PlaySoundReceiveMessageWhileGaming { get; set; }
        public bool PlaySoundFriendLogsOnOff { get; set; }
        public bool ShowOfflineFriends { get; set; }
        public bool ShowNicknames { get; set; }
        public bool NotificationFriendLogsOnOff { get; set; }
        public bool NotificationDownloadStartsFinishes { get; set; }
        public bool PlaySoundSomeoneJoinsLeaveChatroom { get; set; }
        public bool PlaySoundSendReceiveVoiceChatRequest { get; set; }
        public bool PlaySoundScreenshotWhileGaming { get; set; }
        public bool NotificationConnectionStateChanges { get; set; }
    }
}
