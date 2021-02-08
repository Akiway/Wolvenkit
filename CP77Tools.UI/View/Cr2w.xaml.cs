using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WolvenKit.Common.DDS;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Cr2w.xaml
    /// </summary>
    public partial class Cr2w : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = Model.GUIConsole.Instance;

        private System.Timers.Timer timerLoadBar;

        private List<string> files = new List<string>();
        private List<string> filesName = new List<string>();
        private bool displayFullPath = false;

        private bool _isRegex = false;
        public Cr2w()
        {
            InitializeComponent();
        }
        private void updateFileList()
        {
            filesName.Clear();
            filesName.AddRange(files.ConvertAll(f => System.IO.Path.GetFileName(f)));
            PathListBadge.Badge = files.Count > 0 ? files.Count : null;
            Cr2wFileList.ItemsSource = new List<string>();
            Cr2wFileList.ItemsSource = displayFullPath ? files : filesName;
        }
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files|*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                files.AddRange(openFileDialog.FileNames);
                files = files.Distinct().ToList();
                files.Sort();
                updateFileList();
            }
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string valueToRemove = (((Grid)button.Parent).Children[0] as TextBlock).Text;
            if (displayFullPath)
                files.RemoveAt(files.IndexOf(valueToRemove));
            else
                files.RemoveAt(filesName.IndexOf(valueToRemove));
            updateFileList();
        }

        private void FolderList_FullPath_Click(object sender, RoutedEventArgs e)
        {
            displayFullPath = (bool)((System.Windows.Controls.CheckBox)sender).IsChecked;
            updateFileList();
        }

        private void RadioPattern_checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if ((bool)button.IsChecked)
                _isRegex = button.Content.ToString() == "Regex";
        }

        private void btnDump_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1)
            {
                guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                return;
            }

            bool chunks = (bool)ClassOption.IsChecked;
            //string regex = _isRegex ? PatternInput.Text : "";
            //string pattern = !_isRegex ? RegexInput.Text : "";


            Task.Run(() =>
            {
                StartDumpLoadBar();
                ConsoleFunctions.Cr2wTask(files.ToArray(), null, chunks, "", "");
            });
        }

        #region DumpLoadBar
        private void StartDumpLoadBar()
        {
            guiConsole.logger.LogProgress(0);
            mainDispatcher.BeginInvoke((Action)(() =>
            {
                DumpLoadBar.Value = 0;
                DisplayDumpBar(true);
            }));
            timerLoadBar = new Timer(200);
            timerLoadBar.Elapsed += UpdateDumpLoadBar;
            timerLoadBar.AutoReset = true;
            timerLoadBar.Enabled = true;
        }

        private void UpdateDumpLoadBar(Object source, ElapsedEventArgs e)
        {
            if (guiConsole.logger.Progress is null)
                return;

            if (guiConsole.logger.Progress.Item1 == 1)
                timerLoadBar.Dispose();

            mainDispatcher.BeginInvoke((Action)(() =>
            {
                if (guiConsole.logger.Progress.Item1 == 1)
                    DisplayDumpBar(false);
                DumpLoadBar.Value = guiConsole.logger.Progress.Item1;
            }));
        }

        private void DisplayDumpBar(bool visibility)
        {
            DumpBar.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion
    }
}
