using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WolvenKit.Common.Services;
using System.Timers;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Pack.xaml
    /// </summary>
    public partial class Pack : System.Windows.Controls.UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = Model.GUIConsole.Instance;

        private System.Timers.Timer timerLoadBar;

        private string folder = "";

        public Pack()
        {
            InitializeComponent();
        }
        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            // Do not allow the user to create new files via the FolderBrowserDialog.
            folderBrowserDialog.ShowNewFolderButton = false;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folder = folderBrowserDialog.SelectedPath;
                TextFolderSelected.Text = folder;
            }
        }

        private void btnPack_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folder))
            {
                guiConsole.logger.LogString("No folder indicated.", Logtype.Error);
                return;
            }

            string[] buffers = Directory.GetFiles(folder, "*.buffer", SearchOption.AllDirectories);
            string[] dds = Directory.GetFiles(folder, "*.dds", SearchOption.AllDirectories);

            string[] folderList = { folder };
            Debug.WriteLine(buffers.Length);
            Debug.WriteLine(dds.Length);

            var bufferRecombineNeeded = buffers.Length > 0 || (bool)ForceRebuildOption.IsChecked;
            var textureRecombineNeeded = dds.Length > 0 || (bool)ForceRebuildOption.IsChecked;
            var keepOption = (bool)KeepOption.IsChecked;
            var cleanOption = (bool)CleanOption.IsChecked;
            var unsaferawOption = (bool)UnsaferawOption.IsChecked;

            Task.Run(() =>
            {
                // Rebuild if needed
                if (bufferRecombineNeeded || textureRecombineNeeded)
                {
                    try
                    {
                        StartRebuildLoadBar();
                        ConsoleFunctions.RebuildTask(folderList, bufferRecombineNeeded, textureRecombineNeeded, false, keepOption, cleanOption, unsaferawOption);
                    } catch (Exception e)
                    {
                        handleException("rebuild", e.Message);
                    }
                    while (timerLoadBar.Enabled)
                        Thread.Sleep(200);

                    try
                    {
                        StartPackLoadBar();
                        ConsoleFunctions.PackTask(folderList, null);
                    } catch (Exception e)
                    {
                        handleException("pack", e.Message);
                    }
                } else
                {
                    try
                    {
                        StartPackLoadBar();
                        ConsoleFunctions.PackTask(folderList, null);
                    }
                    catch (Exception e)
                    {
                        handleException("pack", e.Message);
                    }
                }

                void handleException(string step, string errorMessage)
                {
                    guiConsole.logger.LogString("Something went wrong during the " + step + " step, please verify your files, some might be missing.", Logtype.Error);
                    guiConsole.logger.LogString(errorMessage, Logtype.Error);
                }
            });
        }

        #region RebuildLoadBar
        private void StartRebuildLoadBar()
        {
            guiConsole.logger.LogProgress(0);
            mainDispatcher.BeginInvoke((Action)(() =>
            {
                RebuildLoadBar.Value = 0;
                DisplayRebuildBar(true);
            }));
            timerLoadBar = new System.Timers.Timer(200);
            timerLoadBar.Elapsed += UpdateRebuildLoadBar;
            timerLoadBar.AutoReset = true;
            timerLoadBar.Enabled = true;
        }

        private void UpdateRebuildLoadBar(Object source, ElapsedEventArgs e)
        {
            if (guiConsole.logger.Progress is null)
                return;

            if (guiConsole.logger.Progress.Item1 == 1)
                timerLoadBar.Dispose();

            mainDispatcher.BeginInvoke((Action)(() =>
            {
                if (guiConsole.logger.Progress.Item1 == 1)
                    DisplayRebuildBar(false);
                RebuildLoadBar.Value = guiConsole.logger.Progress.Item1;
            }));
        }

        private void DisplayRebuildBar(bool visibility)
        {
            RebuildBar.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion

        #region PackLoadBar
        private void StartPackLoadBar()
        {
            guiConsole.logger.LogProgress(0);
            mainDispatcher.BeginInvoke((Action)(() =>
            {
                PackLoadBar.Value = 0;
                DisplayPackBar(true);
            }));
            timerLoadBar = new System.Timers.Timer(200);
            timerLoadBar.Elapsed += UpdatePackLoadBar;
            timerLoadBar.AutoReset = true;
            timerLoadBar.Enabled = true;
        }
        private void UpdatePackLoadBar(Object source, ElapsedEventArgs e)
        {
            if (guiConsole.logger.Progress is null)
                return;

            if (guiConsole.logger.Progress.Item1 == 1)
                timerLoadBar.Dispose();

            mainDispatcher.BeginInvoke((Action)(() =>
            {
                if (guiConsole.logger.Progress.Item1 == 1)
                    DisplayPackBar(false);
                PackLoadBar.Value = guiConsole.logger.Progress.Item1;
            }));
        }

        private void DisplayPackBar(bool visibility)
        {
            PackBar.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion
    }
}
