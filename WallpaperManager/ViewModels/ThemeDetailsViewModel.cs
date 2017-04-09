using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Pages;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace WallpaperManager.ViewModels
{
    public class ThemeDetailsViewModel : WallpaperManagerViewModelBase
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

        private ObservableCollection<StorageFolderFiles> m_folderFiles = new ObservableCollection<StorageFolderFiles>();
        public ObservableCollection<StorageFolderFiles> FolderFiles
        {
            get { return m_folderFiles; }
            set
            {
                m_folderFiles = value;
                RaisePropertyChanged(nameof(FolderFiles));
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
        }

        public override void OnNavigatedTo()
        {
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(OnNavigatedTo)}");
            Theme = (WallpaperTheme)NavigationService.Parameter;

            LoadAllFiles();
        }

        public override void ResetViewModel()
        {
        }

        public async Task LoadAllFiles()
        {
            if (Theme == null) return;

            // Step one we have to populate our openedList with all the directories in the theme
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 1");
            List <WallpaperDirectory> openedList = new List<WallpaperDirectory>();
            openedList.AddRange(Theme.Directories);
            List<WallpaperDirectory> excludedList = openedList.Where(x => x.IsExcluded).ToList();

            // Step two, go through all of the directories and convert them into StorageFolders
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 2");
            List<StorageFolder> allFolders = new List<StorageFolder>();
            while (openedList.Count > 0)
            {
                try
                {
                    // If the folder/file is suppose to be excluded, ignore it now
                    if (openedList[0].IsExcluded)
                        continue;

                    // Convert the path into a StorageFolder
                    var folder = await StorageFolder.GetFolderFromPathAsync(openedList[0].Path);
                    allFolders.Add(folder);

                    // If we are allowed to gather subdirectories, gather them as wells
                    if (openedList[0].IncludeSubdirectories)
                    {
                        var subfolders = await folder.GetFoldersAsync();
                        allFolders.AddRange(subfolders);
                    }
                }
                catch (Exception e) { }

                openedList.RemoveAt(0);
            }

            // Step three, remove all of the excluded directories
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 3");
            for (int i = excludedList.Count - 1; i >= 0; i--)
            {
                bool itemsRemoved = false;
                for (int x = allFolders.Count - 1; x >= 0; x--)
                {
                    if (allFolders[x].Path.Contains(excludedList[i].Path))
                    {
                        itemsRemoved = true;
                        allFolders.RemoveAt(x);
                    }
                }

                if (itemsRemoved)
                    excludedList.RemoveAt(i);
            }

            // Step four, go through all the StorageFolders and gather their image files
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 4");
            ObservableCollection<StorageFolderFiles> folderFiles = new ObservableCollection<StorageFolderFiles>();
            for (int i = 0; i < allFolders.Count; i++)
            {
                try
                {
                    var files = await allFolders[i].GetFilesAsync();

                    StorageFolderFiles tmp = new StorageFolderFiles();
                    tmp.Folder = allFolders[i];

                    for (int x = 0; x < files.Count; i++)
                    {
                        if (files[i].ContentType.ToLower().Contains("image"))
                            tmp.Files.Add(files[i]);
                    }

                    if (tmp.Files.Count > 0)
                        folderFiles.Add(tmp);
                }
                catch (Exception e) { }
            }

            // Step five, finish off the exclusion list
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 5");
            for (int i = excludedList.Count - 1; i >= 0; i--)
            {
                bool itemsRemoved = false;
                foreach (var folder in folderFiles)
                {
                    for (int x = folder.Files.Count - 1; x >= 0; x--)
                    {
                        if (folder.Files[x].Path.Contains(excludedList[i].Path))
                        {
                            itemsRemoved = true;
                            folder.Files.RemoveAt(x);
                        }
                    }
                }

                if (itemsRemoved)
                    excludedList.RemoveAt(i);
            }

            // Step six, store the StorageFolderFiles to the side for later
            Debug.WriteLine($"{nameof(ThemeDetailsViewModel)} - {nameof(LoadAllFiles)} - Step 6");
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                FolderFiles = folderFiles;
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

            // Set the path
            if (m_storageFolder != null)
                NewDirectory.Path = m_storageFolder.Path;
        }
        #endregion

        public RelayCommand AddDirectoryCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (m_storageFolder == null) return;

                    // Save to the database
                    NewDirectory.WallpaperThemeID = Theme.ID;
                    //NewDirectory.Theme = Theme;
                    NewDirectory.Path = m_storageFolder.Path;
                    NewDirectory.StorageLocation = Models.Enums.StorageLocation.Local;
                    DirectoryRepository.Add(NewDirectory);

                    // Update the Themes Directory List
                    //Theme.Directories.Add(NewDirectory);

                    // Reinstantiate the variable to prepare for additional potential Directory Additions
                    NewDirectory = new WallpaperDirectory();

                    // Finally, preform the expensive operation to regather all the files in a background thread
                    LoadAllFiles();
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
