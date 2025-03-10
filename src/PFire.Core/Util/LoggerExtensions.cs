using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages;

namespace PFire.Core.Util
{
    internal static class LoggerExtensions
    {
        private static readonly IList<XFireMessageType> IgnoreMessageIds = new[] {XFireMessageType.KeepAlive};
        
        public static void LogXFireMessage(this ILogger logger, IMessage message, UserModel user)
        {
            if (IgnoreMessageIds.Contains(message.MessageTypeId))
            {
                return;
            }

            var username = user?.Username ?? "unknown";
            var userId = user?.Id ?? 0;
            
            logger.LogDebug($"Sent message[{username},{userId}]: {message}");
        }
    }
}
