using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;
using muxc = Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TstBrowserWinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(RightDragArea);
            TabControl.CanDragTabs = true;
            TabControl.TabDragCompleted += delegate (TabView sender, TabViewTabDragCompletedEventArgs e) { };
            AddTab(TabControl, null);

        }
        
        public void AddTab(TabView Tabview, object arg)
        {
            Frame TabFrame = new() {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name="Frame"
            };
            TabFrame.Navigate(typeof(TabPage),null,
                new DrillInNavigationTransitionInfo());
            TabViewItem NewTab = new()
            {
                Content = TabFrame,
                Header = "New",
            };
            Tabview.TabItems.Add(NewTab);
            ReT();
        }

        public void CloseTab(TabView Tabview,TabViewTabCloseRequestedEventArgs arg)
        {
            try
            {
                Tabview.TabItems.Remove(arg.Tab);
            } catch(Exception e) { Console.WriteLine(e); }
        }

        public void ReT()
        {
            int selectedTabIndex = this.TabControl.SelectedIndex;
            TabViewItem CurrentTab = (TabViewItem)this.TabControl.TabItems[selectedTabIndex];
            var c = (Frame)CurrentTab.Content;
            if (CurrentTab != null) { 
                CurrentTab.Header = c.Name;
            }
            else
            {
                Debug.WriteLine("F");
            }
        }
        
    }
}
