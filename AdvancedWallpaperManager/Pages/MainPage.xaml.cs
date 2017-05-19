using GalaSoft.MvvmLight.Ioc;
using KillerrinStudiosToolkit.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AdvancedWallpaperManager.Models.Enums;
using AdvancedWallpaperManager.Services;
using AdvancedWallpaperManager.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdvancedWallpaperManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance;
        public MainViewModel ViewModel { get { return (MainViewModel)DataContext; } }

        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up any services which require multiple controls
            RegisterSplitViewService();

            // Finally, send the loading event
            ViewModel.Loaded();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.OnNavigatedTo();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.OnNavigatedFrom();
            base.OnNavigatedFrom(e);
        }

        #region Unavoidable Control Events 
        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            Debug.WriteLine($"{nameof(VisualStateGroup_CurrentStateChanged)}");
            ViewModel.CurrentVisualState = e.NewState;
        }
        #endregion

        #region Loaded
        private void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(HomePage));
            ViewModel.CurrentNavigationLocation = NavigationLocation.Home;

            if (!SimpleIoc.Default.IsRegistered<NavigationService>())
            {
                SimpleIoc.Default.Register<NavigationService>(() => { return new NavigationService(MainFrame, false); });
            }
        }

        private void MainProgressIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            if (!SimpleIoc.Default.IsRegistered<ProgressService>())
            {
                SimpleIoc.Default.Register<ProgressService>(() => { return new ProgressService(MainProgressIndicator); });
            }
        }
        #endregion

        bool hamburgerLoaded = false;
        private void HamburgerButton_Loaded(object sender, RoutedEventArgs e)
        {
            hamburgerLoaded = true;
            RegisterSplitViewService();
        }

        bool splitViewLoaded = false;
        private void MainSplitView_Loaded(object sender, RoutedEventArgs e)
        {
            splitViewLoaded = true;
            RegisterSplitViewService();
        }

        private void RegisterSplitViewService()
        {
            if (hamburgerLoaded && splitViewLoaded)
            {
                Debug.WriteLine($"Registering {nameof(SplitViewService)}");
                if (SimpleIoc.Default.IsRegistered<SplitViewService>()) return;
                SimpleIoc.Default.Register<SplitViewService>(() => { return new SplitViewService(MainSplitView, HamburgerButton); });
            }

            //if (!SimpleIoc.Default.IsRegistered<SplitViewService>())
            //{
            //    Debug.WriteLine($"Registering {nameof(SplitViewService)}");
            //    SimpleIoc.Default.Register<SplitViewService>(() => { return new SplitViewService(MainSplitView, HamburgerButton); });
            //}
        }
    }
}
