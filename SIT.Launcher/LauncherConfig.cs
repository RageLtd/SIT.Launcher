﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Launcher
{
    public class LauncherConfig
    {

        private static LauncherConfig instance;// = new LauncherConfig();

        public static LauncherConfig Instance
        {
            get
            {
                if (instance == null)
                    Instance = Load();

                return instance;
            }
            private set => instance = value;
        }


        private LauncherConfig()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public ICollection<ServerInstance> ServerInstances { get; set; }

        public ServerInstance ServerInstance { get; set; } =
            new ServerInstance() { ServerAddress = "https://127.0.0.1:443", WebsocketUrl = string.Empty };

        public string Username { get; set; }

        public string InstallLocation { get; set; }

        public bool AutomaticallyInstallAssemblyDlls { get; set; } = true;

        public bool AutomaticallyDeobfuscateDlls { get; set; } = true;

        public bool AutomaticallyInstallSIT { get; set; } = true;
        public bool AutomaticallyInstallSITPreRelease { get; set; } = false;
        public bool ForceInstallLatestSIT { get; set; } = false;
        public bool AutomaticallyInstallAkiSupport { get; set; } = true;

        public bool EnableCoopServer { get; set; } = false;

        public bool SendInfoToDiscord { get; set; } = false;
        public bool CloseLauncherAfterLaunch { get; set; } = false;

        private static LauncherConfig Load()
        {
            LauncherConfig launcherConfig;

            if (File.Exists(App.ApplicationDirectory + "LauncherConfig.json"))
                launcherConfig = JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText(App.ApplicationDirectory + "LauncherConfig.json"));
            else
                launcherConfig = new LauncherConfig()
                {
                    AutomaticallyDeobfuscateDlls = true,
                    AutomaticallyInstallAssemblyDlls = true,
                    AutomaticallyInstallSIT = true,
                    AutomaticallyInstallAkiSupport = true,
                    CloseLauncherAfterLaunch = false,
                    ServerInstance = new ServerInstance() { ServerAddress = "http://127.0.0.1:6969", WebsocketUrl = "http://127.0.0.1:6970" }
                };

            if (launcherConfig.ServerInstance.ServerAddress.EndsWith("/"))
            {
                launcherConfig.ServerInstance.ServerAddress = launcherConfig.ServerInstance.ServerAddress[..^1];
            }
            return launcherConfig;
        }

        public void Save()
        {
            File.WriteAllText("LauncherConfig.json", JsonConvert.SerializeObject(this));
        }
    }
}
