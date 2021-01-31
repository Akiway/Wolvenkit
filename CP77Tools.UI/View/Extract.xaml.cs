﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using CP77Tools.Tasks;
using WolvenKit.Common.Services;
using WolvenKit.Common.DDS;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Extract.xaml
    /// </summary>
    public partial class Extract : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = Model.GUIConsole.Instance;

        private Timer timerLoadBar;

        private string[] archives = Array.Empty<string>();
        private string[] extensions = { "dds", "tga", "png", "jpeg", "jpg", "bmp" };
        
        private static string command = "Unbundle";
        private static string selectedArchive;

        private bool _isRegex = false;
        private bool _isUncookSelected = false;
        private bool _isArchiveFromGame = true;
        public bool ShowUncookPanel
        {
            get { return _isUncookSelected; }
        }

        public Extract()
        {
            InitializeComponent();

            if (General.LoadArchives())
                archives = General.archives;

            ArchivesDropdown.ItemsSource = Array.ConvertAll(archives, archive => Path.GetFileNameWithoutExtension(archive));
            ExtensionsDropdown.ItemsSource = extensions;
        }
        private void btnSelectArchive_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archive (*.archive)|*.archive";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedArchive = Path.GetFullPath(openFileDialog.FileName);
                TextArchiveSelected.Text = Path.GetFileName(selectedArchive);
            }
        }

        private void RadioCommand_checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if ((bool)button.IsChecked)
                command = button.Content.ToString();

            _isUncookSelected = command == "Uncook";

            if (Step4Block is not null)
                Step4Block.Visibility = _isUncookSelected ? Visibility.Visible : Visibility.Hidden;
        }

        private void RadioPattern_checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if ((bool)button.IsChecked)
                _isRegex = button.Content.ToString() == "Regex";
        }

        private void RadioArchive_checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if ((bool)button.IsChecked)
                _isArchiveFromGame = button.Content.ToString() == "Game's archive";
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            var archive = _isArchiveFromGame ? archives[ArchivesDropdown.SelectedIndex] : !string.IsNullOrEmpty(selectedArchive) ? selectedArchive : null;
            if (string.IsNullOrEmpty(archive))
            {
                guiConsole.logger.LogString("No archive indicated.", Logtype.Error);
                return;
            }

            var patternInput = PatternInput.Text;
            var regex = _isRegex ? patternInput : "";
            var pattern = !_isRegex ? patternInput : "";

            string[] pathList = { archive };

            if (command == "Unbundle")
            {
                Task.Run(() =>
                {
                    StartLoadBar();
                    ConsoleFunctions.UnbundleTask(pathList, null, null, pattern, regex);
                });
            } else if (command == "Uncook")
            {
                var ext = EUncookExtension.dds;
                switch (ExtensionsDropdown.SelectedItem)
                {
                    case "dds":
                        ext = EUncookExtension.dds; break;
                    case "tga":
                        ext = EUncookExtension.tga; break;
                    case "png":
                        ext = EUncookExtension.png; break;
                    case "jpeg":
                        ext = EUncookExtension.jpeg; break;
                    case "jpg":
                        ext = EUncookExtension.jpg; break;
                    case "bmp":
                        ext = EUncookExtension.bmp; break;
                    default:
                        ext = EUncookExtension.dds; break;
                }
                Task.Run(() =>
                {
                    StartLoadBar();
                    ConsoleFunctions.UncookTask(pathList, null, ext, false, 0, pattern, regex);
                });
            }

        }

        private void StartLoadBar()
        {
            guiConsole.logger.LogProgress(0);
            mainDispatcher.BeginInvoke((Action)(() =>
            {
                ExportLoadBar.Value = 0;
                DisplayLoadBar(true);
            }));
            timerLoadBar = new Timer(200);
            timerLoadBar.Elapsed += UpdateLoadBar;
            timerLoadBar.AutoReset = true;
            timerLoadBar.Enabled = true;
        }


        private void UpdateLoadBar(Object source, ElapsedEventArgs e)
        {
            if (guiConsole.logger.Progress is null)
                return;

            if (guiConsole.logger.Progress.Item1 == 1)
                timerLoadBar.Dispose();

            mainDispatcher.BeginInvoke((Action)(() =>
            {
                if (guiConsole.logger.Progress.Item1 == 1)
                    DisplayLoadBar(false);
                ExportLoadBar.Value = guiConsole.logger.Progress.Item1;
            }));
        }

        private void DisplayLoadBar(bool visibility)
        {
            ExportLoadBar.Visibility = visibility ? Visibility.Visible : Visibility.Hidden;
        }
    }

}
