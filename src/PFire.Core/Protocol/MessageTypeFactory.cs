﻿using System;
using System.Collections.Generic;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Bidirectional;
using PFire.Core.Protocol.Messages.Inbound;
using PFire.Core.Protocol.Messages.Outbound;

namespace PFire.Core.Protocol
{
    public class MessageTypeFactory
    {
        private static readonly MessageTypeFactory instance = null;

        private readonly Dictionary<XFireMessageType, IMessage> _messages = new Dictionary<XFireMessageType, IMessage>();

        private MessageTypeFactory()
        {
            Add(new ClientVersion());
            Add(new LoginRequest());
            Add(new LoginFailure());
            Add(new LoginSuccess());
            Add(new ClientConfiguration());
            Add(new Unknown10());
            Add(new ConnectionInformation());
            Add(new Groups());
            Add(new GroupsFriends());
            Add(new ServerList());
            Add(new ChatRooms());
            Add(new GameInformation());
            Add(new KeepAlive());
            Add(new Did());
            Add(new ChatMessage());
            Add(new UserLookup());
            Add(new FriendRequest());
            Add(new FriendRequestAccept());
            Add(new FriendRequestDecline());
            Add(new NicknameChange());
            Add(new StatusChange());
        }

        private void Add(IMessage message)
        {
            _messages.Add(message.MessageTypeId, message);
        }

        public Type GetMessageType(XFireMessageType messageType)
        {
            // Hack: Client sends message type of 2 for chat messages but expects message type of 133 on receive...
            // this is because the client to client message (type 2) is send via UDP to the clients directly,
            // whereas 133 is a message routed via the server to the client
            if(messageType == XFireMessageType.UDPChatMessage)
            {
                return _messages[XFireMessageType.ServerChatMessage].GetType();
            }

            if(!_messages.TryGetValue(messageType, out var message))
            {
                throw new UnknownMessageTypeException(messageType);
            }

            return message.GetType();
        }

        public static MessageTypeFactory Instance => instance ?? new MessageTypeFactory();
    }
}
