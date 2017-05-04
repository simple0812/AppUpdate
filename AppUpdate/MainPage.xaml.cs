using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace AppUpdate
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        public void NavigateToPageWithParameter(int pageIndex, object parameter)
        {
            ScenarioFrame.Navigate(typeof(test), parameter);
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if(ApplicationData.Current.LocalSettings.Values.ContainsKey("PackageStatus"))
                        txtStatus.Text = ApplicationData.Current.LocalSettings.Values["PackageStatus"].ToString();
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("PackageLocation"))
                        txtLocation.Text = ApplicationData.Current.LocalSettings.Values["PackageLocation"].ToString();
                });

                await Task.Delay(200);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            txt.Text = Package.Current.Id.FamilyName;
        }
    }
}
