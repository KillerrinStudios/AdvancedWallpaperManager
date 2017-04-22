using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KillerrinStudiosToolkit.Services
{

    public class NavigationService : ServiceBase
    {
        public event BackClickEventHandler BackButtonClicked;

        private Frame m_frame;
        public Frame Frame { get { return m_frame; } }
        public Type CurrentPage { get { return Frame.CurrentSourcePageType; } }

        private object m_parameter = new object();
        public object Parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        public NavigationService(Frame frame)
        {
            m_frame = frame;

            SystemNavigationManager.GetForCurrentView().BackRequested += NavigationService_BackRequested;
            EnableBackButton();
        }
        public NavigationService(Frame frame, bool backButton)
        {
            m_frame = frame;

            SystemNavigationManager.GetForCurrentView().BackRequested += NavigationService_BackRequested;

            if (backButton) EnableBackButton();
            else DisableBackButton();
        }

        #region Back Button
        public void NavigationService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            BackClickEventArgs backClickEventArgs = new BackClickEventArgs();
            BackButtonClicked?.Invoke(sender, backClickEventArgs);

            if (GoBack()) { }
            else
            {
                Application.Current.Exit();
            }
        }

        public void EnableBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }
        public void DisableBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
        #endregion

        #region Navigation
        public bool Navigate(Type type, object parameter)
        {
            Parameter = parameter;
            return m_frame.Navigate(type, Parameter);
        }

        public bool GoBack()
        {
            if (!m_frame.CanGoBack) return false;
            m_frame.GoBack();
            return true;
        }
        public bool GoForward()
        {
            if (!m_frame.CanGoForward) return false;
            m_frame.GoForward();
            return true;
        }
        #endregion

        #region Helpers
        public bool CanGoBack { get { return m_frame.CanGoBack; } }
        public bool CanGoForward {  get { return m_frame.CanGoForward; } }
        public bool RemoveLastPage()
        {
            if (!CanGoBack)
                return false;

            m_frame.BackStack.Remove(m_frame.BackStack.Last());
            return true;
        }
        #endregion
    }

}
