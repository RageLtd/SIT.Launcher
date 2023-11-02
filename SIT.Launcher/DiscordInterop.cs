﻿using NetDiscordRpc;
using NetDiscordRpc.RPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Launcher
{
    public class DiscordInterop
    {
        private static DiscordRPC discordClient;

        public static DiscordRPC DiscordRpcClient
        {
            get
            {
                //if (discordClient == null)
                //{
                //    var p = new DiscordInterop().StartDiscordClient().Result;
                //    discordClient = p;
                //}
                return discordClient;
            }
            set { discordClient = value; }
        }


        public DiscordRPC StartDiscordClient(string productVersion = "")
        {
            //return await Task.Run(() =>
            //{
            if (DiscordRpcClient == null)
            {
                DiscordRpcClient = new DiscordRPC("983769140684791808")
                {
                    Logger = new NetDiscordRpc.Core.Logger.NullLogger()
                };
                DiscordRpcClient.Initialize();
                DiscordRpcClient.SetPresence(new RichPresence()
                {
                    Details = "",
                    State = productVersion,
                });
                //DiscordRpcClient.Invoke();

            }
            return DiscordRpcClient;
            //});
        }

        public DiscordRPC GetDiscordRPC()
        {
            return StartDiscordClient();
        }
    }
}
