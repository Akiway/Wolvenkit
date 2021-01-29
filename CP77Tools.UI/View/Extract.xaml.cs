using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using CP77Tools.Tasks;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Extract.xaml
    /// </summary>
    public partial class Extract : UserControl
    {
        private string[] archives;
        private string[] extensions = { "dds", "tga", "png", "jpg", "bmp" };
        
        private static string command = "Unbundle";
        private static string selectedArchive;

        private bool _isUncookSelected = false;
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
        }

        private void btnExtract_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearConsole();
            if (string.IsNullOrEmpty(selectedArchive))
                selectedArchive = archives[ArchivesDropdown.SelectedIndex];

            var pattern = PatternInput.Text;
            var regex = RegexInput.Text;
            string[] pathList = { selectedArchive };

            if (command == "Unbundle")
            {
                ConsoleFunctions.UnbundleTask(pathList, null, null, pattern, regex);
            } else if (command == "Uncook")
            {
                ConsoleFunctions.UncookTask(pathList, null, WolvenKit.Common.DDS.EUncookExtension.dds, false, 0, pattern, regex);
            }

        }


    }
}
