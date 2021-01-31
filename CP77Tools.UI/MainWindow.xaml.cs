using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CP77Tools.UI.Model;
using CP77Tools;
using System.Diagnostics;
using Catel.IoC;
using WolvenKit.Common.Oodle;
using WolvenKit.Common.Services;

namespace CP77Tools.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GUIConsole guiConsole = Model.GUIConsole.Instance;
        public MainWindow()
        {

            InitializeComponent();

            // TODO list
            // ? oodle dll import indicator (in the main bar)
            // * button to open folder in the extracted folder
            // * Possibility to extract from multiples archives

            guiConsole.output = ConsoleOutput;
            guiConsole.scrollOutput = ConsoleOutputScroll;

            ServiceLocator.Default.RegisterType<IHashService, HashService>();
            var hashService = ServiceLocator.Default.ResolveType<IHashService>();

            //await ConsoleFunctions.UpdateHashesAsync();

            Task.Run(() =>
            {
                hashService.ReloadLocally();
            });

            // try get oodle dll from game
            if (!General.TryCopyOodleLib())
                guiConsole.logger.LogString("Could not automatically find oo2ext_7_win64.dll. " +
                                            "Please manually copy and paste the dll found here Cyberpunk 2077\\bin\\x64\\oo2ext_7_win64.dll into this folder: " +
                                            $"{AppDomain.CurrentDomain.BaseDirectory}.");


        }
    }
}
