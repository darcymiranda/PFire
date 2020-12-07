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
    /// <summary>
    /// Test class for login/initial handshake tests.
    /// </summary>
    [TestClass()]
    public class E2E_LoginTests : XFireBaseTestClass
    {
        #region Test Initialize/Cleanup
        /// <summary>
        /// Initializes the test server and sets up class variables for test tracking.
        /// </summary>
        [TestInitialize()]
        public void TestSetup()
        {
            DeletePFireDB();
            StartServer();
            ClearAllMessages();
        }

        /// <summary>
        /// Stops the test server and cleans up the queue.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            StopServer();
            DeletePFireDB();
            ClearAllMessages();
        }
        #endregion

        #region Tests
        /// <summary>
        /// Tests that an existing user can successfully log into the server and that the client receives all necessary messages.
        /// </summary>
        [TestMethod()]
        public void TestSuccessfulLogin()
        {
            XFireUser user1 = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName, Constants.Password, Server);
            user1.MessageReceivedHandler += ReceiveMessage;

            // Expected incoming order of message
            ExpectedMessages.Add(typeof(LoginChallenge));
            ExpectedMessages.Add(typeof(LoginSuccess));
            ExpectedMessages.Add(typeof(Did));
            ExpectedMessages.Add(typeof(Unknown10));
            ExpectedMessages.Add(typeof(Groups));
            ExpectedMessages.Add(typeof(GroupsFriends));
            ExpectedMessages.Add(typeof(ServerList));
            ExpectedMessages.Add(typeof(ChatRooms));
            ExpectedMessages.Add(typeof(FriendsList));
            ExpectedMessages.Add(typeof(FriendsSessionAssign));

            user1.Connect();
            WaitForExpectedMessages(true);

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
                XFireUser user1 = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName, "absolutelywrong", Server);
                user1.MessageReceivedHandler += ReceiveMessage;

                ExpectedMessages.Add(typeof(LoginChallenge));
                ExpectedMessages.Add(typeof(LoginFailure));
                ExpectedMessages.Add(typeof(Did));
                ExpectedMessages.Add(typeof(Unknown10));

                user1.Connect();
                WaitForExpectedMessages(true);
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

            XFireUser user1 = new XFireUser(Constants.ServerName, Constants.Port, rndString, rndString, Server);
            user1.MessageReceivedHandler += ReceiveMessage;

            // Expected incoming order of message
            ExpectedMessages.Add(typeof(LoginChallenge));
            ExpectedMessages.Add(typeof(LoginSuccess));
            ExpectedMessages.Add(typeof(Did));
            ExpectedMessages.Add(typeof(Unknown10));
            ExpectedMessages.Add(typeof(Groups));
            ExpectedMessages.Add(typeof(GroupsFriends));
            ExpectedMessages.Add(typeof(ServerList));
            ExpectedMessages.Add(typeof(ChatRooms));
            ExpectedMessages.Add(typeof(FriendsList));
            ExpectedMessages.Add(typeof(FriendsSessionAssign));

            user1.Connect();
            WaitForExpectedMessages(true);
        }
        #endregion
    }
}
