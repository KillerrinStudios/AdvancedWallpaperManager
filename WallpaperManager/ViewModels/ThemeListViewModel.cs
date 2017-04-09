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
                Debug.WriteLine($"{item.ID} - {item.Name}");
                Themes.Add(item);
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

        public void ThemeClicked(WallpaperTheme theme)
        {
            if (!CanNavigate)
                return;

            MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.ThemeDetails;
            NavigationService.Navigate(typeof(ThemeDetailsPage), theme);
        }
    }
}
