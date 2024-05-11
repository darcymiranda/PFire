using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    enum ChatResponse
    {
        OK = 0,
        TooManyRooms = 1,
        Fail = 2, //This just simply didn't do anything, silent fail?
        UnkResponse = 3,
        PasswordRequired = 4,
        WrongPassword = 5
    }
    enum Permissions
    {
        Muted = 1,
        Normal = 2,
        PowerUser = 3,
        Moderator = 4,
        Admin = 5
    }
    internal sealed class ChatRoomJoinInfo : XFireMessage
    {
        private ChatroomModel _chatroomPayload = new ChatroomModel();
        private bool _isNewRoom;
        private int _requestId;
        private string _password;
        public ChatRoomJoinInfo(ChatroomModel chat, int requestId, int response, bool isNewRoom = false, string password = null) : base(XFireMessageType.ChatRoomJoinInfo)
        {
            ChatId = new byte[21];
            _chatroomPayload = chat;
            _isNewRoom = isNewRoom;
            _requestId = requestId;
            Response = response;
            _password = password;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x0b)]
        public int RequestId { get; set; }
        [XMessageField(0x0c)]
        public int Response { get; set; }
        [XMessageField(0x12)]
        public int ResponsePermission { get; set; }
        [XMessageField(0x17)] 
        public int Accessibility { get; set; }
        [XMessageField(0xaa)]
        public int ChatroomType { get; set; }
        [XMessageField(0x05)]
        public string ChatName { get; set; }
        [XMessageField(0x4d)]
        public string MOTD { get; set; }
        [XMessageField(0xa5)]
        public byte IsNewRoom { get; set; }
        [XMessageField(0xa6)]
        public byte IsPassworded { get; set; }
        [XMessageField(0x16)]
        public byte IsSilenced { get; set; }
        [XMessageField(0x1b)]
        public byte ShowJoinLeaveMessages { get; set; }

        public override async Task Process(IXFireClient context)
        {
            ChatId = _chatroomPayload.Id;
            ChatName = _chatroomPayload.Name;
            RequestId = _requestId;
            Accessibility = _chatroomPayload.Visibility;
            MOTD = _chatroomPayload.MOTD;
            IsNewRoom = Convert.ToByte(_isNewRoom);
            ShowJoinLeaveMessages = _chatroomPayload.ShowJoinLeaveMessages;
            ChatroomType = 1; //TODO: Function once broadcast packets are done.

            if (Response == (int)ChatResponse.Fail) //Are we already invalid? Then silently fail.
            {
                IsSilenced = 0;
                IsPassworded = 0;
                ResponsePermission = 0;
            }
            else if (_isNewRoom)
            {
                ResponsePermission = (int)Permissions.Admin;
                IsSilenced = 0;
                IsPassworded = 0;
            }
            else
            {
                var userLevel = await context.Server.Database.GetUserHighestChatPowerLevel(_chatroomPayload.Id, context.User.Id);

                //Check to see if Default Permissions are set and see if the user has a non-standard permission already.
                if (_chatroomPayload.DefaultPerms != 2 && userLevel == (int)Permissions.Normal)
                {
                    await context.Server.Database.UpdateChatroomUserPerms(_chatroomPayload.Id, context.User.Id, _chatroomPayload.DefaultPerms);
                    ResponsePermission = _chatroomPayload.DefaultPerms;
                }
                else
                {
                    ResponsePermission = userLevel;
                }

                IsSilenced = _chatroomPayload.Silenced;

                IsPassworded = (byte)(!string.IsNullOrEmpty(_chatroomPayload.Password) ? 1 : 0);
                if (IsPassworded == 1 && string.IsNullOrEmpty(_password))
                {
                    Response = (int)ChatResponse.PasswordRequired; //Password is needed
                } 
                else if (IsPassworded == 1 && !string.IsNullOrEmpty(_password))
                {
                    Response = (byte)(_password == _chatroomPayload.Password ? (int)ChatResponse.OK : (int)ChatResponse.WrongPassword); //Wrong Password, try again.
                }
            }

            //We all good? Send it to everyone that we have entered.
            if (Response == (int)ChatResponse.OK)
            {
                await context.Server.Database.UserEnteredRoom(ChatId, context.User.Id);
                
                if (_chatroomPayload.GameLobbyID != 0)
                {
                    await context.SendAndProcessMessage(new ChatroomGameLobbyInfo(ChatId, _chatroomPayload.GameLobbyID, _chatroomPayload.GameLobbyHost, _chatroomPayload.GameLobbyIP, _chatroomPayload.GameLobbyPort, _chatroomPayload.GameLobbyPlayers));
                }

                var perms = await context.Server.Database.GetUserHighestChatPowerLevel(ChatId, context.User.Id);
                var users = await context.Server.Database.QueryUsersInChat(ChatId);
                foreach (var user in users)
                {
                    var friendClient = context.Server.GetSession(user);
                    if (friendClient != null)
                    {
                        await friendClient.SendAndProcessMessage(new ChatroomUserJoined(ChatId, context.User, perms));
                    }
                }
            }
        }
    }
}
