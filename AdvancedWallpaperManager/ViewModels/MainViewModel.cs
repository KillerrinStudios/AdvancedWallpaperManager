using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Popups;
using KillerrinStudiosToolkit.Helpers;
using AdvancedWallpaperManager.Models.Enums;
using AdvancedWallpaperManager.Pages;

namespace AdvancedWallpaperManager.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : WallpaperManagerViewModelBase
    {
        public static MainViewModel Instance {  get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }
        public VisualState CurrentVisualState;

        #region Properties
        public RelayCommand TogglePaneCommand { get { return new RelayCommand(() => { IsPaneOpen = !IsPaneOpen; }); } }
        private bool m_isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return m_isPaneOpen; }
            set
            {
                if (m_isPaneOpen == value) return;
                m_isPaneOpen = value;
                RaisePropertyChanged(nameof(IsPaneOpen));
            }
        }

        private NavigationLocation m_currentNavigationLocation = NavigationLocation.None;
        public NavigationLocation CurrentNavigationLocation
        {
            get { return m_currentNavigationLocation; }
            set
            {
                if (m_currentNavigationLocation == value) return;
                m_currentNavigationLocation = value;
                RaisePropertyChanged(nameof(CurrentNavigationLocation));

                TopNavBarText = StringHelpers.AddSpacesToSentence(m_currentNavigationLocation.ToString(), true);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
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
            // Handle Launch Args
        }

        public override void OnNavigatedTo()
        {
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {

        }

        #region Navigation Commands
        public RelayCommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!CanNavigate)
                        return;

                    MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.Home;
                    NavigationService.Navigate(typeof(HomePage), null);
                });
            }
        }

        public RelayCommand NavigateThemesCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!CanNavigate)
                        return;

                    MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.ThemeList;
                    NavigationService.Navigate(typeof(ThemeListPage), null);
                });
            }
        }

        public RelayCommand NavigateAboutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!CanNavigate)
                        return;

                    MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.About;
                    NavigationService.Navigate(typeof(AboutPage), null);
                });
            }
        }

        public RelayCommand NavigateSettingsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!CanNavigate)
                        return;

                    MainViewModel.Instance.CurrentNavigationLocation = Models.Enums.NavigationLocation.Settings;
                    NavigationService.Navigate(typeof(SettingsPage), null);
                });
            }
        }

        public RelayCommand RateAppCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!CanNavigate)
                        return;

                    LaunchReview();
                });
            }
        }

        public async void LaunchReview()
        {
            await ViewModelLocator.Instance.vm_AboutViewModel.ApplicationData.LaunchReview();
        }
        #endregion
    }
}