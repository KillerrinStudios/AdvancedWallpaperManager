using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Popups;
using WallpaperManager.Models.Enums;

namespace WallpaperManager.ViewModels
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

        private NavigationLocation m_currentNavigationLocation = NavigationLocation.Default;
        public NavigationLocation CurrentNavigationLocation
        {
            get { return m_currentNavigationLocation; }
            set
            {
                if (m_currentNavigationLocation == value) return;
                m_currentNavigationLocation = value;
                RaisePropertyChanged(nameof(CurrentNavigationLocation));

                TopNavBarText = Helpers.StringHelpers.AddSpacesToSentence(m_currentNavigationLocation.ToString(), true);
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
        public RelayCommand SampleCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        #region Navigate Settings Command
        //public RelayCommand NavigateSettingsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() =>
        //        {
        //            NavigateSettings();
        //        });
        //    }
        //}

        //public void NavigateSettings()
        //{
        //    Debug.WriteLine("Navigate Settings");
        //    if (!CanNavigate)
        //        return;

        //    if (CurrentVisualState?.Name != AdaptiveTriggerConsts.DesktopMinimumWidthName)
        //        IsPaneOpen = false;

        //    NavigationService.Navigate(typeof(SettingsPage), null);
        //}
        #endregion

        #region Navigate About Command
        //public RelayCommand NavigateAboutCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() =>
        //        {
        //            NavigateAbout();
        //        });
        //    }
        //}

        //public void NavigateAbout()
        //{
        //    Debug.WriteLine("Navigate About");
        //    if (!CanNavigate)
        //        return;

        //    if (CurrentVisualState?.Name != AdaptiveTriggerConsts.DesktopMinimumWidthName)
        //        IsPaneOpen = false;

        //    NavigationService.Navigate(typeof(AboutPage), null);
        //}
        #endregion

        #region Rate App Command
        //public RelayCommand RateAppCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() =>
        //        {
        //            RateApp();
        //        });
        //    }
        //}

        //public void RateApp()
        //{
        //    Debug.WriteLine("Rating App");
        //    if (!CanNavigate)
        //        return;

        //    ViewModelLocator.Instance.vm_AboutViewModel.ApplicationData.LaunchReview();
        //}
        #endregion
        #endregion
    }
}