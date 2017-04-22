using KillerrinStudiosToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace KillerrinStudiosToolkit.Helpers
{
    public class AdaptiveTriggerConsts : ModelBase, IDisposable
    {
        public static AdaptiveTriggerConsts Instance;

        #region Minimum Window Widths
        public const int PhoneSmallMinimumWindowWidth = 0;
        public int PhoneSmallMinimumWidth { get { return PhoneSmallMinimumWindowWidth; } }
        public const string PhoneSmallMinimumWidthName = "PhoneSmall";

        public const int PhoneMinimumWindowWidth = 480;
        public int PhoneMinimumWidth { get { return PhoneMinimumWindowWidth; } }
        public const string PhoneMinimumWidthName = "Phone";


        public const int DesktopMinimumWindowWidth = 720;
        public int DesktopMinimumWidth { get { return DesktopMinimumWindowWidth; } }
        public const string DesktopMinimumWidthName = "Desktop";

        #endregion

        #region Window Sizes
        //public DisplayOrientations Orientation { get { return DisplayInformation.GetForCurrentView().CurrentOrientation; } }
        public double WindowWidth  { get { return 0.0; } } //Window.Current.Bounds.Width; } }
        public double WindowHeight { get { return 0.0; } } //Window.Current.Bounds.Height; } }
        #endregion

        //public APIServiceCollection APIServiceCollection { get { return APIServiceCollection.Instance; } }
        public AdaptiveTriggerConsts()
        {
            //DisplayInformation.GetForCurrentView().OrientationChanged += AdaptiveTriggerConsts_OrientationChanged;
            //Window.Current.SizeChanged += Current_SizeChanged;
            Instance = this;
        }

        #region Raise Property Changed Events
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(WindowWidth));
            RaisePropertyChanged(nameof(WindowHeight));
        }

        private void AdaptiveTriggerConsts_OrientationChanged(DisplayInformation sender, object args)
        {
            //RaisePropertyChanged(nameof(Orientation));
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisplayInformation.GetForCurrentView().OrientationChanged -= AdaptiveTriggerConsts_OrientationChanged;
                    Window.Current.SizeChanged -= Current_SizeChanged;
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
