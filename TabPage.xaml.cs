using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using Microsoft.UI.Input;
using System.Threading;
using System.Xml;
using Windows.Storage.AccessCache;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TstBrowserWinUI3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabPage : Page
    {
        public TabPage()
        {
            this.InitializeComponent();
            AddWebsiteShortcut();
            BrowserControl();
        }

        private void AddWebsiteShortcut()
        {
            WebsiteChoosing.Content = "Choose";
            Flyout WebsiteChoosingOpt = new();
            StackPanel Panel = new();
            string CurrentDirPath = Windows.ApplicationModel.Package.Current.InstalledPath;
            XmlDocument XmlData = new XmlDocument();
            //XmlData.Load(Environment.CurrentDirectory.FirstOrDefault())

            foreach (string item in Directory.EnumerateFiles(CurrentDirPath + "/Assets","*.xml"))
            {
                Button b = new()
                {
                    Content = item.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Panel.Children.Insert(Panel.Children.Count,b);
            }
            WebsiteChoosingOpt.Content = Panel;
            /*
            Button button = new() {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Colors.Gray)
            };
            WebsiteChoosingOpt.Content = button; 
            */
            WebsiteChoosing.Flyout = WebsiteChoosingOpt;
        }
        #region Browser
        async void BrowserControl()
        {
            await Browser1.EnsureCoreWebView2Async();
            Browser1.CoreWebView2.NewWindowRequested += NewWindowRequested;
            Browser1.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            Thread.Sleep(100);
            Debug.WriteLine($"{this.Name} : {this.Frame}");
            try{
                this.Frame.Name = "aaaal";
            }catch(Exception ex) { Debug.WriteLine(ex.ToString()); }
        }

        private async void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            await Browser1.EnsureCoreWebView2Async();
            Frame.Name = Browser1.CoreWebView2.DocumentTitle;
            UrlBar.Text = Browser1.Source.AbsoluteUri;
        }

        async void NavigateUrl(string url)
        {
            await Browser1.EnsureCoreWebView2Async();
            try
            {
                Browser1.CoreWebView2.Navigate(url);
            }catch (Exception) { Debug.WriteLine($"Url: \"{url}\" incorrect"); }
        }
        void NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs e) {
            e.NewWindow = sender;
        }
        #endregion
        #region UrlBar
        private void UrlBar_Clicked(object sender, EventArgs e)
        {

        }

        private void UrlBar_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Debug.WriteLine("Entered");
                if (UrlBar.Text.StartsWith("https"))
                {
                    NavigateUrl(UrlBar.Text);
                }else {
                    string ProcessedString = UrlBar.Text.Replace(' ', '+');
                    NavigateUrl("https://www.bing.com/search?q=" + ProcessedString); 
                }
            }
        }
        #endregion
        #region AppBar & AppBar events
        private async void Back(object sender, RoutedEventArgs e)
        {
            await Browser1.EnsureCoreWebView2Async();
            Browser1.GoBack();
        }

        private async void Forward(object sender, RoutedEventArgs e)
        {
            await Browser1.EnsureCoreWebView2Async();
            Browser1.GoForward();
        }

        private async void Reload(object sender, RoutedEventArgs e)
        {
            await Browser1.EnsureCoreWebView2Async();
            Browser1.CoreWebView2.Reload();
        }

        private async void RunJavascript(object sended, RoutedEventArgs e)
        {
            await Browser1.EnsureCoreWebView2Async();
            await Browser1.CoreWebView2.ExecuteScriptAsync("document.title='lol';alert(document.title);");
            Frame.Name = Browser1.CoreWebView2.DocumentTitle;
        }

        private void WebsiteChoosing_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            /*
            WebsiteChoosing.Opacity = 1;
            WebsiteChoosing.Background = new SolidColorBrush(Colors.DarkBlue);
            OutPut.Text = "Entered";
            WebsiteChoosing.PointerExited += delegate (object sender, PointerRoutedEventArgs e) {
                WebsiteChoosing.Opacity = 1;
                WebsiteChoosing.Background = new SolidColorBrush(Colors.DarkSlateGray);
                OutPut.Text = string.Empty;
            };
            */
        }

        private int count = 0;
        private void WebsiteChoosing_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            count++;
            OutPut.Text=count.ToString();
        }

        #endregion
    }
}
