using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour Dump.xaml
    /// </summary>
    public partial class Dump : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = GUIConsole.Instance;

        private List<string> files = new List<string>();
        private List<string> filesName = new List<string>();
        private bool displayFullPath = false;
        public Dump()
        {
            InitializeComponent();
        }

        private void updateFileList()
        {
            filesName.Clear();
            filesName.AddRange(files.ConvertAll(f => Path.GetFileName(f)));
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
        private void FolderList_FullPath_Click(object sender, RoutedEventArgs e)
        {
            displayFullPath = (bool)((CheckBox)sender).IsChecked;
            updateFileList();
        }

        private void btnDump_Click(object sender, RoutedEventArgs e)
        {
            if (files.Count < 1)
            {
                guiConsole.logger.LogString("No archive indicated.", Logtype.Error);
                return;
            }

            bool imports = (bool)ImportsOption.IsChecked;
            bool missinghashes = (bool)MissingOption.IsChecked;
            bool texinfo = (bool)TexOption.IsChecked;
            bool classinfo = (bool)ClassOption.IsChecked;
            bool dump = (bool)DumpOption.IsChecked;
            bool list = (bool)ListOption.IsChecked;

            Task.Run(() =>
            {
                ConsoleFunctions.DumpTask(files.ToArray(), imports, missinghashes, texinfo, classinfo, dump, list);
            });
        }
    }
}
