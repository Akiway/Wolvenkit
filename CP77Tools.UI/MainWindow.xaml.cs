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
        public MainWindow()
        {
            InitializeComponent();

            // TODO list
            // * oodle dll import indicator (in the main bar)
            // * button to open folder in the extracted folder
            // * Possibility to extract from multiples archives

            ServiceLocator.Default.RegisterType<ILoggerService, LoggerService>();
            ServiceLocator.Default.RegisterType<IHashService, HashService>();

            var logger = ServiceLocator.Default.ResolveType<ILoggerService>();
            var hashService = ServiceLocator.Default.ResolveType<IHashService>();
            logger.OnStringLogged += delegate (object? sender, LogStringEventArgs args)
            {
                switch (args.Logtype)
                {

                    case Logtype.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case Logtype.Important:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case Logtype.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case Logtype.Normal:
                    case Logtype.Wcc:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Debug.WriteLine("[" + args.Logtype + "]" + args.Message);
                PrintConsole("[" + args.Logtype + "]" + args.Message);
            };

            //await ConsoleFunctions.UpdateHashesAsync();
            hashService.ReloadLocally();

            // try get oodle dll from game
            if (!General.TryCopyOodleLib())
            {
                logger.LogString("Could not automatically find oo2ext_7_win64.dll. " +
                                 "Please manually copy and paste the dll found here Cyberpunk 2077\\bin\\x64\\oo2ext_7_win64.dll into this folder: " +
                                 $"{AppDomain.CurrentDomain.BaseDirectory}.");
            }


        }

        public void PrintConsole(string text)
        {
            ConsoleOutput.Text += text + '\r';
        }

        public void ClearConsole()
        {
            ConsoleOutput.Text += "";
        }
    }
}
