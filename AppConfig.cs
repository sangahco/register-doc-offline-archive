﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace pmis
{
    public static class AppConfig
    {

        //private static string appDataName = ".pmis-archive";

        private static string appDataFullPath;
        public static string AppDataFullPath { get { return appDataFullPath; } }

        public static Dictionary<string, string> Languages = new Dictionary<string, string> {
            { "en_US", "English" },
            { "ko_KR", "Korean" }
        };

        public static Dictionary<string, string> StorageOptions = new Dictionary<string, string>
        {
            { "sqlite", "SQLite" }
        };

        public static string HISTORY_SHOW_ALL = "show_all";
        public static string HISTORY_LATEST = "last_revision";
        public static Dictionary<string, string> RegisterHistoryOptions = new Dictionary<string, string>
        {
            { HISTORY_SHOW_ALL, "Show All Revisions" },
            { HISTORY_LATEST, "Latest Revision Only" }
        };

        public static void InitConfig()
        {
            var _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataFullPath = Path.Combine(_path, ".pmis-archive");

            if (!Directory.Exists(AppDataFullPath))
                Directory.CreateDirectory(AppDataFullPath);

            // set the log root folder
            LogUtil.LogRootFolder = Path.Combine(AppDataFullPath, "logs");
        }

        public static string ProductVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

    }
}
