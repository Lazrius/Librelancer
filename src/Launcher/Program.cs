// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.IO;
using System.Runtime.InteropServices;
using LibreLancer;
using LibreLancer.Dialogs;

namespace Launcher
{
    static class Program
    {
        public static bool introForceDisable = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppHandler.Run(() =>
            {
                WindowsChecks();
                new MainWindow().Run();
            });
        }

 
        static void WindowsChecks()
        {
            if (Platform.RunningOS != OS.Windows) return;
            object legacyWMPCheck = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Active Setup\Installed Components\{22d6f312-b0f6-11d0-94ab-0080c74c7e95}", "IsInstalled", null);
            if (legacyWMPCheck == null || legacyWMPCheck.ToString() != "1")
            {
                introForceDisable = true;
                var msg = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("Launcher.WMPMessage.txt")).ReadToEnd();
                CrashWindow.Run("Librelancer", "Missing Components", msg);
            }
        }
    }
}
    