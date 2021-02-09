using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System.Windows.Threading;
using WolvenKit.Common.Services;
using CP77Tools.Tasks;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Oodle.xaml
    /// </summary>
    public partial class Oodle : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = GUIConsole.Instance;

        private List<string> files = new List<string>();
        private List<string> filesName = new List<string>();
        private bool displayFullPath = false;

        public Oodle()
        {
            InitializeComponent();
        }

        private void updateFileList()
        {
            filesName.Clear();
            filesName.AddRange(files.ConvertAll(f => System.IO.Path.GetFileName(f)));
            PathListBadge.Badge = files.Count > 0 ? files.Count : null;
            FileList.ItemsSource = new List<string>();
            FileList.ItemsSource = displayFullPath ? files : filesName;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files|*";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                files.Clear();
                files.Add(openFileDialog.FileName);
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
            displayFullPath = (bool)((CheckBox)sender).IsChecked;
            updateFileList();
        }

        private void btnOodle_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1)
            {
                guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (Convert.ToBoolean(ConsoleFunctions.OodleTask(files[0], null, true)))
                        guiConsole.logger.LogString("File decompressed successfuly.", Logtype.Success);
                    else
                        guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                } catch
                {
                    guiConsole.logger.LogString("This file doesn't support oodle decompression.", Logtype.Error);
                }
            });
        }
    }
}
