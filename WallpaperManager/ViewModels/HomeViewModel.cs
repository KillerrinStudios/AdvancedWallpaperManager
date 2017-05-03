﻿using GalaSoft.MvvmLight.Command;
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
using System.IO;
using KillerrinStudiosToolkit.UserProfile;
using WallpaperManager.Services;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace WallpaperManager.ViewModels
{
    public class HomeViewModel : WallpaperManagerViewModelBase
    {
        public static HomeViewModel Instance { get { return ServiceLocator.Current.GetInstance<HomeViewModel>(); } }

        public ActiveDesktopThemeSetting ActiveDesktopThemeSetting { get; } = new ActiveDesktopThemeSetting();
        public ActiveLockscreenThemeSetting ActiveLockscreenThemeSetting { get; } = new ActiveLockscreenThemeSetting();
        public ActiveDesktopThemeHistorySetting ActiveDesktopThemeHistorySetting { get; } = new ActiveDesktopThemeHistorySetting();
        public ActiveLockscreenThemeHistorySetting ActiveLockscreenThemeHistorySetting { get; } = new ActiveLockscreenThemeHistorySetting();
        private ActiveThemeService m_activeThemeService = new ActiveThemeService();

        #region Active Themes
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
        #endregion

        #region Recent Images
        private ObservableCollection<BitmapImage> m_activeDesktopThemeRecentHistory = new ObservableCollection<BitmapImage>();
        public ObservableCollection<BitmapImage> ActiveDesktopThemeRecentHistory
        {
            get { return m_activeDesktopThemeRecentHistory; }
            set
            {
                m_activeDesktopThemeRecentHistory = value;
                RaisePropertyChanged(nameof(ActiveDesktopThemeRecentHistory));
            }
        }
        private ObservableCollection<BitmapImage> m_activeLockscreenThemeRecentHistory = new ObservableCollection<BitmapImage>();
        public ObservableCollection<BitmapImage> ActiveLockscreenThemeRecentHistory
        {
            get { return m_activeLockscreenThemeRecentHistory; }
            set
            {
                m_activeLockscreenThemeRecentHistory = value;
                RaisePropertyChanged(nameof(ActiveLockscreenThemeRecentHistory));
            }
        }
        #endregion

        #region Current Images
        private string m_currentWallpaperImagePath = "";
        public string CurrentWallpaperImagePath
        {
            get { return m_currentWallpaperImagePath; }
            set
            {
                m_currentWallpaperImagePath = value;
                RaisePropertyChanged(nameof(CurrentWallpaperImagePath));
            }
        }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage CurrentLockscreenImage { get { return LockscreenManager.GetLockscreenBitMapImage(); } }
        private string m_currentLockscreenImagePath = "";
        public string CurrentLockscreenImagePath
        {
            get { return m_currentLockscreenImagePath; }
            set
            {
                m_currentLockscreenImagePath = value;
                RaisePropertyChanged(nameof(CurrentLockscreenImagePath));
            }
        }
        #endregion

        #region Next Images
        private string m_nextWallpaperImagePath = "";
        public string NextWallpaperImagePath
        {
            get { return m_nextWallpaperImagePath; }
            set
            {
                m_nextWallpaperImagePath = value;
                RaisePropertyChanged(nameof(NextWallpaperImagePath));
            }
        }

        private Windows.UI.Xaml.Media.Imaging.BitmapImage m_nextWallpaperBitmapImage = null;
        public Windows.UI.Xaml.Media.Imaging.BitmapImage NextWallpaperBitmapImage
        {
            get { return m_nextWallpaperBitmapImage; }
            set
            {
                m_nextWallpaperBitmapImage = value;
                RaisePropertyChanged(nameof(NextWallpaperBitmapImage));
            }
        }

        private string m_nextLockscreenImagePath = "";
        public string NextLockscreenImagePath
        {
            get { return m_nextLockscreenImagePath; }
            set
            {
                m_nextLockscreenImagePath = value;
                RaisePropertyChanged(nameof(NextLockscreenImagePath));
            }
        }
        private Windows.UI.Xaml.Media.Imaging.BitmapImage m_nextLockscreenBitmapImage = null;
        public Windows.UI.Xaml.Media.Imaging.BitmapImage NextLockscreenBitmapImage
        {
            get { return m_nextLockscreenBitmapImage; }
            set
            {
                m_nextLockscreenBitmapImage = value;
                RaisePropertyChanged(nameof(NextLockscreenBitmapImage));
            }
        }
        #endregion

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

        public async override void OnNavigatedTo()
        {
            ActiveWallpaperTheme = m_activeThemeService.GetActiveDesktopTheme();
            ActiveLockscreenTheme = m_activeThemeService.GetActiveLockscreenTheme();

            CurrentWallpaperImagePath = m_activeThemeService.GetCurrentDesktopImagePath();
            CurrentLockscreenImagePath = m_activeThemeService.GetCurrentLockscreenImagePath();

            NextWallpaperImagePath = m_activeThemeService.GetNextDesktopImagePath();
            NextLockscreenImagePath = m_activeThemeService.GetNextLockscreenImagePath();

            Debug.WriteLine($"Active Wallpaper Theme: {ActiveDesktopThemeSetting.Value} - {ActiveWallpaperTheme?.ID} - {ActiveWallpaperTheme?.Name}");
            Debug.WriteLine($"Active Lockscreen Theme: {ActiveLockscreenThemeSetting.Value} - {ActiveLockscreenTheme?.ID} - {ActiveLockscreenTheme?.Name}");

            NextWallpaperBitmapImage = await GetImage(NextWallpaperImagePath);
            NextLockscreenBitmapImage = await GetImage(NextLockscreenImagePath);
            RefreshRecentWallpaper();
            RefreshRecentLockscreen();
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {

        }

        private async Task<Windows.UI.Xaml.Media.Imaging.BitmapImage> GetImage(string path)
        {
            var file = await StorageTask.Instance.GetFileFromPath(new Uri(path));
            return await StorageTask.StorageFileToBitmapImage(file);
        }

        #region Refresh Recent
        public RelayCommand RefreshRecentWallpaperCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RefreshRecentWallpaper();
                });
            }
        }
        public async void RefreshRecentWallpaper()
        {
            ActiveDesktopThemeRecentHistory = new ObservableCollection<BitmapImage>();
            var tmpHistory = ActiveDesktopThemeHistorySetting.Value;
            foreach (var item in tmpHistory)
            {
                var img = await GetImage(item);
                ActiveDesktopThemeRecentHistory.Add(img);
            }
        }

        public RelayCommand RefreshRecentLockscreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RefreshRecentLockscreen();
                });
            }
        }
        public async void RefreshRecentLockscreen()
        {
            ActiveLockscreenThemeRecentHistory = new ObservableCollection<BitmapImage>();
            var tmpHistory = ActiveLockscreenThemeHistorySetting.Value;
            foreach (var item in tmpHistory)
            {
                var img = await GetImage(item);
                ActiveLockscreenThemeRecentHistory.Add(img);
            }
        }
        #endregion

        #region Next Commands
        public RelayCommand NextDesktopWallpaperCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NextDesktopWallpaper();
                });
            }
        }
        public async void NextDesktopWallpaper()
        {
            await m_activeThemeService.NextDesktopBackground(NextWallpaperImagePath);
            CurrentWallpaperImagePath = NextWallpaperImagePath;
            NextWallpaperImagePath = m_activeThemeService.GetNextDesktopImagePath();
            ActiveDesktopThemeHistorySetting.RaiseValuePropertyChanged();

            NextWallpaperBitmapImage = await GetImage(NextWallpaperImagePath);
            RefreshRecentWallpaper();
        }

        public RelayCommand NextLockscreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NextLockscreen();
                });
            }
        }

        public async void NextLockscreen()
        {
            await m_activeThemeService.NextLockscreenBackground(NextLockscreenImagePath);
            CurrentLockscreenImagePath = NextLockscreenImagePath;
            NextLockscreenImagePath = m_activeThemeService.GetNextLockscreenImagePath();
            ActiveLockscreenThemeHistorySetting.RaiseValuePropertyChanged();

            NextLockscreenBitmapImage = await GetImage(NextLockscreenImagePath);
            RaisePropertyChanged(nameof(CurrentLockscreenImage));
            RefreshRecentLockscreen();
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
                ActiveThemeService activeThemeService = new ActiveThemeService();
                activeThemeService.DeselectActiveDesktopTheme();
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
                ActiveThemeService activeThemeService = new ActiveThemeService();
                activeThemeService.DeselectActiveLockscreenTheme();
                ActiveLockscreenTheme = null;
            }
        }
        #endregion
    }
}
