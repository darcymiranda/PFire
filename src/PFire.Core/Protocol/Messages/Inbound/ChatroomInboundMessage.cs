using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages.Bidirectional;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using PFire.Infrastructure.Entities;

namespace PFire.Core.Protocol.Messages.Inbound
{
    enum Accessibility
    {
        publicChat = 1,
        friendsOnly = 2
    }

    enum Response
    {
        OK = 0,
        TooManyRooms = 1,
        Fail = 2, //This just simply didn't do anything, silent fail?
        UnkResponse = 3,
        PasswordRequired = 4,
        WrongPassword = 5
    }

    internal sealed class ChatroomInboundMessage : XFireMessage
    {
        public ChatroomInboundMessage() : base(XFireMessageType.ChatroomInbound) { }

        [XMessageField("climsg")]
        public int CliMsg { get; set; }
        [XMessageField("msg")]
        public Dictionary<byte, object> MsgList { get; set; }

        public override async Task Process(IXFireClient context)
        {
            switch(CliMsg)
            {
                case 0x4cf4: await JoinOrCreateAsync(context); break;
                case 0x4cf5: await LeaveRoom(context); break;
                case 0x4cf6: await ChatroomMsg(context); break;
                case 0x4cf7: await InviteToChatroom(context); break;
                case 0x4cf8: await ChangeTopic(context); break;
                case 0x4cf9: await ChangeUserPermissions(context); break;
                case 0x4cfa: await ChatroomList(context); break;
                case 0x4cfb: await KickUser(context); break;
                case 0x4cfd: await SaveRoomToggle(context); break;
                case 0x4cfe: await ChangeVoiceSetting(context); break;
                case 0x4cff: await InvitationDenied(context); break;
                case 0x4d03: await HostGameLobby(context); break;
                case 0x4d06: await ChatroomInfo(context); break;
                case 0x4d08: await DefaultPermsChanged(context); break;
                case 0x4d09: await GameLobbyUserJoined(context); break;
                case 0x4d0a: await GameLobbyUserLeft(context); break;
                case 0x4d0c: await ChangeMOTD(context); break;
                case 0x4d0f: await GameLobbyLaunch(context); break;
                case 0x4d14: await CheckTopicNameAvailability(context); break;
                case 0x4d15: await PasswordSet(context); break;
                case 0x4d16: await SetVisibility(context); break;
                case 0x4d17: await SilenceToggle(context); break;
                case 0x4d18: await ShowJoinLeaveMessagesToggle(context); break;
            }
        }

        private async Task InvitationDenied(IXFireClient context)
        {
            var Cid = (byte[])MsgList.FirstOrDefault(kv => kv.Key == 0x04).Value;
            await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatRoomInvitationDenied(Cid)); //TODO: Does this need to be the admins strictly? Does this even do anything?
        }

