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
        #region Private Variables
        private XFireClient _xFireClient;
        private Thread _listenerThread;
        private TcpClient _client;
        #endregion

        #region Public Variables
        public delegate void MessageReceivedHandlerDelegate(object sender, MessageReceivedEventArgs args);
        public event EventHandler<MessageReceivedEventArgs> MessageReceivedHandler;
        #endregion

        #region Constructor
        public XFireUser(string host, int port, string userName, string passWord)
        {
            LoginUser(host, port);
            _xFireClient.User = new PFire.Database.User()
            {
                Username = userName,
                Password = passWord
            };
        }
        #endregion

        #region Public Methods
        public void Connect()
        {
            //IPEndPoint ipAddy = IPEndPoint.Parse("127.0.0.1:25999");
            
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


        public void Disconnect()
        {
            if (_xFireClient == null)
            {
                return;
            }

            _xFireClient.TcpClient.Close();
        }


        public void SendFriendRequest(string user, string nickname, string msg)
        {
            FriendRequest request = new FriendRequest()
            {
                Username = user,
                Message = msg
            };
            _xFireClient.SendMessage(request);
        }

        public void GetFriendRequests()
        {
        }

        #endregion

        #region Private Methods
        private void LoginUser(string hostName, int port)
        {

            _client = new TcpClient();
            _client.Connect(hostName, port);
            _xFireClient = new XFireClient(_client);
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
            Byte[] bytesReceived = new Byte[4096];
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
                catch(Exception)
                {
                    Console.WriteLine("Conversion of message failed! Message: {0}", Encoding.Default.GetString(bytesReceived));
                    continue;
                }

                switch(message.MessageTypeId)
                {
                    case PFire.Core.Protocol.Messages.XFireMessageType.ChatContent:
                        Console.WriteLine("Chat message received");

                        if (((ChatMessage)message).MessagePayload == null)
                        {
                            break;
                        }

                        foreach (KeyValuePair<string, dynamic> kvp in ((ChatMessage)message).MessagePayload)
                        {
                            Console.WriteLine("Key:{0}  Value:{1}", kvp.Key, kvp.Value);
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ServerChatMessage:
                        Console.WriteLine("Server Chat message received");

                        if (((ChatMessage)message).MessagePayload == null)
                        {
                            break;
                        }

                        foreach (KeyValuePair<string, dynamic> kvp in ((ChatMessage)message).MessagePayload)
                        {
                            Console.WriteLine("Key:{0}  Value:{1}", kvp.Key, kvp.Value);
                        }
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.ChatRooms:
                        Console.WriteLine("Chat rooms message received. Chat Ids:");

                        if (((ChatRooms)message).ChatIds == null)
                        {
                            break;
                        }

                        foreach (int chatId in ((ChatRooms)message).ChatIds)
                        {
                            Console.WriteLine("\t{0}", chatId);

                        }
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
                            Console.WriteLine($"User: {0}.\tNickname: {1}.\tMessage: {2}.",
                                invite.Usernames[i], invite.Nicknames[i], invite.Messages[i]);
                        }
                            
                        break;
                    case PFire.Core.Protocol.Messages.XFireMessageType.FriendRequest:
                        Console.WriteLine("Received a FriendRequest");
                        FriendRequest request = (FriendRequest)message;
                        Console.WriteLine($"User: {0}.\tMessage: {1}.",
                            request.Username, request.Message);
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
                            Console.WriteLine($"User: {0}.\tNickname: {1}.\tUser Id: {2}",
                                list.Usernames[i], list.Nicks[i], list.UserIds[i]);
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
                            Console.WriteLine($"",
                                assign.UserIds[i], assign.SessionIds[i]);
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
                        Console.WriteLine($"Game Id: {0}.\tGame IP: {1}.Game Port: {2}",
                            info.GameId, info.GameIP, info.GamePort);
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
                            Console.WriteLine($"Group Id: {0}.\tGroup Name: {1}.",
                                groups.GroupIds[i], groups.GroupNames[i]);
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
                            Console.WriteLine($"Group Id: {0}.\tUser Id: {1}",
                                friends.GroupIds[i], friends.UserIds[i]);
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
                        Console.WriteLine($"ClientSet: {0}\n" +
                            $"Country: {1}\nDlSet: {2}\nMaxRect: {3}\nMinRect: {4}\nN1: {5}\nN2: {6}\n" +
                            $"N3: {7}\nNickname: {8}\nP2PSet: {9}\nPublicIp: {10}\nReason: {11}\n" +
                            $"Session Id: {12}\nStatus: {13}\nUser Id: {14}\n",
                            loginSuccess.ClientSet,
                            loginSuccess.Country,
                            loginSuccess.DlSet,
                            loginSuccess.MaxRect,
                            loginSuccess.MinRect,
                            loginSuccess.N1,
                            loginSuccess.N2,
                            loginSuccess.N3,
                            loginSuccess.Nickname,
                            loginSuccess.P2PSet,
                            loginSuccess.PublicIp,
                            loginSuccess.Reason,
                            loginSuccess.SessionId,
                            loginSuccess.Status,
                            loginSuccess.UserId);
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
                            Console.WriteLine($"",
                                serverList.GameIds[i],
                                serverList.GameIPs[i],
                                serverList.GamePorts[i]);
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
