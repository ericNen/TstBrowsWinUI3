using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Xml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.System;
using System.IO;
using System.Web;

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
            Setup();
            UrlBar.Background = new SolidColorBrush() { Color= Colors.White };
            fullScreen.Icon = new FontIcon() { Glyph = "\uE740"};
            ExecuteScript.Icon = new FontIcon() { Glyph = "\uE943"};
            ShowSideBar.Content = new FontIcon() { Glyph = "\uE76B"};
            BrowserControl();
            UrlBar.GotFocus += UrlBar_Clicked;
        }

        private TabViewItem CurrentTabViewItem;
        private TabViewListView MainTabviewListView;
        private string javascript1;
        private string javascript2;
        private bool IsNew = true;

        private async void Setup()
        {
            await Browser1.EnsureCoreWebView2Async();

            CurrentTabViewItem = (TabViewItem)Frame.Parent;
            
            MoreOption.Flyout = App.m_window.MoreOptionFlyout;

            

            WebsiteChoosing.Content = App.m_window.PlaceholdWebsite;

            if (Frame.Name != "Normal")
            {
                IsNew = false;
                NavigateUrl(Frame.Name);
            }
            else
            {
                NavigateUrl(App.m_window.HomepageUrl);
                UrlBar.Focus(FocusState.Programmatic);
            }

            Downloads.Flyout = App.m_window.DownloadFlyout;

            WebsiteChoosing.Flyout = App.m_window.WebsiteChoosingOpt;
            var a = App.m_window.WebsiteChoosingOpt.Items;
            foreach (MenuFlyoutItem item in a)
            {
                item.Click += delegate (object sender, RoutedEventArgs args) {
                    if (CurrentTabViewItem == App.m_window.tabViewListView.Items[App.m_window.tabViewListView.SelectedIndex] as TabViewItem)
                    {
                        NavigateUrl(item.Name);
                    }
                };
            }
        }

        #region Browser
        async void BrowserControl()
        {
            javascript1 = File.ReadAllText(@"D:\Eric\testAddon.js");
            javascript2 = File.ReadAllText(@"D:\Eric\FindElement.js");
            await Browser1.EnsureCoreWebView2Async();

            Browser1.CoreWebView2.Profile.PreferredTrackingPreventionLevel = CoreWebView2TrackingPreventionLevel.Strict;
            Browser1.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
            Browser1.CoreWebView2.NewWindowRequested += NewWindowRequested;
            Browser1.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            Browser1.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            Browser1.CoreWebView2.NavigationStarting += WebviewNavigating;
            Browser1.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
            Browser1.CoreWebView2.DocumentTitleChanged += CoreWebView2_TitleChanged;
            Browser1.CoreWebView2.ContainsFullScreenElementChanged += FullScreen;
            Browser1.CoreWebView2.FrameCreated += IFrameCreated;
            Browser1.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
            Browser1.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;

            Browser1.CoreWebView2.AddWebResourceRequestedFilter("*",CoreWebView2WebResourceContext.All);
            Browser1.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;

            await Browser1.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(javascript2);
        }

        private void CoreWebView2_HistoryChanged(CoreWebView2 sender, object args)
        {
            if (Browser1.CanGoBack)
            {
                BackBut.IsEnabled = true;
            }
            else
            {
                BackBut.IsEnabled = false;
            }

            if (Browser1.CanGoForward)
            {
                ForwardBut.IsEnabled = true;
            }
            else
            {
                ForwardBut.IsEnabled = false;
            }
        }

        private void CoreWebView2_WebResourceResponseReceived(CoreWebView2 sender, CoreWebView2WebResourceResponseReceivedEventArgs args)
        {
            /*
            if (args.Request.Uri == Browser1.Source.ToString())
            {
                string DocumentTitle = await Browser1.CoreWebView2.ExecuteScriptAsync("document.title");
                CurrentTabViewItem.Header = DocumentTitle;
            }*/
        }

        private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            MenuFlyoutItem New = new() { Text = args.DownloadOperation.Uri };
            App.m_window.DownloadFlyout.Items.Add(New);
        }

        private void CoreWebView2_ProcessFailed(CoreWebView2 sender, CoreWebView2ProcessFailedEventArgs args)
        {
            OutPut.Text = "Failed";
        }

        private async void WebviewNavigating(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (!IsNew)
            {
                UrlBar.Text = sender.Source.ToString();
            }
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            if (Browser1.CoreWebView2.FaviconUri != string.Empty && Browser1.CoreWebView2.FaviconUri != null)
            {
                CurrentTabViewItem.IconSource = new BitmapIconSource
                {
                    UriSource = new Uri(Browser1.CoreWebView2.FaviconUri),
                    ShowAsMonochrome = false,
                };
            }
            else
            {
                CurrentTabViewItem.IconSource = new FontIconSource() { Glyph = "\uE783" };
            }
        }

        private void CoreWebView2_TitleChanged(CoreWebView2 sender, object args)
        {
            CurrentTabViewItem.Header = Browser1.CoreWebView2.DocumentTitle;
            if (!IsNew)
            {
                UrlBar.Text = sender.Source;
            }
        }

        private async void IFrameCreated(CoreWebView2 sender, CoreWebView2FrameCreatedEventArgs arg) {
            await sender.ExecuteScriptAsync(javascript2);
        }

        private void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (!IsNew)
            {
                UrlBar.Text = sender.Source;
                Browser1.Focus(FocusState.Programmatic);
                if (Browser1.Source.AbsoluteUri != App.m_window.HomepageUrl)
                {
                    Debug.WriteLine($"\"{Browser1.CoreWebView2.DocumentTitle}\"\"{Browser1.Source.AbsoluteUri}\" has been written into history");
                    App.m_window.WriteHistory(Browser1.CoreWebView2.DocumentTitle, Browser1.Source.AbsoluteUri, DateTime.Now);
                }
            }
            else
            {
                IsNew = false;
            }

            if (args.HttpStatusCode != 200)
            {
                CurrentTabViewItem.IconSource = new FontIconSource() { Glyph = "\uE783" };
            }
        }

        async void NavigateUrl(string url)
        {
            Browser1.CoreWebView2.Navigate(url);
            if (!url.StartsWith("https://"))
            {
                await Browser1.ExecuteScriptAsync($"alert('{url}' is not secured)");
            }
        }

        void NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs e) {
            App.m_window.OpenDesiredNewTab(e.Uri);
            e.Handled = true;
        }

        #endregion
        #region UrlBar
        private void UrlBar_Clicked(object sender, RoutedEventArgs arg)
        {
            UrlBar.SelectAll();
        }

        private void UrlBar_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Debug.WriteLine("Entered");
                if (UrlBar.Text.StartsWith("https://") || UrlBar.Text.StartsWith("http://"))
                {
                    NavigateUrl(UrlBar.Text);
                }else {
                    string ProcessedString = HttpUtility.UrlEncode(UrlBar.Text);
                    string NewString = ProcessedString.Replace(' ', '+'); ;
                    NavigateUrl("https://www.bing.com/search?q=" + NewString); 
                }
                Browser1.Focus(FocusState.Programmatic);
            }
        }
        #endregion

        #region AppBar & AppBar events

        private void Back(object sender, RoutedEventArgs e)
        {
            Browser1.GoBack();
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            Browser1.GoForward();
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            Browser1.CoreWebView2.Reload();
        }

        private bool IsNavigationStoped = false;
        private void StartPauseNavigation(object sender, RoutedEventArgs e)
        {
            if (IsNavigationStoped)
            {
                //Browser1.CoreWebView2.Resume();
                //Browser1.CoreWebView2.Settings.IsScriptEnabled = true;
                StartStopNavigation.Icon = new SymbolIcon(Symbol.Pause) {Foreground = new SolidColorBrush(Colors.Red) };
                OutPut.Text = "Resume";
                IsNavigationStoped = false;
            }
            else
            {
                //Browser1.CoreWebView2.Stop();
                //Browser1.CoreWebView2.Settings.IsScriptEnabled = false;
                StartStopNavigation.Icon = new SymbolIcon(Symbol.Play) { Foreground = new SolidColorBrush(Colors.Green) };
                OutPut.Text = "Stop";
                IsNavigationStoped= true;
            }
        }

        private async void RunJavascript(object sended, RoutedEventArgs e)
        {
            javascript1 = File.ReadAllText(@"D:\Eric\testAddon.js");
            await Browser1.CoreWebView2.ExecuteScriptAsync(javascript1);
        }

        private void WebsiteChoosing_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            NavigateUrl(App.m_window.PlaceholdWebsiteUrl);
        }
        
        private void MoreOption_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem New = new() { Text = "aaa" };
            App.m_window.DownloadFlyout.Items.Add(New);
        }

        private void AddPage(object sender, RoutedEventArgs e)
        {
            App.m_window.AddTab(null, null);
        }

        private void OpenSettingPage(object sender, RoutedEventArgs e)
        {
            if (App.m_window.IsSettingPageOpened == false)
            {
                App.m_window.OpenDesiredNewTab("Setting");
            }
        }

        private void Full_Screen(object sender, RoutedEventArgs e)
        {
            App.m_window.ToFullScreen(MainTabviewListView);
        }

        #endregion

        bool IsFullScreen = false;
        private void FullScreen(CoreWebView2 Browser,object obj)
        {
            if (IsFullScreen) 
            {
                App.m_window.WebviewFullScreen(false);
                //MainColumn0.Width = new GridLength(180);
                TopBar.Height = new GridLength(50);
                IsFullScreen = false;
            }
            else
            {
                App.m_window.WebviewFullScreen(true);
                //MainColumn0.Width = new GridLength(0);
                TopBar.Height = new GridLength(0);
                IsFullScreen = true;
            }
        }

        private void ShowSideBarL(object sender, RoutedEventArgs arg)
        {
            MainColumn0.Width = new GridLength(0);
        }

        
        private void MouseNearLeftBorder(PointerRoutedEventArgs arg)
        {

        }
    }
}
