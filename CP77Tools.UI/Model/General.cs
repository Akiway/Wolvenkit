using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CP77Tools;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace CP77Tools.UI.Model
{
    public class General
    {
        private delegate void StrDelegate(string value);
        public static string[] archives;
        public static string cp77BinDir = "";
        public static string cp77Dir = "";

        public static void TryGetGameInstallDir()
        {
            var cp77exe = "";
            // check for CP77_DIR environment variable first
            var CP77_DIR = System.Environment.GetEnvironmentVariable("CP77_DIR", EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(CP77_DIR) && new DirectoryInfo(CP77_DIR).Exists)
                cp77BinDir = Path.Combine(CP77_DIR, "bin", "x64");
            if (File.Exists(Path.Combine(cp77BinDir, "Cyberpunk2077.exe")))
            {
                cp77Dir = CP77_DIR;
                return;
            }

            // else: look for install location
            const string uninstallkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";
            const string uninstallkey2 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";
            const string gameName = "Cyberpunk 2077";
            const string exeName = "Cyberpunk2077.exe";
            var exePath = "";
            StrDelegate strDelegate = msg => cp77exe = msg;

            try
            {
                Parallel.ForEach(Registry.LocalMachine.OpenSubKey(uninstallkey)?.GetSubKeyNames(), item =>
                {
                    var programName = Registry.LocalMachine.OpenSubKey(uninstallkey + item)
                        ?.GetValue("DisplayName");
                    var installLocation = Registry.LocalMachine.OpenSubKey(uninstallkey + item)
                        ?.GetValue("InstallLocation");
                    if (programName != null && installLocation != null)
                    {
                        if (programName.ToString().Contains(gameName) ||
                            programName.ToString().Contains(gameName))
                        {
                            cp77Dir = (string)installLocation;
                            exePath = Directory.GetFiles(installLocation.ToString(), exeName, SearchOption.AllDirectories).First();
                        }
                    }

                    strDelegate.Invoke(exePath);
                });
                Parallel.ForEach(Registry.LocalMachine.OpenSubKey(uninstallkey2)?.GetSubKeyNames(), item =>
                {
                    var programName = Registry.LocalMachine.OpenSubKey(uninstallkey2 + item)
                        ?.GetValue("DisplayName");
                    var installLocation = Registry.LocalMachine.OpenSubKey(uninstallkey2 + item)
                        ?.GetValue("InstallLocation");
                    if (programName != null && installLocation != null)
                    {
                        if (programName.ToString().Contains(gameName) ||
                            programName.ToString().Contains(gameName))
                        {
                            if (Directory.Exists(installLocation.ToString()))
                            {
                                cp77Dir = (string)installLocation;
                                exePath = Directory.GetFiles(installLocation.ToString(), exeName, SearchOption.AllDirectories).First();
                            }
                        }
                    }

                    strDelegate.Invoke(exePath);
                });

                if (File.Exists(cp77exe))
                    cp77BinDir = new FileInfo(cp77exe).Directory.FullName;
            }
            catch (Exception e)
            {

            }
        }

        public static bool LoadArchives()
        {
            if (string.IsNullOrEmpty(cp77Dir))
                TryGetGameInstallDir();
            if (string.IsNullOrEmpty(cp77Dir))
                return false;

            var folderPath = Path.Combine(cp77Dir, "archive", "pc", "content");
            if (!Directory.Exists(folderPath))
                return false;

            archives = Directory.GetFiles(folderPath, "*.archive");

            /*foreach (string archive in archives)
                Debug.WriteLine(Path.GetFileNameWithoutExtension(archive));*/

            
            return true;
        }

        public static bool TryCopyOodleLib()
        {
            if (string.IsNullOrEmpty(cp77Dir))
                TryGetGameInstallDir();

            var ass = AppDomain.CurrentDomain.BaseDirectory;
            var destFileName = Path.Combine(ass, "oo2ext_7_win64.dll");
            if (File.Exists(destFileName))
                return true;

            if (string.IsNullOrEmpty(cp77BinDir))
                return false;

            // copy oodle dll
            var oodleInfo = new FileInfo(Path.Combine(cp77BinDir, "oo2ext_7_win64.dll"));
            if (!oodleInfo.Exists)
                return false;

            if (!File.Exists(destFileName))
                oodleInfo.CopyTo(destFileName);

            return true;
        }
    }
}
