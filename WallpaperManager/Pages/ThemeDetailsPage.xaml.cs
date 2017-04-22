using Killerrin_Studios_Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallpaperManager.Models;
using WallpaperManager.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WallpaperManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThemeDetailsPage : Page
    {
        public ThemeDetailsViewModel ViewModel { get { return (ThemeDetailsViewModel)DataContext; } }

        public ThemeDetailsPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Loaded();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.FolderBrowserClosed += ViewModel_FolderBrowserClosed;
            ViewModel.OnNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.FolderBrowserClosed -= ViewModel_FolderBrowserClosed;
            ViewModel.OnNavigatedFrom();
            base.OnNavigatedFrom(e);
        }

        private void EditTheme_Save_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EditThemeFlyout.Hide();
        }

        private void EditTheme_Delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //EditThemeFlyout.Hide();
        }

        private void ViewModel_FolderBrowserClosed(object sender, EventArgs e)
        {
            addDirectoryButton.Flyout.ShowAt(addDirectoryButton);
        }

        private void ImageGridTapped_OpenFlyout(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void ImageGridRightTapped_OpenFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void OpenFileButton(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            FileDiscoveryCache cache = btn.DataContext as FileDiscoveryCache;
            StorageFile file = await StorageFile.GetFileFromPathAsync(cache.FilePath);
            StorageTask.OpenFile(file);
        }

        private async void OpenDirectoryButton(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            FileDiscoveryCache cache = btn.DataContext as FileDiscoveryCache;
            StorageFile file = await StorageFile.GetFileFromPathAsync(cache.FilePath);
            StorageFolder folder = await file.GetParentAsync();

            if (folder != null)
            {
                FolderLauncherOptions launchOptions = new FolderLauncherOptions();
                launchOptions.ItemsToSelect.Add(file);
                StorageTask.OpenFolderInExplorer(folder, launchOptions);
            }
        }
    }
}
