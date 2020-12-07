using Microsoft.VisualStudio.TestTools.UnitTesting;
using PFire.Client;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;
using System;
using System.Threading;

namespace PFire.CoreTests
{
    /// <summary>
    /// Tests friend requests, accepts, denies, and removals.
    /// </summary>
    [TestClass]
    public class E2E_FriendTests : XFireBaseTestClass
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
        [TestMethod]
        public void TestFriendRequestAndAccept()
        {
            XFireUser user1 = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName, Constants.Password, Server);
            MessageListener listen1 = new MessageListener();
            listen1.ExpectedMessages.Add(typeof(LoginSuccess));
            user1.MessageReceivedHandler += listen1.ReceiveMessage;
            user1.Connect();
            listen1.WaitForExpectedMessages();

            XFireUser user2 = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName2, Constants.Password2, Server);
            MessageListener listen2 = new MessageListener();
            listen2.ExpectedMessages.Add(typeof(LoginSuccess));
            user2.MessageReceivedHandler += listen2.ReceiveMessage;
            user2.Connect();
            listen2.WaitForExpectedMessages();

            user1.SendFriendRequest(Constants.UserName2, Constants.FriendMessage);
            listen2.ExpectedMessages.Add(typeof(FriendInvite));
            listen2.WaitForExpectedMessages();

            Thread.Sleep(5000);

            Assert.IsTrue(user2.FriendInvites.Count >= 1);

            listen1.ExpectedMessages.Add(typeof(FriendRequestAccept));
            user2.AcceptFriendRequest(user2.FriendInvites[0].Username);
            listen1.WaitForExpectedMessages();

        }
        #endregion

    }
}
