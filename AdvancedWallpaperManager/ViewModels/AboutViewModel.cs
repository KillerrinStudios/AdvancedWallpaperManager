using GalaSoft.MvvmLight.Command;
using KillerrinStudiosToolkit;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWallpaperManager.ViewModels
{
    public class AboutViewModel : WallpaperManagerViewModelBase
    {
        public static AboutViewModel Instance { get { return ServiceLocator.Current.GetInstance<AboutViewModel>(); } }

        KillerrinApplicationData m_applicationData = new KillerrinApplicationData("http://www.hummingbird.me");
        public KillerrinApplicationData ApplicationData
        {
            get { return m_applicationData; }
            protected set
            {
                m_applicationData = value;
                RaisePropertyChanged(nameof(ApplicationData));
            }
        }

        public string EmailFeedBackContent { get { return "Feedback - " + ApplicationData.FeedbackUrl; } }
        public string EmailSupportContent { get { return "Support - " + ApplicationData.FeedbackUrl; } }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public AboutViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                ApplicationData.Name = "Advanced Wallpaper Manager";
                ApplicationData.PublisherName = "Killerrin Studios";
                ApplicationData.Description = "Description Goes Here";
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
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {
        }

        #region Email Feedback
        public RelayCommand EmailFeedbackCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    EmailFeedback();
                });
            }
        }

        public async void EmailFeedback()
        {
            Debug.WriteLine("Emailing Feedback");
            await KillerrinApplicationData.SendEmail(ApplicationData.FeedbackUrl, ApplicationData.FeedbackSubject, "");
        }
        #endregion

        #region Email Support
        public RelayCommand EmailSupportCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    EmailSupport();
                });
            }
        }

        public async void EmailSupport()
        {
            Debug.WriteLine("Emailing Support");
            await KillerrinApplicationData.SendEmail(ApplicationData.SupportUrl, ApplicationData.SupportSubject, "");
        }
        #endregion
    }
}
