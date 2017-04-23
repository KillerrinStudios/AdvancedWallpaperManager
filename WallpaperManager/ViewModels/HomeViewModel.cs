using GalaSoft.MvvmLight.Command;
using KillerrinStudiosToolkit;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using Windows.UI.Xaml.Controls;

namespace WallpaperManager.ViewModels
{
    public class HomeViewModel : WallpaperManagerViewModelBase
    {
        public static HomeViewModel Instance { get { return ServiceLocator.Current.GetInstance<HomeViewModel>(); } }

        public ActiveDesktopThemeSetting ActiveDesktopThemeSetting { get; } = new ActiveDesktopThemeSetting();
        public ActiveLockscreenThemeSetting ActiveLockscreenThemeSetting { get; } = new ActiveLockscreenThemeSetting();

        WallpaperTheme m_activeWallpaperTheme = null;
        public WallpaperTheme ActiveWallpaperTheme
        {
            get { return m_activeWallpaperTheme; }
            set
            {
                m_activeWallpaperTheme = value;
                RaisePropertyChanged(nameof(ActiveWallpaperTheme));
            }
        }

        WallpaperTheme m_activeLockscreenTheme = null;
        public WallpaperTheme ActiveLockscreenTheme
        {
            get { return m_activeLockscreenTheme; }
            set
            {
                m_activeLockscreenTheme = value;
                RaisePropertyChanged(nameof(ActiveLockscreenTheme));
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public HomeViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            ResetViewModel();
        }

        public override void Loaded()
        {

        }

        public override void OnNavigatedTo()
        {
            try
            {
                if (ActiveDesktopThemeSetting.Value.HasValue)
                {
                    ActiveWallpaperTheme = ThemeRepository.Find(ActiveDesktopThemeSetting.Value.Value);
                    Debug.WriteLine($"Active Wallpaper Theme: {ActiveDesktopThemeSetting.Value} - {ActiveWallpaperTheme?.ID} - {ActiveWallpaperTheme?.Name}");
                }
            }
            catch (Exception) { ActiveDesktopThemeSetting.RevertToDefault(); }

            try
            { 
                if (ActiveLockscreenThemeSetting.Value.HasValue)
                {
                    ActiveLockscreenTheme = ThemeRepository.Find(ActiveLockscreenThemeSetting.Value.Value);
                    Debug.WriteLine($"Active Lockscreen Theme: {ActiveLockscreenThemeSetting.Value} - {ActiveLockscreenTheme?.ID} - {ActiveLockscreenTheme?.Name}");
                }
            }
            catch (Exception) { ActiveLockscreenThemeSetting.RevertToDefault(); }
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {

        }

        #region Next Commands
        public RelayCommand NextDesktopWallpaperCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }
        public RelayCommand NextLockscreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                });
            }
        }
        #endregion

        #region Navigate Commands
        public RelayCommand NavigateActiveWallpaperThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ActiveWallpaperTheme == null) return;
                    ThemeListViewModel.Instance.NavigateTheme(ActiveWallpaperTheme);
                });
            }
        }
        public RelayCommand NavigateActiveLockscreenThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ActiveLockscreenTheme == null) return;
                    ThemeListViewModel.Instance.NavigateTheme(ActiveLockscreenTheme);
                });
            }
        }
        #endregion

        #region Deselect Theme Commands
        public RelayCommand DeselectActiveWallpaperThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DeselectActiveWallpaperThemeDialog();
                });
            }
        }
        public async void DeselectActiveWallpaperThemeDialog()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Deselect Desktop Wallpaper Theme?",
                Content = "If you deselect this theme it will no longer automatically update your Desktop Wallpaper Background",
                PrimaryButtonText = "Deselect",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ActiveDesktopThemeSetting.RevertToDefault();
                ActiveWallpaperTheme = null;
            }
        }

        public RelayCommand DeselectActiveLockscreenThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DeselectActiveLockscreenThemeDialog();
                });
            }
        }
        public async void DeselectActiveLockscreenThemeDialog()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Deselect Lockscreen Theme?",
                Content = "If you deselect this theme it will no longer automatically update your Lockscreen Background",
                PrimaryButtonText = "Deselect",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ActiveLockscreenThemeSetting.RevertToDefault();
                ActiveLockscreenTheme = null;
            }
        }
        #endregion
    }
}
