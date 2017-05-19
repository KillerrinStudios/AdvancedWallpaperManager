using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace KillerrinStudiosToolkit.Services
{
    public class SplitViewService : ServiceBase
    {
        SplitView m_splitViewControl;
        public SplitView SplitViewControl
        {
            get { return m_splitViewControl; }
            protected set { m_splitViewControl = value; }
        }

        Button m_paneToggleControl;
        public Button PaneToggleControl
        {
            get { return m_paneToggleControl; }
            protected set { m_paneToggleControl = value; }
        }

        public bool IsPaneOpen
        {
            get { return m_splitViewControl.IsPaneOpen; }
            set {
                m_splitViewControl.IsPaneOpen = value;
                PaneToggled?.Invoke(this, new TappedRoutedEventArgs());
            }
        }

        public SplitViewDisplayMode DisplayMode
        {
            get { return m_splitViewControl.DisplayMode; }
            set { m_splitViewControl.DisplayMode = value; }
        }

        public SplitViewPanePlacement PanePlacement
        {
            get { return m_splitViewControl.PanePlacement; }
            set { m_splitViewControl.PanePlacement = value; }
        }

        public double CompactPaneLength
        {
            get { return m_splitViewControl.CompactPaneLength; }
            set { m_splitViewControl.CompactPaneLength = value; }
        }

        public double OpenPaneLength
        {
            get { return m_splitViewControl.OpenPaneLength; }
            set { m_splitViewControl.OpenPaneLength = value; }
        }

        public SplitViewService(SplitView view, Button paneToggleButton)
        {
            Debug.WriteLine($"{nameof(SplitViewService)} - Created");
            m_splitViewControl = view;
            m_paneToggleControl = paneToggleButton;

            paneToggleButton.Tapped += M_paneToggleControl_Tapped;
        }

        public event EventHandler<TappedRoutedEventArgs> PaneToggled;

        private void M_paneToggleControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Debug.WriteLine($"{nameof(SplitViewService)} - {nameof(M_paneToggleControl_Tapped)}");
            TogglePane();
        }
        public void TogglePane()
        {
            IsPaneOpen = !IsPaneOpen;
            PaneToggled?.Invoke(this, new TappedRoutedEventArgs());
        }
        public void OpenPane()
        {
            IsPaneOpen = true;
            PaneToggled?.Invoke(this, new TappedRoutedEventArgs());
        }
        public void ClosePane()
        {
            IsPaneOpen = false;
            PaneToggled?.Invoke(this, new TappedRoutedEventArgs());
        }
    }
}
