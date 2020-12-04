using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

using PFire.Client;
using PFire.Protocol.Messages;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace PFire.CoreTests
{
    /* Test class for login/initial handshake tests.
     * Run against an existing database with friend requests and users - probably wise to clear this off beforehand. TODO.
     */
    [TestClass()]
    public class E2E_LoginTests
    {
        #region Private Variables
        private PFire.Client.XFireUser _xFireUser;
        private List<Type> _expectedMessages;
        private List<Type> _unexpectedMessages;
        private PFireServer _server;
        private Logger _logger;
        #endregion

        #region Test Initialize/Cleanup

        /// <summary>
        /// Initializes the test server and sets up class variables for test tracking.
        /// </summary>
        [TestInitialize()]
        public void TestSetup()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Constants.ServerName), Constants.Port);
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _server = new PFireServer(baseDirectory, endPoint);
            _server.Start();

            // Arbitrary wait. Would be better for us to monitor a callback - work for later.
            Thread.Sleep(10000);

            _expectedMessages = new List<Type>();
            _unexpectedMessages = new List<Type>();
        }

        /// <summary>
        /// Stops the test server and cleans up the queue.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            _xFireUser.MessageReceivedHandler -= this.ReceiveMessage;
            _xFireUser.Disconnect();
            _server.Stop();
            _expectedMessages.Clear();
            _unexpectedMessages.Clear();
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests that an existing user can successfully log into the server and that the client receives all necessary messages.
        /// </summary>
        [TestMethod()]
        public void TestSuccessfulLogin()
        {
            _xFireUser = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName, Constants.Password);
            _xFireUser.MessageReceivedHandler += this.ReceiveMessage;

            // Expected incoming order of message
            _expectedMessages.Add(typeof(LoginChallenge));
            _expectedMessages.Add(typeof(LoginSuccess));
            _expectedMessages.Add(typeof(Did));
            _expectedMessages.Add(typeof(Unknown10));
            _expectedMessages.Add(typeof(Groups));
            _expectedMessages.Add(typeof(GroupsFriends));
            _expectedMessages.Add(typeof(ServerList));
            _expectedMessages.Add(typeof(ChatRooms));
            _expectedMessages.Add(typeof(FriendsList));
            _expectedMessages.Add(typeof(FriendsSessionAssign));

            _xFireUser.Connect();
            WaitForExpectedMessages();

        }

        /// <summary>
        /// Tests that an existing username with the wrong password fails login.
        /// CURRENTLY FAILING.
        /// </summary>
        [TestMethod]
        public void TestLoginFailure()
        {
            try
            {
                _xFireUser = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName, "absolutelywrong");
                _xFireUser.MessageReceivedHandler += this.ReceiveMessage;

                _expectedMessages.Add(typeof(LoginChallenge));
                _expectedMessages.Add(typeof(LoginFailure));
                _expectedMessages.Add(typeof(Did));
                _expectedMessages.Add(typeof(Unknown10));

                _xFireUser.Connect();
                WaitForExpectedMessages();
            }
            catch(ObjectDisposedException)
            {
                Logger.LogMessage("Underlying connection was disposed of.");
            }
        }

        /// <summary>
        /// Ensures that a brand new username/password combination is created on initial log in and login is successful.
        /// </summary>
        [TestMethod]
        public void TestNewUserLogin()
        {
            string rndString = Path.GetRandomFileName();
            rndString = rndString.Replace(".", ""); // Remove period

            _xFireUser = new XFireUser(Constants.ServerName, Constants.Port, rndString, rndString);
            _xFireUser.MessageReceivedHandler += this.ReceiveMessage;

            // Expected incoming order of message
            _expectedMessages.Add(typeof(LoginChallenge));
            _expectedMessages.Add(typeof(LoginSuccess));
            _expectedMessages.Add(typeof(Did));
            _expectedMessages.Add(typeof(Unknown10));
            _expectedMessages.Add(typeof(Groups));
            _expectedMessages.Add(typeof(GroupsFriends));
            _expectedMessages.Add(typeof(ServerList));
            _expectedMessages.Add(typeof(ChatRooms));
            _expectedMessages.Add(typeof(FriendsList));
            _expectedMessages.Add(typeof(FriendsSessionAssign));

            _xFireUser.Connect();
            WaitForExpectedMessages();
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback for receiving messages.
        /// </summary>
        /// <param name="sender">The object that sent the notification (XFireClient).</param>
        /// <param name="args">A MessageReceivedEventArgs object containing the message.</param>
        public void ReceiveMessage(object sender, MessageReceivedEventArgs args)
        {
            try
            {
                IMessage message = args.MessageReceived;

                if (!_expectedMessages.Contains(message.GetType()))
                {
                    Logger.LogMessage($"Message with type {message.GetType()} was not expected.");
                    _unexpectedMessages.Add(message.GetType());
                }

                _expectedMessages.Remove(message.GetType());
            }
            catch (ObjectDisposedException)
            {
                Logger.LogMessage("Underlying connection was disposed of.");
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Simple loop and check to ensure that we receive all of the expected messages and no unexpected messages.
        /// </summary>
        private void WaitForExpectedMessages()
        {
            Logger.LogMessage("Waiting for messages to be received...");
            int waitCount = 0;
            while ((_expectedMessages.Count > 0) && (waitCount < 10))
            {
                System.Threading.Thread.Sleep(500);
                waitCount++;
            }

            if ((waitCount == 10) ||
                (_unexpectedMessages.Count > 0) ||
                (_expectedMessages.Count > 0))
            {
                Logger.LogMessage("Expected messages that were not found:");
                foreach(Type t in _expectedMessages)
                {
                    Logger.LogMessage(t.Name);
                }

                Logger.LogMessage("Unexpected messages that were received:");
                foreach(Type t in _unexpectedMessages)
                {
                    Logger.LogMessage(t.Name);
                }

                Assert.Fail("The expected messages either were not received, or additional messages were received.");

            }
        }
        #endregion
    }
}
