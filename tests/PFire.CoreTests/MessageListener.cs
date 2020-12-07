using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

using PFire.Client;
using PFire.Protocol.Messages;
using System;
using System.Collections.Generic;

namespace PFire.CoreTests
{
    /// <summary>
    /// For generic or single-user message checks, use the XFireBaseTestClass methods.
    /// These are user-specific so each user can have their own listening queue.
    /// </summary>
    internal class MessageListener
    {
        public List<Type> ExpectedMessages;
        public List<Type> UnexpectedMessages;

        public MessageListener()
        {
            ExpectedMessages = new List<Type>();
            UnexpectedMessages = new List<Type>();
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

        /// <summary>
        /// Simple loop and check to ensure that we receive all of the expected messages and no unexpected messages.
        /// </summary>
        public void WaitForExpectedMessages()
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

                Logger.LogMessage("The expected messages either were not received, or additional messages were received.");

            }
        }
    }
}
