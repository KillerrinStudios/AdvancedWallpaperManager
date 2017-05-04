using GalaSoft.MvvmLight.Command;
using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Models;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWallpaperManager.DAL.Repositories;
using AdvancedWallpaperManager.Models;
using AdvancedWallpaperManager.Models.Settings;
using AdvancedWallpaperManager.Services;
using Windows.UI.Xaml.Controls;

namespace AdvancedWallpaperManager.ViewModels
{
    public class SettingsViewModel : WallpaperManagerViewModelBase
    {
        public static SettingsViewModel Instance { get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); } }

        public FileDiscoveryEnableSetting FileDiscoveryEnabled { get; } = new FileDiscoveryEnableSetting();
        public FileDiscoveryFrequencySetting FileDiscoveryFrequency { get; } = new FileDiscoveryFrequencySetting();
        public FileDiscoveryLastRunSetting FileDiscoveryLastRun { get; } = new FileDiscoveryLastRunSetting();

        public ActiveThemeTaskLastRunSetting ActiveThemeTaskLastRun { get; } = new ActiveThemeTaskLastRunSetting();
        public FileDiscoveryTaskLastRunSetting FileDiscoveryTaskLastRun { get; } = new FileDiscoveryTaskLastRunSetting();

        public List<int> DaysList { get; } = new List<int>();
        private int m_frequencyDays = 0;
        public int FrequencyDays
        {
            get { return m_frequencyDays; }
            set
            {
                m_frequencyDays = value;
                RaisePropertyChanged(nameof(FrequencyDays));
            }
        }

        public List<int> HoursList { get; } = new List<int>();

        private int m_frequencyHours = 0;
        public int FrequencyHours
        {
            get { return m_frequencyHours; }
            set
            {
                m_frequencyHours = value;
                RaisePropertyChanged(nameof(FrequencyHours));
            }
        }

        public List<int> MinutesList { get; } = new List<int>();
        private int m_frequencyMinutes = 0;
        public int FrequencyMinutes
        {
            get { return m_frequencyMinutes; }
            set
            {
                m_frequencyMinutes = value;
                RaisePropertyChanged(nameof(FrequencyMinutes));
            }
        }

        private ObservableCollection<FileAccessToken> m_fileAccessTokens = new ObservableCollection<FileAccessToken>();
        public ObservableCollection<FileAccessToken> FileAccessTokens
        {
            get { return m_fileAccessTokens; }
            set
            {
                m_fileAccessTokens = value;
                RaisePropertyChanged(nameof(FileAccessTokens));
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SettingsViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            // Populate the Timespan Selector Lists
            DaysList = new List<int>();
            for (int i = 0; i <= 7; i++)
                DaysList.Add(i);

            HoursList = new List<int>();
            for (int i = 0; i < 24; i++)
                HoursList.Add(i);

            MinutesList = new List<int>();
            for (int i = 0; i < 60; i++)
                MinutesList.Add(i);

            ResetViewModel();
        }

        public override void Loaded()
        {
        }

        public override void OnNavigatedTo()
        {
            // Set the Defaults
            RevertFileDiscoveryFrequencyCommand.Execute(null);

            // Refresh the File Access Tokens
            var accessTokens = AccessTokenRepository.GetAll();
            foreach (var token in accessTokens)
            {
                FileAccessTokens.Add(token);
            }
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {

        }

        private void Progress_ProgressChanged(object sender, IndicatorProgressReport e)
        {
            ProgressService.SetIndicatorAndShow(e.RingEnabled, e.Percentage, e.StatusMessage, e.WriteToDebugConsole);

            if (e.Percentage >= 100.0)
            {
                ProgressService.Hide();
                FileDiscoveryLastRun.RaiseValuePropertyChanged();
            }
        }

        private async void RefreshFileCache(IProgress<IndicatorProgressReport> progress)
        {
            Debug.WriteLine($"{nameof(SettingsViewModel)} - {nameof(RefreshFileCache)} - BEGIN CACHE TASK");
            FileDiscoveryService fileDiscoveryService = new FileDiscoveryService((WallpaperManagerContext)ThemeRepository.DatabaseInfo.Context);
            var cache = await fileDiscoveryService.PreformFileDiscoveryAll(progress);
            Debug.WriteLine($"{nameof(SettingsViewModel)} - {nameof(RefreshFileCache)} - CACHE TASK COMPLETE");
        }

        public async void RemoveFileAccessToken(FileAccessToken token)
        {
            ContentDialog removeDialog = new ContentDialog
            {
                Title = "Remove File Access Token?",
                Content = "By doing this, the application will no longer have access to these directories and their children. As a result the directories and their files will be removed from your Themes and will require resetting up themes which use them. Are you sure you want to remove this File Access Token from the App?",
                PrimaryButtonText = "Remove",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await removeDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Remove from the Lists
                FileAccessTokens.Remove(token);

                // Delete the Directory
                AccessTokenRepository.RemoveAndCommit(token.ID);

                // Update the Cache
                RefreshFileCacheCommand.Execute(null);
            }
        }

        public RelayCommand SaveFileDiscoveryFrequencyCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    FileDiscoveryFrequency.Value = new TimeSpan(FrequencyDays, FrequencyHours, FrequencyMinutes, 0);
                });
            }
        }
        public RelayCommand RevertFileDiscoveryFrequencyCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    FrequencyDays = FileDiscoveryFrequency.Value.Days;
                    FrequencyHours = FileDiscoveryFrequency.Value.Hours;
                    FrequencyMinutes = FileDiscoveryFrequency.Value.Minutes;
                });
            }
        }

        public RelayCommand ShowProgressCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ProgressService.Show();
                });
            }
        }
        public RelayCommand HideProgressCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ProgressService.Hide();
                });
            }
        }

        public RelayCommand RefreshFileCacheCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Progress<IndicatorProgressReport> progress = new Progress<IndicatorProgressReport>();
                    progress.ProgressChanged += Progress_ProgressChanged;
                    RefreshFileCache(progress);
                });
            }
        }
        public RelayCommand DeleteFileCacheCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    FileDiscoveryCacheRepository.Clear();
                });
            }
        }

        public RelayCommand DeselectActiveDesktopThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ActiveThemeService service = new ActiveThemeService();
                    service.DeselectActiveDesktopTheme();
                });
            }
        }
        public RelayCommand DeselectActiveLockscreenThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ActiveThemeService service = new ActiveThemeService();
                    service.DeselectActiveLockscreenTheme();
                });
            }
        }
    }
}
