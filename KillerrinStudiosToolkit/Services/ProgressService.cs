using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Controls;
using Windows.UI.Xaml;

namespace KillerrinStudiosToolkit.Services
{
    public class ProgressService : ServiceBase
    {
        private ProgressIndicator m_progressIndicator;
        public ProgressIndicator ProgressIndicator { get { return m_progressIndicator; } }

        public ProgressService(ProgressIndicator progressIndicator)
        {
            m_progressIndicator = progressIndicator;
        }

        public void Reset()
        {
            IsRingEnabled = false;

            PercentageVisibility = Visibility.Visible;
            PercentageCompleted = 0.0;

            StatusMessage = "";

            Visibility = Visibility.Collapsed;
        }

        #region Show/Hide
        public void Show()
        {
            m_progressIndicator.Visibility = Visibility.Visible;
            m_progressIndicator.PercentageVisibility = Visibility.Visible;
        }
        public void Hide()
        {
            m_progressIndicator.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Enable/Disable Ring
        public void EnableRing()
        {
            IsRingEnabled = true;
        }
        public void DisableRing()
        {
            IsRingEnabled = false;
        }
        #endregion

        #region Individual Values
        public Visibility Visibility
        {
            get { return m_progressIndicator.Visibility; }
            set { m_progressIndicator.Visibility = value; }
        }

        public bool IsRingEnabled
        {
            get { return m_progressIndicator.IsRingActive; }
            set { m_progressIndicator.IsRingActive = value; }
        }

        public Visibility PercentageVisibility
        {
            get { return m_progressIndicator.PercentageVisibility; }
            set { m_progressIndicator.PercentageVisibility = value; }
        }

        public double PercentageCompleted
        {
            get { return m_progressIndicator.PercentageCompleted; }
            set { m_progressIndicator.PercentageCompleted = value; }
        }

        public string StatusMessage
        {
            get { return m_progressIndicator.StatusMessage; }
            set { m_progressIndicator.StatusMessage = value; }
        }
        #endregion

        public void SetIndicator(bool isRingEnabled, double percentage, string message, bool debugWriteLine = false)
        {
            if (!ServiceEnabled) return;

            IsRingEnabled = isRingEnabled;
            m_progressIndicator.PercentageCompleted = percentage;
            m_progressIndicator.StatusMessage = message;

            if (debugWriteLine)
                Debug.WriteLine(string.Format("{0} | {1} | {2}", isRingEnabled, percentage, message));
        }
        public void SetIndicatorAndShow(bool isRingEnabled, double percentage, string message, bool debugWriteLine = false)
        {
            if (!ServiceEnabled) return;

            SetIndicator(isRingEnabled, percentage, message, debugWriteLine);
            Show();
        }

        public override void EnableService()
        {
            Show();
            base.EnableService();
        }
        public override void DisableService()
        {
            Hide();
            base.DisableService();
        }
    }
}