        private async Task InviteToChatroom(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x11, 0x47 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x11 && MsgList[key] is Guid?) ||
                (key == 0x47 && MsgList[key] is int?))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var TargetSession = (Guid)MsgList[0x11];
            var Unk47 = (int)MsgList[0x47];

            var Target = context.Server.GetSession(TargetSession);
            string chatTopic = await context.Server.Database.GetChatroomName(Cid);

            if (Target != null && chatTopic != null)
            {
                await Target.SendMessage(new ChatroomInvitationSent(Cid, chatTopic, context.User.Id, context.User.Username, Unk47));
            }
        }

        private async Task DefaultPermsChanged(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x13 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x13 && MsgList[key] is int))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var Perms = (int)MsgList[0x13];

            await context.Server.Database.UpdateChatroomDefaultPerms(Cid, Perms);
            await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomDefaultPermsChanged(Cid, Perms));
        }

        private Task ChangeVoiceSetting(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x34, 0xA8, 0x37, 0x41 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x34 && MsgList[key] is int) ||
                (key == 0xA8 && MsgList[key] is byte) ||
                (key == 0x37 && MsgList[key] is int) ||
                (key == 0x41 && MsgList[key] is int))))
            {
                return Task.CompletedTask;
            }

            var Cid = (byte[])MsgList[0x04];
            var HostType = (int)MsgList[0x34];
            var VoiceEnabled = (byte)MsgList[0xA8];
            var VoiceBandwidth = (int)MsgList[0x37];
            var VoiceChatterLimit = (int)MsgList[0x41];
            return Task.CompletedTask;
        }

        private async Task GameLobbyUserLeft(IXFireClient context)
        {
            byte requiredKey = 0x04;

            if (!MsgList.TryGetValue(requiredKey, out var value) || value is not byte[] Cid)
            {
                return;
            }

            await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomGameLobbyUserLeft(Cid, context.User.Id));

        }

        private async Task GameLobbyUserJoined(IXFireClient context)
        {
            byte requiredKey = 0x04;

            if (!MsgList.TryGetValue(requiredKey, out var value) || value is not byte[] Cid)
            {
                return;
            }

            await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomGameLobbyUserJoined(Cid, context.User.Id));
        }

        private async Task GameLobbyLaunch(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x5A };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x5A && MsgList[key] is int))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var LaunchStatus = (int)MsgList[0x5A];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                var users = await context.Server.Database.QueryGameLobbyUsersInChat(Cid);
                foreach (var user in users)
                {
                    var friendClient = context.Server.GetSession(user);
                    if (friendClient != null)
                    {
                        await friendClient.SendAndProcessMessage(new ChatroomGameLobbyLaunch(Cid, LaunchStatus));
                    }
                }
            }
        }

        private async Task HostGameLobby(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x21, 0x22, 0x23 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x21 && MsgList[key] is int) ||
                (key == 0x22 && MsgList[key] is int) ||
                (key == 0x23 && MsgList[key] is int))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var GameId = (int)MsgList[0x21];
            var GameIp = (int)MsgList[0x22];
            var GamePort = (int)MsgList[0x23];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                await context.Server.Database.StartChatroomGameLobby(Cid, GameId, GameIp, GamePort, [context.User.Id]);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomGameLobbyInfo(Cid, GameId, context.User.Id, GameIp, GamePort, [context.User.Id]));
            }
        }

        private async Task PasswordSet(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x5F };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x5F && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var Password = (string)MsgList[0x5F];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                await context.Server.Database.UpdateChatroomPassword(Cid, Password);
                byte IsPasswordEnabled = (byte)(Password.Length > 0 ? 1 : 0);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomPasswordChanged(Cid, IsPasswordEnabled));
            }
        }

        private async Task SetVisibility(IXFireClient context)
        {
            var Cid = (byte[])MsgList.FirstOrDefault(kv => kv.Key == 0x04).Value;
            var Visibility = (int)MsgList.FirstOrDefault(kv => kv.Key == 0x17).Value;

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel == 5)
            {
                await context.Server.Database.UpdateChatroomVisibility(Cid, Visibility);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomVisibilityChanged(Cid, Visibility));
            }
        }

        private async Task ShowJoinLeaveMessagesToggle(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x1B };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x1B && MsgList[key] is byte))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var NotificationToggle = (byte)MsgList[0x1B];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                await context.Server.Database.UpdateChatroomJoinNotification(Cid, NotificationToggle);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomJoinNotificationChanged(Cid, NotificationToggle));
            }
        }

        private async Task SilenceToggle(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x16 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x16 && MsgList[key] is byte))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var SilenceToggle = (byte)MsgList[0x16];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                await context.Server.Database.UpdateChatroomSilenceStatus(Cid, SilenceToggle);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomSilenceChanged(Cid, context.User.Id, SilenceToggle));
            }
        }

        private async Task SaveRoomToggle(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x2A };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x2A && MsgList[key] is byte))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var SaveToggle = (byte)MsgList[0x2A];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel == 5)
            {
                await context.Server.Database.UpdateChatroomSaveStatus(Cid, SaveToggle);
            }
        }

        private async Task ChangeMOTD(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x2E };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x2E && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var motd = (string)MsgList[0x2E];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 2)
            {
                await context.Server.Database.UpdateChatroomMOTD(Cid, motd);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomMOTDChanged(Cid, motd));
            }
        }

        private async Task KickUser(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x18 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x18 && MsgList[key] is int))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var Uid = (int)MsgList[0x18];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 3)
            {
                var users = await context.Server.Database.QueryUsersInChat(Cid);
                await context.Server.Database.KickChatroomUser(Cid, Uid);
                foreach (var user in users)
                {
                    var friendClient = context.Server.GetSession(user);
                    if (friendClient != null)
                    {
                        await friendClient.SendAndProcessMessage(new ChatroomUserKicked(Cid, Uid));
                        await friendClient.SendAndProcessMessage(new ChatroomUserLeft(Cid, Uid));
                    }
                }
            }
        }

        private async Task ChangeUserPermissions(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x18, 0x13 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x18 && MsgList[key] is int) ||
                (key == 0x13 && MsgList[key] is int))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var UserId = (int)MsgList[0x18];
            var Permissions = (int)MsgList[0x13];

            var UserPowerLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            if (UserPowerLevel > 2)
            {
                await context.Server.Database.UpdateChatroomUserPerms(Cid, UserId, Permissions);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomUserPermsChanged(Cid, UserId, Permissions));
            }
        }

        private async Task CheckTopicNameAvailability(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x0B, 0x05 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x0B && MsgList[key] is int) ||
                (key == 0x05 && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var requestId = (int)MsgList[0x0B];
            var newName = (string)MsgList[0x05];

            if (context.Server.Database.QueryChatroom(newName) == null)
            {
                await context.SendMessage(new ChatroomNameAvailability(requestId, 1));
            }
            else
            {
                await context.SendMessage(new ChatroomNameAvailability(requestId, 0)); //Name already exists
            }
        }

        private async Task ChangeTopic(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x05 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x05 && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var newName = (string)MsgList[0x05];

            if (Cid != null && newName != null)
            {
                await context.Server.Database.UpdateChatroomName(Cid, newName);
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomNameChanged(Cid, newName));
            }
        }

        private async Task LeaveRoom(IXFireClient context)
        {
            byte requiredKey = 0x04;

            if (!MsgList.ContainsKey(requiredKey) || !(MsgList[requiredKey] is byte[] Cid))
            {
                return;
            }

            var UserLeftResult = await context.Server.Database.UserLeftRoom(Cid, context.User.Id);

            if (Cid != null && UserLeftResult == 0)
            {
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomUserLeft(Cid, context.User.Id));
            }
        }

        private async Task ChatroomList(IXFireClient context)
        {
            await context.SendAndProcessMessage(new ChatroomInfoOnLogin());
        }

        private async Task ChatroomInfo(IXFireClient context)
        {
            byte requiredKey = 0x04;

            if (!MsgList.ContainsKey(requiredKey) || !(MsgList[requiredKey] is byte[] Cid))
            {
                return;
            }

            ChatroomModel workingChatPayload = await context.Server.Database.QueryChatroom(Cid);

            if (workingChatPayload != null)
            {
                await context.SendAndProcessMessage(new ChatRoomFriendList(workingChatPayload));
            }

        }

        private async Task ChatroomMsg(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 0x2E };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 0x2E && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var Message = (string)MsgList[0x2E];

            var UserLevel = await context.Server.Database.GetUserHighestChatPowerLevel(Cid, context.User.Id);
            var IsRoomSilenced = await context.Server.Database.IsRoomSilenced(Cid);

            if ((IsRoomSilenced && UserLevel > 3) || (!IsRoomSilenced))
            {
                await SendAndProcessMessageToAllChatUsers(context, Cid, new ChatroomUserMessage(Cid, context.User.Id, Message));
            }
        }

        private async Task JoinOrCreateAsync(IXFireClient context)
        {
            var requiredKeys = new byte[] { 0x04, 167, 0x0B, 0x5F, 0x05 };

            if (!requiredKeys.All(key => MsgList.ContainsKey(key) &&
                ((key == 0x04 && MsgList[key] is byte[]) ||
                (key == 167 && MsgList[key] is byte) ||
                (key == 0x0B && MsgList[key] is int) ||
                (key == 0x5F && MsgList[key] is string) ||
                (key == 0x05 && MsgList[key] is string))))
            {
                return;
            }

            var Cid = (byte[])MsgList[0x04];
            var isNewChatroom = (byte)MsgList[167];
            var requestId = (int)MsgList[0x0B];
            var inputtedPassword = (string)MsgList[0x5F];
            var chatName = (string)MsgList[0x05];
            ChatroomModel workingChatPayload = new ChatroomModel();

            //We dont know the chatid right now (user inputted the chat room name and hit enter) or created a room via Create Room
            if (IsCidEmpty(Cid)) 
            {
                if (chatName == null)
                    return;

                var existingChatroom = await context.Server.Database.QueryChatroom(chatName);
                if (existingChatroom == null || (isNewChatroom == 1 && existingChatroom != null))
                {
                    if (isNewChatroom == 1 && existingChatroom != null)
                    {
                        //If the room name exists and you're autonaming, then add a _ for now.
                        chatName += "_";
                    }

                    isNewChatroom = 1; //This is a new room now.
                    workingChatPayload.Name = chatName;
                    workingChatPayload.MOTD = "";
                    workingChatPayload.Visibility = (int)Accessibility.friendsOnly;
                    new Random().NextBytes(workingChatPayload.Id);
                    workingChatPayload.Administrators.Add(context.User.Id);
                    workingChatPayload.Users.Add(context.User.Id);
                    await context.Server.Database.InsertChatroom(workingChatPayload);
                }
                else
                {
                    workingChatPayload = existingChatroom;
                }

                await context.SendAndProcessMessage(new ChatRoomJoinInfo(workingChatPayload, requestId, (int)Response.OK, isNewChatroom == 1, inputtedPassword));
            }
            else
            {
                var workingChat = await context.Server.Database.QueryChatroom(Cid);
                if (workingChat != null)
                {
                    workingChatPayload = workingChat;
                    await context.SendAndProcessMessage(new ChatRoomJoinInfo(workingChatPayload, requestId, (int)Response.OK, false, inputtedPassword));
                }
                else
                {
                    await context.SendAndProcessMessage(new ChatRoomJoinInfo(workingChatPayload, requestId, (int)Response.Fail));
                }
            }
        }


        private static async Task SendAndProcessMessageToAllChatUsers(IXFireClient context, byte[] Cid, IMessage message)
        {
            var users = await context.Server.Database.QueryUsersInChat(Cid);
            foreach (var user in users)
            {
                var friendClient = context.Server.GetSession(user);
                if (friendClient != null)
                {
                    await friendClient.SendAndProcessMessage(message);
                }
            }
        }

        private static bool IsCidEmpty(byte[] byteArray)
        {
            foreach (byte b in byteArray)
            {
                if (b != 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
