using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using PFire.Client;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace PFire.CoreTests
{
    /// <summary>
    /// Verifies multi-user scenarios.
    /// </summary>
    [TestClass]
    public class E2E_MultiUserTests : XFireBaseTestClass
    {
        #region Test Setup/Cleanup
        [TestInitialize]
        public void TestSetup()
        {
            DeletePFireDB();
            StartServer();
            ClearAllMessages();
        }


        [TestCleanup]
        public void TestCleanup()
        {
            StopServer();
            DeletePFireDB();
            ClearAllMessages();
        }
        #endregion

        #region Tests
        /// <summary>
        /// Verifies that more than one user can be logged in at a time, and that the session IDs for each user are different.
        /// </summary>
        [TestMethod]
        public void TestMultiUserLogin()
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

            XFireUser user3 = new XFireUser(Constants.ServerName, Constants.Port, Constants.UserName3, Constants.Password3, Server);
            MessageListener listen3 = new MessageListener();
            listen3.ExpectedMessages.Add(typeof(LoginSuccess));
            user3.MessageReceivedHandler += listen3.ReceiveMessage;
            user3.Connect();
            listen3.WaitForExpectedMessages();

            Assert.AreNotEqual(user1.LoginInformation.SessionId, user2.LoginInformation.SessionId);
            Assert.AreNotEqual(user1.LoginInformation.SessionId, user3.LoginInformation.SessionId);
            Assert.AreNotEqual(user3.LoginInformation.SessionId, user2.LoginInformation.SessionId);
        }


        #endregion
    }
}
