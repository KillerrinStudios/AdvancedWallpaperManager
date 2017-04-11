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
using Windows.UI.Xaml.Controls;

namespace WallpaperManager.ViewModels
{
    public class ThemeListViewModel : WallpaperManagerViewModelBase
    {
        public static ThemeListViewModel Instance { get { return ServiceLocator.Current.GetInstance<ThemeListViewModel>(); } }

        public ObservableCollection<WallpaperTheme> Themes { get; set; } = new ObservableCollection<WallpaperTheme>();

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

        public ThemeListViewModel()
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
            Debug.WriteLine($"{nameof(ThemeListViewModel)} - {nameof(OnNavigatedTo)}");
            Themes.Clear();

            var tmp = ThemeRepository.GetAll();
            foreach (var item in tmp)
            {
                Themes.Add(item);
            }

            // Print all the values to the screen to debug
            foreach (var item in AccessTokenRepository.GetAll())
            {
                Debug.WriteLine($"Access Tokens: {item.Path} - {item.AccessTokenType} | {item.AccessToken}");
            }
            foreach (var item in ThemeRepository.GetAll())
            {
                Debug.WriteLine($"Theme: {item.Name}");
            }
            foreach (var item in DirectoryRepository.GetAll())
            {
                Debug.WriteLine($"Directories: {item.Path}");
            }
        }

        public override void ResetViewModel()
        {
            Themes.Clear();
        }

        public RelayCommand CreateThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(NewThemeName))
                        return;

                    // Create the New Theme
                    var newTheme = new WallpaperTheme();
                    newTheme.Name = "" + NewThemeName;
                    newTheme.DateLastModified = DateTime.UtcNow;
                    newTheme.DateCreated = DateTime.UtcNow;
                    ThemeRepository.Add(newTheme);

                    // Reset the variables and add to GridView
                    NewThemeName = "";
                    Themes.Add(newTheme);
                });
            }
        }

        public async void DeleteThemeDialog(int id)
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
                var theme = ThemeRepository.Find(id);
                if (theme == null)
                    return;
                ThemeRepository.Remove(id);

                // Remove from the Themes List
                for (int i = Themes.Count - 1; i >= 0; i--)
                {
                    if (Themes[i].ID == id)
                    {
                        Themes.RemoveAt(i);
                        break;
                    }
                }
            }

        }

        public void ThemeClicked(WallpaperTheme theme)
        {
            if (!CanNavigate)
                return;

            MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.ThemeDetails;
            NavigationService.Navigate(typeof(ThemeDetailsPage), theme);
        }
    }
}
