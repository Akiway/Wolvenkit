using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WolvenKit.Common.Services;
using System.Timers;
using System.Threading;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

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

        private List<string> folders = new List<string>();
        private List<string> foldersName = new List<string>();
        private bool displayFullPath = false;

        public Pack()
        {
            InitializeComponent();
        }
        private void updateFileList()
        {
            foldersName.Clear();
            foldersName.AddRange(folders.ConvertAll(f => System.IO.Path.GetFileName(f)));
            PathListBadge.Badge = folders.Count > 0 ? folders.Count : null;
            PackFolderList.ItemsSource = new List<string>();
            PackFolderList.ItemsSource = displayFullPath ? folders : foldersName;
        }
        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            // Do not allow the user to create new files via the FolderBrowserDialog.
            folderBrowserDialog.ShowNewFolderButton = false;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folders.Add(folderBrowserDialog.SelectedPath);
                folders = folders.Distinct().ToList();
                folders.Sort();
                updateFileList();
            }
        }
        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            string valueToRemove = (((Grid)button.Parent).Children[0] as TextBlock).Text;
            if (displayFullPath)
                folders.RemoveAt(folders.IndexOf(valueToRemove));
            else
                folders.RemoveAt(foldersName.IndexOf(valueToRemove));
            updateFileList();
        }
        
        private void PackFolderList_FullPath_Click(object sender, RoutedEventArgs e)
        {
            displayFullPath = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            updateFileList();
        }
        private void btnPack_Click(object sender, RoutedEventArgs e)
        {
            if (folders.Count < 1)
            {
                guiConsole.logger.LogString("No folder indicated.", Logtype.Error);
                return;
            }
            List<string> buffers = new List<string>();
            List<string> dds = new List<string>();
            folders.ForEach(folder =>
            {
                buffers.AddRange(Directory.GetFiles(folder, "*.buffer", SearchOption.AllDirectories));
                dds.AddRange(Directory.GetFiles(folder, "*.dds", SearchOption.AllDirectories));
            });


            string[] folderList = folders.ToArray();

            var bufferRecombineNeeded = buffers.Count > 0 || (bool)ForceRebuildOption.IsChecked;
            var textureRecombineNeeded = dds.Count > 0 || (bool)ForceRebuildOption.IsChecked;
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
