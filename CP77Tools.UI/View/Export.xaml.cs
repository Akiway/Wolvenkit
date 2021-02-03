using CP77.CR2W;
using CP77Tools.Tasks;
using CP77Tools.UI.Model;
using Microsoft.Win32;
using System;
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

        private string file = "";
        private string[] extensions = { "dds", "tga", "png", "jpeg", "jpg", "bmp" };
        public Export()
        {
            InitializeComponent();

            ExtensionOption.ItemsSource = extensions;
        }
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*)|*";
            if (openFileDialog.ShowDialog() == true)
            {
                file = openFileDialog.FileName;
                TextFileSelected.Text = file;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(file))
            {
                guiConsole.logger.LogString("No file indicated.", Logtype.Error);
                return;
            }

            string[] fileList = { file };

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
                ConsoleFunctions.ExportTask(fileList, ext);
            });
        }
    }
}
