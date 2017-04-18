using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;

namespace AppUpdaterService
{
    public sealed class BackGroundUpdaterTask : IBackgroundTask
    {
        BackgroundTaskDeferral serviceDeferral;
        AppServiceConnection connection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            //initialize my stuff

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details != null) connection = details.AppServiceConnection;

            //Listen for incoming app service requests
            connection.RequestReceived += OnRequestReceived;
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            serviceDeferral?.Complete();
        }

        async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Debug.Write("........");
            //Get a deferral so we can use an awaitable API to respond to the message
            var messageDeferral = args.GetDeferral();

            ValueSet message = args.Request.Message;
            ValueSet returnData = new ValueSet();

            try
            {
                string packageFamilyName = message["PackageFamilyName"] as string;
                string packageLocation = message["PackageLocation"] as string;
                PackageManager packagemanager = new PackageManager();
                var x =
                    await
                        packagemanager.UpdatePackageAsync(new Uri(packageLocation), null,
                            DeploymentOptions.ForceApplicationShutdown);

                Debug.Write(x.ErrorText + "," + x.ActivityId.ToString());
                //Don't need to send anything back since the app is killed during updating but you might want this if you ask to update another app instead
                //of yourself.

//                returnData.Add("Status", "OK");
//                await args.Request.SendResponseAsync(returnData);
            }
            catch (Exception)
            {
                //
            }
            finally
            {
                //Complete the message deferral so the platform knows we're done responding
                messageDeferral.Complete();
            }
        }
    }
}
