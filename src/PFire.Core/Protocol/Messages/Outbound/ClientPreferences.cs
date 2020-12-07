﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClientPreferences : XFireMessage
    {
        public ClientPreferences() : base(XFireMessageType.ClientPreferences) {}

        [XMessageField(0x4c)]
        public Dictionary<byte, string> preferences { get; set; }

        public override Task Process(IXFireClient context)
        {
            preferences = new Dictionary<byte, string>
            {
                {
                    1, "0"
                },
                {
                    4, "0"
                },
                {
                    5, "0"
                },
                {
                    6, "1"
                },
                {
                    7, "0"
                },
                {
                    8, "0"
                },
                {
                    11, "0"
                },
                {
                    17, "0"
                },
                {
                    18, "0"
                },
                {
                    19, "0"
                },
                {
                    20, "0"
                },
                {
                    21, "0"
                }
            };

            return Task.CompletedTask;
        }
    }
}
