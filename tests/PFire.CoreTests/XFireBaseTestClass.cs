using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using PFire.Client;
using PFire.Protocol.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace PFire.CoreTests
{
    public class XFireBaseTestClass
    {
        /// <summary>
        /// The test server to run locally for testing.
        /// </summary>
        public PFireServer Server { get; set; }

        /// <summary>
        /// The types of expected messages from the server to wait on/check.
        /// </summary>
        public List<Type> ExpectedMessages { get; set; }

        /// <summary>
        /// Messages that were received, but not part of the list of ExpectedMessages
        /// </summary>
        public List<Type> UnexpectedMessages { get; set; }

        public XFireBaseTestClass()
        {
            ExpectedMessages = new List<Type>();
            UnexpectedMessages = new List<Type>();
        }

        /// <summary>
        /// Clears all messages queues.
        /// </summary>
        public void ClearAllMessages()
        {
            if (ExpectedMessages == null)
            {
                ExpectedMessages = new List<Type>();
            }
            
            if (UnexpectedMessages == null)
            {
                UnexpectedMessages = new List<Type>();
            }

            ExpectedMessages.Clear();
            UnexpectedMessages.Clear();
        }

        /// <summary>
        /// Deletes the PFireDB for testing purposes.
        /// </summary>
        public void DeletePFireDB()
        {
            string pfiredb = Path.Combine(Assembly.GetExecutingAssembly().Location, "pfiredb");

            if (File.Exists(pfiredb))
            {
                File.Delete(pfiredb);
            }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Constants.ServerName), Constants.Port);
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Server = new PFireServer(baseDirectory, endPoint);
            Server.Start();

            // Arbitrary wait. Would be better for us to monitor a callback - work for later.
            Thread.Sleep(10000);
        }


        public void StopServer()
        {
            Server.Stop();
        }


        /// <summary>
        /// Simple loop and check to ensure that we receive all of the expected messages and no unexpected messages.
        /// </summary>
        public void WaitForExpectedMessages(bool failOnUnexpected)
        {
            Logger.LogMessage("Waiting for messages to be received...");
            int waitCount = 0;
            while ((ExpectedMessages.Count > 0) && (waitCount < 10))
            {
                System.Threading.Thread.Sleep(500);
                waitCount++;
            }

            if ((waitCount == 10) ||
                (UnexpectedMessages.Count > 0) ||
                (ExpectedMessages.Count > 0))
            {
                Logger.LogMessage("Expected messages that were not found:");
                foreach (Type t in ExpectedMessages)
                {
                    Logger.LogMessage(t.Name);
                }

                Logger.LogMessage("Unexpected messages that were received:");
                foreach (Type t in UnexpectedMessages)
                {
                    Logger.LogMessage(t.Name);
                }

                if (failOnUnexpected)
                {
                    Assert.Fail("The expected messages either were not received, or additional messages were received.");
                }
                else
                {
                    Logger.LogMessage("The expected messages either were not received, or additional messages were received.");
                }

            }
        }

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

                if (!ExpectedMessages.Contains(message.GetType()))
                {
                    Logger.LogMessage($"Message with type {message.GetType()} was not expected.");
                    UnexpectedMessages.Add(message.GetType());
                }

                ExpectedMessages.Remove(message.GetType());
            }
            catch (ObjectDisposedException)
            {
                Logger.LogMessage("Underlying connection was disposed of.");
            }
        }
    }
}
