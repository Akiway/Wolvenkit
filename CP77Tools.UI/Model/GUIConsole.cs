using Catel.IoC;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using WolvenKit.Common.Services;

namespace CP77Tools.UI.Model
{
    public sealed class GUIConsole
    {
        private static Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;

        private static GUIConsole instance = new GUIConsole();

        public StackPanel output;
        public ScrollViewer scrollOutput;

        public ILoggerService logger;

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
                mainDispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.TextWrapping = TextWrapping.Wrap;

                    switch (args.Logtype)
                    {
                        case Logtype.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            textBlock.Foreground = Brushes.Red;
                            break;
                        case Logtype.Important:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            textBlock.Foreground = Brushes.Yellow;
                            break;
                        case Logtype.Success:
                            Console.ForegroundColor = ConsoleColor.Green;
                            textBlock.Foreground = Brushes.Green;
                            break;
                        case Logtype.Normal:
                        case Logtype.Wcc:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Debug.WriteLine("[" + args.Logtype + "]" + args.Message);
                    textBlock.Text = "[" + args.Logtype + "]" + args.Message;


                    Print(textBlock);
                }));
            };
        }

        private void Print(TextBlock text)
        {
            output.Children.Add(text);
            scrollOutput.ScrollToBottom();
        }

        public void Clear()
        {
            output.Children.Clear();
        }
    }
}
