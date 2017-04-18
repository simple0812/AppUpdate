using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace DashboardApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }


        private AppServiceConnection updaterService;
        private async void UIElement_OnTapped(object sender, RoutedEventArgs e)
        {
            if (this.updaterService == null)
            {
                this.updaterService = new AppServiceConnection();
                this.updaterService.AppServiceName = "net.hoekstraonline.appupdater";
                this.updaterService.PackageFamilyName = "189703c0-3a5c-47dd-a12f-af8b2ed78d2d_wbwwzgv8bqypp";

                var status = await this.updaterService.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    txtOutput.Text = "Failed to connect!";
                    updaterService = null;
                    return;
                }
                else
                {
                    txtOutput.Text = "connected!";
                }
            }
            try
            {
//                Uri updatePackage = new Uri(@"H:\AppUpdate\DashboardApp\AppPackages\DashboardApp_1.0.0.0_Debug_Test\DashboardApp_1.0.0.0_x86_x64_Debug.appxbundle");
                Uri updatePackage = new Uri($"http://10.0.0.8:8080/DashboardApp_{txtVersion.Text}_Debug_Test/DashboardApp_{txtVersion.Text}_x64_Debug.appxbundle");
                var message = new ValueSet();
                message.Add("PackageFamilyName", Windows.ApplicationModel.Package.Current.Id.FamilyName);
                message.Add("PackageLocation", updatePackage.ToString());

                AppServiceResponse response = await this.updaterService.SendMessageAsync(message);

                if (response.Status == AppServiceResponseStatus.Success)
                {
                    txtOutput.Text = "Update started, time to say goodbye" + response.Message["Status"];

                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.Message;
            }

        }
    }
}
