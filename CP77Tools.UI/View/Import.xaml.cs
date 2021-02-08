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
using static CP77.CR2W.Types.Enums;
using CP77Tools.Tasks;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Import.xaml
    /// </summary>
    public partial class Import : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = GUIConsole.Instance;

        private List<string> files = new List<string>();
        private List<string> filesName = new List<string>();
        private bool displayFullPath = false;

        public Import()
        {
            InitializeComponent();

            TextureGroupOption.ItemsSource = Enum.GetValues(typeof(GpuWrapApieTextureGroup));
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
            displayFullPath = (bool)((CheckBox)sender).IsChecked;
            updateFileList();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1)
            {
                guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                return;
            }

            bool vflip = (bool)FlipOption.IsChecked;
            var textureGroup = (GpuWrapApieTextureGroup)TextureGroupOption.SelectedItem;

            Task.Run(() =>
            {
                ConsoleFunctions.ImportTask(files.ToArray(), textureGroup, vflip);
            });
        }
    }
}
