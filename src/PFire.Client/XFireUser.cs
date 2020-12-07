using PFire;
using PFire.Core.Protocol.Messages;
using PFire.Protocol;
using PFire.Protocol.Messages;
using PFire.Protocol.Messages.Bidirectional;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace PFire.Client
{
    public class XFireUser
    {
        #region Public structs
        public struct FriendInviteMessage
        {
            public string Username;
            public string Nickname;
            public string Message;
        }


        public struct FriendRequestMessage
        {
            public string Username;
            public string Message;
        }


        public struct ListOfFriends
        {
            public string Username;
            public string Nickname;
            public int UserId;
        }


        public struct FriendSessions
        {
            public int UserId;
            public Guid SessionId;
        }


        public struct GameInfo
        {
            public int GameId;
            public int GameIP;
            public int GamePort;
        }


        public struct GroupInfo
        {
            public string GroupName;
            public int GroupId;
        }


        public struct FriendsInGroups
        {
            public int GroupId;
            public int UserId;
        }

        public struct LoginInfo
        {
            public string ClientSet;
            public int Country;
            public string DlSet;
            public int MaxRect;
            public int MinRect;
            public int N1;
            public int N2;
            public int N3;
            public string Nickname;
            public string P2PSet;
            public int PublicIp;
            public string Reason;
            public Guid SessionId;
            public int Status;
            public int UserId;
        }
        #endregion

        #region Private Variables
        private XFireClient _xFireClient;
        private Thread _listenerThread;
        private TcpClient _client;
        #endregion

        #region Public Variables
        public delegate void MessageReceivedHandlerDelegate(object sender, MessageReceivedEventArgs args);
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedHandler;
        #endregion

        #region Public Properties
        /// <summary>
        /// List of chat messages received.
        /// </summary>
        public Dictionary<string, dynamic> ChatMessages { get; private set; }

        /// <summary>
        /// Server chat messages received.
        /// </summary>
        public Dictionary<string, dynamic> ServerChatMessages { get; private set; }

        /// <summary>
        /// The list of chat room Ids.
        /// </summary>
        public List<int> ChatRooms { get; private set; }

        /// <summary>
        /// List of friend invites.
        /// </summary>
        public List<FriendInviteMessage> FriendInvites { get; private set; }

        /// <summary>
        /// List of friend requests.
        /// </summary>
        public List<FriendRequestMessage> FriendRequests { get; private set; }

        /// <summary>
        /// The list of friends for the user.
        /// </summary>
        public List<ListOfFriends> UserFriendsList { get; private set; }

        /// <summary>
        /// List of friends sessions.
        /// </summary>
        public List<FriendSessions> FriendsSessions { get; private set; }

        /// <summary>
        /// Info for games being played.
        /// </summary>
        public List<GameInfo> GameInfos { get; private set; }

        /// <summary>
        /// Info for groups that various friends are a part of.
        /// </summary>
        public List<GroupInfo> GroupsInfo { get; private set; }

        /// <summary>
        /// List of friends in particular groups.
        /// </summary>
        public List<FriendsInGroups> GroupsFriends { get; private set; }

        /// <summary>
        /// The returned login information when a successful login is completed.
        /// </summary>
        public LoginInfo LoginInformation { get; private set; }

        /// <summary>
        /// List of game servers.
        /// </summary>
        public List<GameInfo> ListOfServers { get; private set; }
        #endregion

        #region Constructor
        public XFireUser(string host, int port, string userName, string passWord, PFireServer server)
        {
            LoginUser(host, port, server);
            _xFireClient.User = new PFire.Database.User()
            {
                Username = userName,
                Password = passWord
            };

            this.FriendInvites = new List<FriendInviteMessage>();
            this.FriendRequests = new List<FriendRequestMessage>();
            this.UserFriendsList = new List<ListOfFriends>();
            this.FriendsSessions = new List<FriendSessions>();
            this.GameInfos = new List<GameInfo>();
            this.GroupsInfo = new List<GroupInfo>();
            this.GroupsFriends = new List<FriendsInGroups>();
            this.ListOfServers = new List<GameInfo>();
        }


        public XFireUser(string host, int port, string userName, string passWord)
        {
            LoginUser(host, port, null);
            _xFireClient.User = new PFire.Database.User()
            {
                Username = userName,
                Password = passWord
            };

            this.FriendInvites = new List<FriendInviteMessage>();
            this.FriendRequests = new List<FriendRequestMessage>();
            this.UserFriendsList = new List<ListOfFriends>();
            this.FriendsSessions = new List<FriendSessions>();
            this.GameInfos = new List<GameInfo>();
            this.GroupsInfo = new List<GroupInfo>();
            this.GroupsFriends = new List<FriendsInGroups>();
            this.ListOfServers = new List<GameInfo>();
        }
        #endregion

        #region Public Methods
        public void Connect()
        {
            _listenerThread = new Thread(this.RunListener);
            _listenerThread.Start();
            

            // Initialize the client/server handshake.
            ClientVersion versionInfo = new ClientVersion()
            {
                MajorVersion = 1,
                Version = 155
            };
            var temp = MessageSerializer.Serialize(versionInfo);
            _xFireClient.SendMessage(versionInfo);

            LoginRequest request = new LoginRequest();
            request.Username = _xFireClient.User.Username;
            request.Password = _xFireClient.User.Password;
            _xFireClient.SendMessage(request);

            ClientConfiguration config = new ClientConfiguration()
            {
                Language = "English",
                Partner = "",
                Theme = "",
                Skin = ""
            };
            _xFireClient.SendMessage(config);

            ConnectionInformation connInfo = new ConnectionInformation()
            {
                Connection = 18,
                ClientIp = 2130706433, // Hard-coded af
                Nat = 0,
                NatError = 15
            };
            _xFireClient.SendMessage(connInfo);
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            if (_xFireClient == null)
            {
                return;
            }

            _xFireClient.TcpClient.Close();
        }

        /// <summary>
        /// Sends a friend request to the specified user with a message
        /// </summary>
        /// <param name="user">The user to friend.</param>
        /// <param name="msg">The message to send to the user.</param>
        public void SendFriendRequest(string user, string msg)
        {
            FriendRequest request = new FriendRequest()
            {
                Username = user,
                Message = msg,
            };
            _xFireClient.SendMessage(request);
        }

        /// <summary>
        /// Accepts a given friend request.
        /// </summary>
        /// <param name="request">The FriendRequest to accept.</param>
        public void AcceptFriendRequest(string userName)
        {
            FriendRequestAccept accept = new FriendRequestAccept();
            accept.FriendUsername = userName;
            _xFireClient.SendMessage(accept);
        }

        /// <summary>
        /// Sets the end-server information for the client.
        /// </summary>
        /// <param name="server">A PFireServer instance.</param>
        public void SetServer(PFireServer server)
        {
            _xFireClient.Server = server;
        }
        #endregion

        #region Private Methods
        private void LoginUser(string hostName, int port, PFireServer server)
        {

            _client = new TcpClient();
            _client.Connect(hostName, port);
            _xFireClient = new XFireClient(_client);
            if (server != null)
            {
                _xFireClient.Server = server;
            }
            _xFireClient.InitializeClient();
        }

        private string HashPassword(string username, string pwd)
        {
            string hashedPwd = username + pwd + "UltimateArena";
            byte[] hash = SHA1.HashData(Encoding.ASCII.GetBytes(hashedPwd));
            string pwd_2 = $"0x{hashedPwd:X}" + "4dc383ea21bf4bca83ea5040cb10da62";
            byte[] hash_2 = SHA1.HashData(Encoding.ASCII.GetBytes(pwd_2));
            return Encoding.ASCII.GetString(hash_2);
        }

        private void RunListener()
        {
            Byte[] bytesReceived = new Byte[256];
            string page = String.Empty;
            IMessage message = null;

            while (true)
            {
                int bytes;
                NetworkStream stream = _client.GetStream();
                
                var headerBuffer = new byte[2];
                bytes = stream.Read(headerBuffer, 0, headerBuffer.Length);

                if (bytes != 2)
                {
                    continue;
                }
                
                var messageLength = BitConverter.ToInt16(headerBuffer, 0) - headerBuffer.Length;
                var messageBuffer = new byte[messageLength];
                bytes = stream.Read(bytesReceived, 0, messageLength);

                if (bytes == 0)
                {
                    continue;
                }

                try
                {
                    message = MessageSerializer.Deserialize(bytesReceived);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Conversion of message failed! Message: {0}", Encoding.Default.GetString(bytesReceived));
                    continue;
                }

                switch(message.MessageTypeId)
                {
                    case PFire.Core.Protocol.Messages.XFireMessageType.ChatContent:
                        Console.WriteLine("Chat messages received");

                        if (((ChatMessage)message).MessagePayload == null)
                        {
                            break;
                        }

                        this.ChatMessages = ((ChatMessage)message).MessagePayload;
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ServerChatMessage:
                        Console.WriteLine("Server Chat messages received");

                        if (((ChatMessage)message).MessagePayload == null)
                        {
                            break;
                        }

                        this.ServerChatMessages = ((ChatMessage)message).MessagePayload;
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ChatRooms:
                        Console.WriteLine("Chat rooms message received. Chat Ids:");

                        if (((ChatRooms)message).ChatIds == null)
                        {
                            break;
                        }
                        this.ChatRooms = ((ChatRooms)message).ChatIds;
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ClientConfiguration:
                        Console.WriteLine("Received ClientConfiguration, which shouldn't happen...");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ClientPreferences:
                        Console.WriteLine("Received ClientPreferences, which shouldn't happen...");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ClientVersion:
                        Console.WriteLine("Received ClientVersion, which shouldn't happen...");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ConnectionInformation:
                        Console.WriteLine("Received ConnectionInformation, which shouldn't happen...");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.Did:
                        Console.WriteLine("Received DID Message");
                        Console.WriteLine($"Data: {0}", Encoding.ASCII.GetString(((Did)message).Unknown));
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendInvite:
                        Console.WriteLine("Friend invite received");
                        FriendInvite invite = ((FriendInvite)message);

                        if ((invite.Messages == null) || (invite.Nicknames == null) || (invite.Usernames == null))
                        {
                            break;
                        }

                        for (int i = 0; i<invite.Messages.Count; i++)
                        {
                            this.FriendInvites.Add(new FriendInviteMessage() {
                                Username = invite.Usernames[i],
                                Nickname = invite.Nicknames[i],
                                Message = invite.Messages[i]
                            });
                        }
                            
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendRequest:
                        Console.WriteLine("Received a FriendRequest");
                        FriendRequest request = (FriendRequest)message;
                        FriendRequestMessage msg = new FriendRequestMessage()
                        {
                            Username = request.Username,
                            Message = request.Message
                        };
                        this.FriendRequests.Add(msg);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendRequestAccept:
                        Console.WriteLine("Received a FriendRequestAccept");
                        Console.WriteLine($"Friend: {0}", ((FriendRequestAccept)message).FriendUsername);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendRequestDecline:
                        Console.WriteLine("Received a FriendRequestDecline");
                        Console.WriteLine($"Friend: {0}", ((FriendRequestDecline)message).RequesterUsername);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendsList:
                        Console.WriteLine("Received a FriendsList");
                        FriendsList list = (FriendsList)message;
                        Console.WriteLine("Friends:");

                        if ((list.Nicks == null) || (list.UserIds == null) || (list.Usernames == null))
                        {
                            break;
                        }

                        for(int i=0; i<list.UserIds.Count; i++)
                        {
                            this.UserFriendsList.Add(new ListOfFriends()
                            {
                                UserId = list.UserIds[i],
                                Username = list.Usernames[i],
                                Nickname = list.Nicks[i]
                            });
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendsSessionAssign:
                        Console.WriteLine("Received FriendsSessionAssign");
                        FriendsSessionAssign assign = (FriendsSessionAssign)message;
                        Console.WriteLine($"Unknown value: {0}", assign.Unknown.ToString());

                        if ((assign.UserIds == null) || (assign.SessionIds == null))
                        {
                            break;
                        }

                        for(int i=0; i<assign.UserIds.Count; i++)
                        {
                            this.FriendsSessions.Add(new FriendSessions()
                            {
                                SessionId = assign.SessionIds[i],
                                UserId = assign.UserIds[i]
                            });
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendStatusChange:
                        Console.WriteLine("Received FriendStatusChange");
                        FriendStatusChange statusChange = (FriendStatusChange)message;

                        if((statusChange.SessionIds == null) || (statusChange.Messages == null))
                        {
                            break;
                        }

                        for(int i=0; i<statusChange.SessionIds.Count; i++)
                        {
                            Console.WriteLine($"Session Id: {0}.\tMessage: {1}.",
                                statusChange.SessionIds[i], statusChange.Messages[i]);
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.GameInformation:
                        Console.WriteLine("Received GameInformation");
                        GameInformation info = (GameInformation)message;
                        this.GameInfos.Add(new GameInfo()
                        {
                            GameId = info.GameId,
                            GameIP = info.GameIP,
                            GamePort = info.GamePort
                        });
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.Groups:
                        Console.WriteLine("Received Groups");
                        Groups groups = (Groups)message;

                        if ((groups.GroupIds == null) || (groups.GroupNames == null))
                        {
                            break;
                        }

                        for(int i=0; i<groups.GroupIds.Count; i++)
                        {
                            this.GroupsInfo.Add(new GroupInfo()
                            {
                                GroupName = groups.GroupNames[i],
                                GroupId = groups.GroupIds[i]
                            });
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.GroupsFriends:
                        Console.WriteLine("Received GroupsFriends");
                        GroupsFriends friends = (GroupsFriends)message;

                        if ((friends.GroupIds == null) || (friends.UserIds == null))
                        {
                            break;
                        }

                        for(int i=0; i<friends.GroupIds.Count; i++)
                        {
                            this.GroupsFriends.Add(new FriendsInGroups()
                            {
                                GroupId = friends.GroupIds[i],
                                UserId = friends.UserIds[i]
                            });
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.KeepAlive:
                        Console.WriteLine("Received KeepAlive");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.LoginChallenge:
                        Console.WriteLine("Received LoginChallenge");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.LoginFailure:
                        Console.WriteLine("Received LoginFailure");
                        Console.WriteLine($"Reason: {0}", ((LoginFailure)message).Reason);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.LoginRequest:
                        Console.WriteLine("Received LoginRequest. That shouldn't happen...");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.LoginSuccess:
                        Console.WriteLine("Received LoginSuccess");
                        LoginSuccess loginSuccess = (LoginSuccess)message;
                        this.LoginInformation = new LoginInfo()
                        {
                            ClientSet = loginSuccess.ClientSet,
                            Country = loginSuccess.Country,
                            DlSet = loginSuccess.DlSet,
                            MaxRect = loginSuccess.MaxRect,
                            MinRect = loginSuccess.MinRect,
                            N1 = loginSuccess.N1,
                            N2 = loginSuccess.N2,
                            N3 = loginSuccess.N3,
                            Nickname = loginSuccess.Nickname,
                            P2PSet = loginSuccess.P2PSet,
                            PublicIp = loginSuccess.PublicIp,
                            Reason = loginSuccess.Reason,
                            SessionId = loginSuccess.SessionId,
                            Status = loginSuccess.Status,
                            UserId = loginSuccess.UserId
                        };
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.NicknameChange:
                        Console.WriteLine("Received NicknameChange");
                        Console.WriteLine($"Nickname: {0}.",
                            ((NicknameChange)message).Nickname);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ServerList:
                        Console.WriteLine("Received ServerList");
                        ServerList serverList = (ServerList)message;

                        if ((serverList.GameIds == null) || (serverList.GameIPs == null) ||
                            (serverList.GamePorts == null))
                        {
                            break;
                        }

                        for(int i=0; i<serverList.GameIds.Count; i++)
                        {
                            this.ListOfServers.Add(new GameInfo()
                            {
                                GameId = serverList.GameIds[i],
                                GameIP = serverList.GameIPs[i],
                                GamePort = serverList.GamePorts[i]
                            });
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.StatusChange:
                        Console.WriteLine("Received StatusChange");
                        Console.WriteLine($"Message: {0}", ((StatusChange)message).Message);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.UDPChatMessage:
                        Console.WriteLine("Received UDPChatMessage");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.Unknown:
                        Console.WriteLine("Received Unknown");
                        UnknownMessageType unknown = (UnknownMessageType)message;

                        if (unknown.AttributeValues == null)
                        {
                            break;
                        }

                        foreach(KeyValuePair<string, dynamic> kvp in unknown.AttributeValues)
                        {
                            Console.WriteLine($"Key: {0}. Value: {1}.",
                                kvp.Key, kvp.Value);
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.Unknown10:
                        Console.WriteLine("Received Unknown10");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.Unknown37:
                        Console.WriteLine("Received Unknown37");
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.UserLookup:
                        Console.WriteLine("Received UserLookup");
                        UserLookup userLookup = (UserLookup)message;
                        Console.WriteLine($"Email: {0}\nFirst Name: {1}\nLast Name: {2}\nUsername: {3}.",
                            userLookup.Email,
                            userLookup.FirstName,
                            userLookup.LastName,
                            userLookup.Username);
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.UserLookupResult:
                        Console.WriteLine("Received UserLookupResult");
                        UserLookupResult lookupResult = (UserLookupResult)message;

                        if ((lookupResult.Emails == null) || (lookupResult.FirstNames == null)
                            || (lookupResult.LastNames == null) || (lookupResult.Usernames == null))
                        {
                            break;
                        }

                        for(int i=0; i<lookupResult.Emails.Count; i++)
                        {
                            Console.WriteLine($"",
                                lookupResult.Emails[i],
                                lookupResult.FirstNames[i],
                                lookupResult.LastNames[i],
                                lookupResult.Usernames[i]);
                        }
                        break;
                    default:
                        Console.WriteLine("Something else occurred. Probably an undefined message type.");
                        break;
                }
                MessageReceivedHandler?.Invoke(this, new MessageReceivedEventArgs() { MessageReceived = message });
            }
        }
        #endregion
    }
}
