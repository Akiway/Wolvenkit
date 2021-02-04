using CP77.CR2W;
using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using WolvenKit.Common.DDS;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.View
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class Export : UserControl
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private GUIConsole guiConsole = Model.GUIConsole.Instance;

        private System.Timers.Timer timerLoadBar;

        private List<string> files = new List<string> { };
        private string[] extensions = { "dds", "tga", "png", "jpeg", "jpg", "bmp" };
        public Export()
        {
            InitializeComponent();

            ExtensionOption.ItemsSource = extensions;
        }
        // TODO: ExportFileList Scroll to much

        private void updateFileList()
        {
            ExportFileList.ItemsSource = new List<string> { };
            ExportFileList.ItemsSource = files;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            guiConsole.logger.LogString(files.Count.ToString(), Logtype.Error);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files|*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                files.AddRange(openFileDialog.FileNames);
                updateFileList();
            }
        }

        private void btnRemovefile_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string valueToRemove = (((Grid)button.Parent).Children[0] as TextBlock).Text;
            files.RemoveAt(files.IndexOf(valueToRemove));
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
