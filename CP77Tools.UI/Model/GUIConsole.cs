using Catel.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.Model
{
    public sealed class GUIConsole
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private static GUIConsole instance = new GUIConsole();

        static GUIConsole()
        {
        }

        public static GUIConsole Instance
        {
            get { return instance; }
        }

        private GUIConsole()
        {
            ServiceLocator.Default.RegisterType<ILoggerService, LoggerService>();
            logger = ServiceLocator.Default.ResolveType<ILoggerService>();

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
                Print("[" + args.Logtype + "]" + args.Message);
            };
        }

        public TextBlock output;
        public ScrollViewer scrollOutput;

        public ILoggerService logger;

        private void Print(string text)
        {
            try
            {
                if (Dispatcher.CurrentDispatcher == mainDispatcher)
                {
                    output.Text += text + '\r';
                    scrollOutput.ScrollToBottom();
                }
                else
                {
                    mainDispatcher.BeginInvoke((Action)(() =>
                    {
                        output.Text += text + '\r';
                        scrollOutput.ScrollToBottom();
                    }));
                }
            } catch (Exception e)
            {

            }
        }

        public void Clear()
        {
            output.Text += "";
        }
    }
}
