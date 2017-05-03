﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Athame.Logging;
using Athame.Plugin;
using Athame.PluginAPI;
using Athame.Settings;
using Athame.UI;

namespace Athame
{
    public static class Program
    {
        private const string SettingsFilename = "settings.json";
        private static string SettingsPath;

        public static AthameApplication DefaultApp;
        public static PluginManager DefaultPluginManager;
        public static SettingsManager<AthameSettings> DefaultSettings;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {

            // Create app instance config
            DefaultApp = new AthameApplication
            {
                IsWindowed = true,
#if DEBUG
                UserDataPath = Path.Combine(Directory.GetCurrentDirectory(), "UserDataDebug")
#else
                UserDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Athame")
#endif
            };

            // Install logging
            Log.AddLogger("file", new FileLogger(DefaultApp.UserDataPath));
#if !DEBUG
            Log.Filter = Level.Warning;
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Log.WriteException(Level.Fatal, "AppDomain", args.ExceptionObject as Exception);
            };
#endif

            // Ensure user data dir
            Directory.CreateDirectory(DefaultApp.UserDataPath);

            // Load settings
            SettingsPath = DefaultApp.UserDataPathOf(SettingsFilename);
            DefaultSettings = new SettingsManager<AthameSettings>(SettingsPath);

            // Create plugin manager instance
            DefaultPluginManager = new PluginManager(Path.Combine(Directory.GetCurrentDirectory(), PluginManager.PluginDir));
            
            // Begin main form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
