using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallpaperManager.Models;
using WallpaperManager.Services;
using WallpaperManager.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ThemeListPage : Page
    {
        public ThemeListViewModel ViewModel { get { return (ThemeListViewModel)DataContext; } }

        public ThemeListPage()
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
            ViewModel.OnNavigatedTo();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.OnNavigatedFrom();
            base.OnNavigatedFrom(e);
        }

        private void NewTheme_Create_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CreateThemeFlyout.Hide();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine($"{nameof(ThemeListPage)} - {nameof(GridView_ItemClick)}");
            ViewModel.NavigateTheme((WallpaperTheme)e.ClickedItem);
        }

        WallpaperTheme m_rightClickedWallPaperTheme;
        private void GridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            GridView gridView = (GridView)sender;
            themesMenuFlyout.ShowAt(gridView, e.GetPosition(gridView));
            m_rightClickedWallPaperTheme = ((FrameworkElement)e.OriginalSource).DataContext as WallpaperTheme;
        }

        private void MenuFlyoutItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (m_rightClickedWallPaperTheme == null)
                return;

            ViewModel.DeleteThemeDialog(m_rightClickedWallPaperTheme.ID);
            m_rightClickedWallPaperTheme = null;
        }

        private void MenuFlyoutItem_SetActiveWallpaper_Click(object sender, RoutedEventArgs e)
        {
            if (m_rightClickedWallPaperTheme == null)
                return;

            ViewModel.SetActiveDesktopTheme(m_rightClickedWallPaperTheme);
            m_rightClickedWallPaperTheme = null;
        }
        private void MenuFlyoutItem_SetActiveLockscreen_Click(object sender, RoutedEventArgs e)
        {
            if (m_rightClickedWallPaperTheme == null)
                return;

            ViewModel.SetActiveLockscreenTheme(m_rightClickedWallPaperTheme);
            m_rightClickedWallPaperTheme = null;
        }

        private async void MenuFlyoutItem_CreateWindowsTheme_Click(object sender, RoutedEventArgs e)
        {
            if (m_rightClickedWallPaperTheme == null)
                return;

            WindowsThemeService service = new WindowsThemeService();
            await service.CreateWindowsTheme(m_rightClickedWallPaperTheme);
        }
    }
}
