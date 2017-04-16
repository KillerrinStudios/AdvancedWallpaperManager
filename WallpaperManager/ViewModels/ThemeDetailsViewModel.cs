using GalaSoft.MvvmLight.Command;
using Killerrin_Studios_Toolkit;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Pages;
using WallpaperManager.Services;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace WallpaperManager.ViewModels
{
    public partial class ThemeDetailsViewModel : WallpaperManagerViewModelBase
    {
        #region Themes
        private WallpaperTheme m_theme;
        public WallpaperTheme Theme
        {
            get { return m_theme; }
            set
            {
                m_theme = value;
                RaisePropertyChanged(nameof(Theme));
            }
        }
        private string m_newThemeName = "";
        public string NewThemeName
        {
            get { return m_newThemeName; }
            set
            {
                m_newThemeName = value;
                RaisePropertyChanged(nameof(NewThemeName));
            }
        }
        #endregion

        #region Directory
        private WallpaperDirectory m_newDirectory = new WallpaperDirectory();
        public WallpaperDirectory NewDirectory
        {
            get { return m_newDirectory; }
            set
            {
                m_newDirectory = value;
                RaisePropertyChanged(nameof(NewDirectory));
            }
        }

        private StorageFolder m_storageFolder;
        #endregion

        private ObservableCollection<GroupedFileCache> m_fileCache = new ObservableCollection<GroupedFileCache>();
        public ObservableCollection<GroupedFileCache> FileCache
        {
            get { return m_fileCache; }
            set
            {
                m_fileCache = value;
                RaisePropertyChanged(nameof(FileCache));
            }
        }

        public ThemeDetailsViewModel()
            : base()
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

        public override void OnNavigatedFrom()
        {
            m_storageFolder = null;
        }

        public override void OnNavigatedTo()
        {
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(OnNavigatedTo)}");

            // Grab the parameter
            Theme = (WallpaperTheme)NavigationService.Parameter;

            // Load the files
            Progress<IndicatorProgressReport> progress = new Progress<IndicatorProgressReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            LoadAllFiles(progress);
        }

        private void Progress_ProgressChanged(object sender, IndicatorProgressReport e)
        {
            ProgressService.SetIndicatorAndShow(e.RingEnabled, e.Percentage, e.StatusMessage, e.WriteToDebugConsole);

            if (e.Percentage >= 100.0)
                ProgressService.Hide();
        }

        public override void ResetViewModel()
        {
        }
        public async Task LoadAllFiles(IProgress<IndicatorProgressReport> progress)
        {
            if (Theme == null) return;

            FileDiscoveryService fileDiscoveryService = new FileDiscoveryService();
            var cache = await fileDiscoveryService.PreformFileDiscovery(Theme, progress);

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - COMPLETE");
                ProgressService.Hide();

                // Group into an easy to process Dictionary
                Dictionary<string, List<FileDiscoveryCache>> cacheDictionary = (from c in cache
                                                                                group c by c.FolderPath
                                                                                into groupedCache
                                                                                select groupedCache).ToDictionary(x => x.Key, x => x.ToList());

                // Clear the previous Cache and add in the new ones
                FileCache.Clear();
                foreach (var c in cacheDictionary)
                {
                    GroupedFileCache groupedCache = new GroupedFileCache();
                    groupedCache.FolderPath = c.Key;
                    groupedCache.Files = c.Value;
                    FileCache.Add(groupedCache);
                }
            });
        }

        #region Commands
        #region Open Folder Browser
        public RelayCommand OpenFolderBrowserCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    OpenFolderBrowser();
                });
            }
        }

        private async void OpenFolderBrowser()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            m_storageFolder = await folderPicker.PickSingleFolderAsync();

            if (m_storageFolder != null)
            {
                NewDirectory.Path = m_storageFolder.Path;
            }
        }
        #endregion

        public RelayCommand AddDirectoryCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (m_storageFolder == null) return;

                    // Step one, determine if the Access Token needs to be officially created by using the Path.
                    FileAccessToken token = null;

                    // If the Access Token for our Path already exists, or our path is within the subdirectories of the existing tokens path
                    // Then use the existing token
                    token = AccessTokenRepository.GetAllQuery()
                        .Where(x => m_storageFolder.Path.Contains(x.Path))
                        .FirstOrDefault();

                    // If the Access Token for our Path does not exist, create a new token
                    if (token == null)
                    {
                        // Add to FA without metadata
                        var m_futureAccessToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(m_storageFolder);

                        // Create the token
                        token = new FileAccessToken();
                        token.AccessToken = "" + m_futureAccessToken;
                        token.AccessTokenType = Models.Enums.FileAccessTokenType.FutureAccess;
                        token.Path = m_storageFolder.Path;
                        AccessTokenRepository.Add(token);
                    }

                    // If the token.ID is still 0, something has gone wrong and we should exit
                    if (token.ID == 0) return;

                    // Step two, Add the Directory to the Database
                    NewDirectory.WallpaperThemeID = Theme.ID;
                    NewDirectory.FileAccessTokenID = token.ID;
                    NewDirectory.Path = m_storageFolder.Path;
                    NewDirectory.StorageLocation = Models.Enums.StorageLocation.Local;
                    DirectoryRepository.Add(NewDirectory);

                    // Reinstantiate the variable to prepare for additional potential Directory Additions
                    NewDirectory = new WallpaperDirectory();

                    // Reset the browse folder window
                    m_storageFolder = null;

                    // Finally, preform the expensive operation to regather all the files in a background thread
                    Progress<IndicatorProgressReport> progress = new Progress<IndicatorProgressReport>();
                    progress.ProgressChanged += Progress_ProgressChanged;
                    LoadAllFiles(progress);
                });
            }
        }


        public RelayCommand EditThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(NewThemeName))
                        return;

                    // Update the Theme
                    Theme.Name = "" + NewThemeName;
                    Theme.DateLastModified = DateTime.UtcNow;
                    ThemeRepository.Update(Theme);

                    // Reset the Variables
                    NewThemeName = "";
                });
            }
        }

        #region Delete
        public RelayCommand DeleteThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DeleteThemeDialog();
                });
            }
        }

        public async void DeleteThemeDialog()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Theme Permanently?",
                Content = "If you delete this theme you will not be able to recover it. Are you sure you want to PERMANENTLY delete this Theme?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Delete the theme
                var theme = ThemeRepository.Find(Theme.ID);
                if (theme == null)
                    return;
                ThemeRepository.Remove(Theme.ID);

                // Redirect to the MainPage
                MainViewModel.Instance.NavigateThemesCommand.Execute(null);
            }

        }
        #endregion
        #endregion
    }
}
