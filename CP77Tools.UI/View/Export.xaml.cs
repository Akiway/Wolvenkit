using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WolvenKit.Common.DDS;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Export.xaml
    /// </summary>
    public partial class Export : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = Model.GUIConsole.Instance;

        private List<string> files = new List<string>();
        private List<string> filesName = new List<string>();
        private bool displayFullPath = false;
        private string[] extensions = { "dds", "tga", "png", "jpeg", "jpg", "bmp" };
        public Export()
        {
            InitializeComponent();

            ExtensionOption.ItemsSource = extensions;
        }

        private void updateFileList()
        {
            filesName.Clear();
            filesName.AddRange(files.ConvertAll(f => System.IO.Path.GetFileName(f)));
            PathListBadge.Badge = files.Count > 0 ? files.Count : null;
            ExportFileList.ItemsSource = new List<string>();
            ExportFileList.ItemsSource = displayFullPath ? files : filesName;
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
        private void ExportFolderList_FullPath_Click(object sender, RoutedEventArgs e)
        {
            displayFullPath = (bool)((CheckBox)sender).IsChecked;
            updateFileList();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1)
            {
                guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                return;
            }

            var ext = EUncookExtension.dds;
            switch (ExtensionOption.SelectedItem)
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
                ConsoleFunctions.ExportTask(files.ToArray(), ext);
            });
        }
    }
}
