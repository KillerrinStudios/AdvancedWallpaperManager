using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace KillerrinStudiosToolkit.Controls
{
    public sealed partial class ProgressIndicator : UserControl
    {
        public ProgressIndicator()
        {
            this.InitializeComponent();
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LayoutRoot.DataContext = this;
            }
        }

        #region StatusMessage
        public bool IsRingActive
        {
            get { return (bool)GetValue(IsRingActiveProperty); }
            set { SetValue(IsRingActiveProperty, value); }
        }
        public static readonly DependencyProperty IsRingActiveProperty =
            DependencyProperty.Register(nameof(IsRingActive), typeof(bool), typeof(ProgressIndicator), new PropertyMetadata(false));
        #endregion

        #region PercentageVisibility
        public Visibility PercentageVisibility
        {
            get { return (Visibility)GetValue(PercentageVisibilityProperty); }
            set { SetValue(PercentageVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PercentageVisibilityProperty =
            DependencyProperty.Register(nameof(PercentageVisibility), typeof(Visibility), typeof(ProgressIndicator), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region Percentage Completed
        public double PercentageCompleted
        {
            get { return (double)GetValue(PercentageCompletedProperty); }
            set { SetValue(PercentageCompletedProperty, value); }
        }
        public static readonly DependencyProperty PercentageCompletedProperty =
            DependencyProperty.Register(nameof(PercentageCompleted), typeof(double), typeof(ProgressIndicator), new PropertyMetadata(0.0));
        #endregion

        #region StatusMessage
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }
        public static readonly DependencyProperty StatusMessageProperty =
            DependencyProperty.Register(nameof(StatusMessage), typeof(string), typeof(ProgressIndicator), new PropertyMetadata(""));
        #endregion
    }
}
