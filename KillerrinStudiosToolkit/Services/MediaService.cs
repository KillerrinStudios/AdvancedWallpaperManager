using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KillerrinStudiosToolkit.Services
{
    public class MediaService : ServiceBase
    {
        private MediaElement m_mediaElement;
        public MediaElement MediaPlayer
        {
            get { return m_mediaElement; }
            protected set { m_mediaElement = value; }
        }

        public MediaService(MediaElement mediaElement)
        {
            m_mediaElement = mediaElement;
        }

        #region Set Source
        public void SetSource(Uri source)
        {
            m_mediaElement.Source = source;
        }
        public void SetSource(IRandomAccessStream randomAccessStream, string mimeType)
        {
            m_mediaElement.SetSource(randomAccessStream, mimeType);
        }
        #endregion

        #region Play Pause Stop
        public void Play()
        {
            m_mediaElement.Play();
        }

        public bool CanPause { get { return m_mediaElement.CanPause; } }
        public void Pause()
        {
            m_mediaElement.Pause();
        }

        public void Stop()
        {
            m_mediaElement.Stop();
        }
        #endregion
    }
}
