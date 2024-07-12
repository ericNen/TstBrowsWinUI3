using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using WinRT;
using System.Runtime.InteropServices;
using Microsoft.UI.Composition;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TstBrowserWinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public bool IsSettingPageOpened { get; set; }
        public TabViewItem SettingPageTab { get; set; }
        public bool IsTabviewShown = true;
        public bool Fulled = false;
        public bool IsSettingPageOpend = false;
        public string CurrentDirPath = Windows.ApplicationModel.Package.Current.InstalledPath;
        public MenuFlyout DownloadFlyout;
        public MenuFlyout WebsiteChoosingOpt;
        public string PlaceholdWebsite;
        public string PlaceholdWebsiteUrl;
        public MenuFlyout MoreOptionFlyout;
        public TabViewListView tabViewListView;
        public string HomepageUrl;
        public string SearchEngineWebsite;

        public MainWindow()
        {
            this.InitializeComponent();
            WebsiteChoosingOpt = new();

            ExtendsContentIntoTitleBar = true;
            AppWindow CurrentWindow = AppWindow;

            MoreOptionFlyout = new() { Placement = FlyoutPlacementMode.Bottom};
            LoadMoreOpt();

            DownloadFlyout = new() { Placement = FlyoutPlacementMode.Bottom };

            WebsiteChoosingOpt = new() { Placement = FlyoutPlacementMode.Bottom};
            LoadWebsiteShortcut();

            WindowBackGroundControlling();

            SetTitleBar(RightDragArea);
            TabControl.CanDragTabs = true;
            TabControl.TabDragCompleted += delegate (TabView sender, TabViewTabDragCompletedEventArgs e) { };
            AddTab(TabControl, null);
            waitForFirstTab();
        }

        public static DesktopAcrylicController AcyController;
        private static SystemBackdropConfiguration BackdropConfiguration;

        async void waitForFirstTab()
        {
            while (tabViewListView == null)
            {
                await Task.Delay(500);
                TabViewItem FirstTab = (TabViewItem)TabControl.TabItems[0];
                tabViewListView = FirstTab.Parent as TabViewListView;
            }
        }

        public void WriteHistory(string Title,string Url,DateTime NavigationTime)
        {
            XmlDocument HistoryData = new();
            HistoryData.Load(@"Z:\BrowsingHistory.xml");
            string Year = DateTime.Today.Year.ToString();
            string Month = DateTime.Today.Month.ToString();
            string Day = DateTime.Today.Day.ToString();
            string ElementName = $"T_{Year}_{Month}_{Day}";
            XmlElement NewHistory = HistoryData.CreateElement(ElementName);
            NewHistory.SetAttribute("Title", Title);
            NewHistory.SetAttribute("url", Url);
            NewHistory.SetAttribute("Time", NavigationTime.ToString());
            HistoryData.GetElementsByTagName("HistoryList")[0].PrependChild(NewHistory);
            HistoryData.Save(@"Z:\BrowsingHistory.xml");
        }

        void LoadWebsiteShortcut()
        {
            WebsiteChoosingOpt.Items.Clear();
            XmlDocument XmlData = new();
            XmlData.Load(CurrentDirPath + "\\Assets\\XMLData.xml");
            XmlNodeList a = XmlData.GetElementsByTagName("WebsiteList")[0].ChildNodes;
            HomepageUrl = XmlData.GetElementsByTagName("HomePage")[0].InnerText;
            PlaceholdWebsite = XmlData.GetElementsByTagName("PlaceholdWebsite")[0].Attributes["name"].Value;
            PlaceholdWebsiteUrl = XmlData.GetElementsByTagName("PlaceholdWebsite")[0].InnerText;
            SearchEngineWebsite = XmlData.GetElementsByTagName("SearchEngine")[0].InnerText;

            foreach (XmlNode item in a)
            {
                Uri WebsiteUri = new(item.Attributes["url"].Value);
                string FavIconPath = WebsiteUri.Scheme + Uri.SchemeDelimiter + WebsiteUri.Host + "/favicon.ico";
                MenuFlyoutItem b = new()
                {
                    Name = item.Attributes["url"].Value,
                    Text = item.Name,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = new AcrylicBrush() { Opacity = 0.3, FallbackColor = Colors.Orange, TintColor = Colors.Red, TintOpacity = 0.5 },
                    Icon = new BitmapIcon { UriSource = new Uri(FavIconPath), ShowAsMonochrome = false }
                };
                ToolTip toolTip = new()
                {
                    Content = item.Attributes["url"].Value
                };
                ToolTipService.SetToolTip(b, toolTip);
                WebsiteChoosingOpt.Items.Add(b);
            }
        }
        
        private void LoadMoreOpt()
        {
            MenuFlyoutItem FullScreen = new()
            {
                Icon = new FontIcon() { Glyph = "\uE740" },
                Text = "Full screen"
            };
            FullScreen.Click += (sender,arg)=> { ToFullScreen(tabViewListView); };
            MoreOptionFlyout.Items.Add(FullScreen);
            MenuFlyoutItem Setting = new()
            {
                Icon = new SymbolIcon(Symbol.Setting),
                Text = "Setting"
            };
            Setting.Click += (sender,arg)=> { OpenDesiredNewTab("Setting"); };
            MoreOptionFlyout.Items.Add(Setting);
        }
        
        public void WindowBackGroundControlling()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                AcyController = new()
                {
                    Kind = DesktopAcrylicKind.Thin,
                    TintOpacity = 0,
                    LuminosityOpacity = 0,
                    TintColor = Colors.Transparent,
                    FallbackColor = Colors.LightCyan,
                };
                BackdropConfiguration = new();
                
                AcyController.AddSystemBackdropTarget(this.As< ICompositionSupportsSystemBackdrop >());
                AcyController.SetSystemBackdropConfiguration(BackdropConfiguration);
                AcrylicBrush acrylicBrush = new() { TintOpacity = 0.7, Opacity = 0.5, 
                    TintColor = Colors.Green, FallbackColor = Colors.Transparent,               
                };
                TabControl.Background = acrylicBrush;
            }
        }
        
        public void AddTab(TabView Tabview, object arg)
        {
            Frame TabFrame = new() {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name="Normal"
            };
            TabViewItem NewTab = new()
            {
                Header = "New",
                Content = TabFrame
            };
            TabControl.TabItems.Add(NewTab);
            TabFrame.Navigate(typeof(TabPage));
            TabControl.SelectedItem = NewTab;
        }

        public void OpenDesiredNewTab(string Arg)
        {
            Frame TabFrame = new()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name = Arg
            };
            TabViewItem NewTab = new()
            {
                Content = TabFrame
            };
            /*
            if (Arg == "Setting")
            {
                NewTab.Header = "Setting"; 
                NewTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Setting }; 
                TabFrame.Navigate(typeof(SettingPage));
                IsSettingPageOpened = true;
                NewTab.CloseRequested += (tabview, args) => { IsSettingPageOpened = false; };
            }
            
            else {*/
            NewTab.Header = "New"; TabFrame.Navigate(typeof(TabPage));
            TabControl.TabItems.Add(NewTab);
            TabControl.SelectedItem = NewTab;
        }

        public void CloseTab(TabView Tabview,TabViewTabCloseRequestedEventArgs arg)
        {
            try
            {
                Tabview.TabItems.Remove(arg.Tab);
            } catch(Exception e) { Console.WriteLine(e); }
        }

        public void ToFullScreen(TabViewListView MainTabviewListView)
        {
            if (Fulled)
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                TitleBarButton.Visibility = Visibility.Visible;
                MainTabviewListView.Visibility = Visibility.Visible;
                TabControl.IsAddTabButtonVisible = true;
                Debug.WriteLine(MainTabviewListView.Name);
                Fulled = false;
            }
            else
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                TitleBarButton.Visibility = Visibility.Collapsed;
                MainTabviewListView.Visibility = Visibility.Collapsed;
                TabControl.IsAddTabButtonVisible = false;
                Debug.WriteLine(MainTabviewListView.Name);
                Fulled = true;
            }
        }

        public void WebviewFullScreen(bool IsBrowserFull)
        {
            if (IsBrowserFull)
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
                TitleBarButton.Visibility = Visibility.Collapsed;
                tabViewListView.Visibility = Visibility.Collapsed;
                TabControl.IsAddTabButtonVisible = false;
            }
            else
            {
                AppWindow.SetPresenter(AppWindowPresenterKind.Default);
                TitleBarButton.Visibility = Visibility.Visible;
                tabViewListView.Visibility = Visibility.Visible;
                TabControl.IsAddTabButtonVisible = true;
            }
        }

        private void TitleBarButton_Click(object sender, RoutedEventArgs e)
        {
            using (Process OpenXML = new())
            {
                OpenXML.StartInfo.FileName = CurrentDirPath + "\\Assets\\XMLData.xml";
                OpenXML.StartInfo.UseShellExecute = true;
                OpenXML.Start();
            }
            using (Process OpenHistoryXML = new())
            {
                OpenHistoryXML.StartInfo.FileName = CurrentDirPath + "\\Assets\\BrowsingHistory.xml";
                OpenHistoryXML.StartInfo.UseShellExecute = true;
                OpenHistoryXML.Start();
            }
            LoadWebsiteShortcut();
        }
    }
}
