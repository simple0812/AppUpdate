using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;

namespace BackgroundProcess
{
    class Program
    {
        static AppServiceConnection connection = null;

        /// <summary>
        /// Creates an app service thread
        /// </summary>
        static void Main(string[] args)
        {
            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*****************************");
            Console.WriteLine("**** Classic desktop app ****");
            Console.WriteLine("*****************************");
            Console.ReadLine();
        }

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static void ThreadProc()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "CommunicationService";
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            var ax = connection.OpenAsync();
            
            ax.Completed = (info, asyncStatus) =>
            {
                AppServiceConnectionStatus status  = info.GetResults();
                switch (status)
                {
                    case AppServiceConnectionStatus.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Connection established - waiting for requests");
                        Console.WriteLine();
                        break;
                    case AppServiceConnectionStatus.AppNotInstalled:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The app AppServicesProvider is not installed.");
                        return;
                    case AppServiceConnectionStatus.AppUnavailable:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The app AppServicesProvider is not available.");
                        return;
                    case AppServiceConnectionStatus.AppServiceUnavailable:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName);
                        return;
                    case AppServiceConnectionStatus.Unknown:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                        return;
                }
            };

        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string key = args.Request.Message.First().Key;
            string value = args.Request.Message.First().Value.ToString();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Received message '{0}' with value '{1}'", key, value);
            if (key == "request")
            {
                ValueSet valueSet = new ValueSet();
                valueSet.Add("response", value.ToUpper());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Sending response: '{0}'", value.ToUpper());
                Console.WriteLine();
                args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            }
        }
    }
}
