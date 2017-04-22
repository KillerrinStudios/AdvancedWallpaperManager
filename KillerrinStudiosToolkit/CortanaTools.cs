using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace KillerrinStudiosToolkit
{
    public enum CortanaRecognizerState
    {
        Listening,
        NotListening
    }

    public class CortanaTools
    {
        public static CortanaTools Instance { get; } = new CortanaTools();

        #region Static
        public static Uri CortanaListeningEarcon { get; } = new Uri("ms-appx:///Assets/Sound Effects/ListeningEarcon.wav");
        public static Uri CortanaCancelledEarcon { get; } = new Uri("ms-appx:///Assets/Sound Effects/CancelledEarcon.wav");

        public static async void InstallCortanaVDFile(Uri voiceDefinitionFile)
        {
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(voiceDefinitionFile);
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }

        public static async Task SynthesizeVoice(string textToRead, MediaElement player)
        {
            using (var speech = new SpeechSynthesizer())
            {
                var voiceStream = await speech.SynthesizeTextToStreamAsync(textToRead);
                player.SetSource(voiceStream, voiceStream.ContentType);
                player.Play();
            }
        }
        #endregion

        #region Properties
        private SpeechRecognizerState m_speechRecognizerState;
        public SpeechRecognizerState SpeechRecognizerState
        {
            get { return m_speechRecognizerState; }
            protected set { m_speechRecognizerState = value; }
        }

        private SpeechRecognitionAudioProblem m_speechRecognitionAudioProblem;
        public SpeechRecognitionAudioProblem SpeechRecognitionAudioProblem
        {
            get { return m_speechRecognitionAudioProblem; }
            protected set { m_speechRecognitionAudioProblem = value; }
        }

        private SpeechRecognizer m_speechRecognizer;
        public SpeechRecognizer VoiceRecognizer
        {
            get { return m_speechRecognizer; }
            protected set { m_speechRecognizer = value; }
        }

        private IProgress<APIProgressReport> m_progress;
        #endregion

        private CortanaTools()
        {
        }

        #region Start/Stop Listening
        public async void StartListening(IProgress<APIProgressReport> progress, string exampleText, bool readBackEnabled = false, bool showConfirmation = false)
        {
            m_progress = progress;
            SpeechRecognitionResult speechRecognitionResult = null;

            // Create the Recognizer
            VoiceRecognizer = new SpeechRecognizer();
            VoiceRecognizer.StateChanged += speechRecognizer_StateChanged;
            VoiceRecognizer.HypothesisGenerated += VoiceRecognizer_HypothesisGenerated;
            VoiceRecognizer.RecognitionQualityDegrading += speechRecognizer_RecognitionQualityDegrading;

            // Set special commands
            VoiceRecognizer.UIOptions.ExampleText = "Ex. " + exampleText;
            VoiceRecognizer.UIOptions.IsReadBackEnabled = readBackEnabled;
            VoiceRecognizer.UIOptions.ShowConfirmation = showConfirmation;

            // Set Timeouts
            VoiceRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(6.0);
            VoiceRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(4.0);
            VoiceRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(1.2);

            try
            {
                SpeechRecognitionCompilationResult compilationResult = await VoiceRecognizer.CompileConstraintsAsync();
                speechRecognitionResult = await VoiceRecognizer.RecognizeWithUIAsync(); // RecognizeAsync();
            }
            catch (Exception e) { DebugTools.PrintOutException(e, "Speech Recognition Exception"); }

            VoiceRecognizer = null;
            if (speechRecognitionResult != null)
                if (progress != null)
                    progress.Report(new APIProgressReport(100.0, "Successfully Received Voice Recognition", APIResponse.Successful, speechRecognitionResult));
            else
                if (progress != null)
                    progress.Report(new APIProgressReport(100.0, "Failed to Retrieve Voice Recognition", APIResponse.Failed));
        }

        public void StopListening()
        {
            VoiceRecognizer?.StopRecognitionAsync();
        }
        #endregion


        private void VoiceRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            if (m_progress != null)
                m_progress.Report(new APIProgressReport(50.0, "Hypothosis Generated", APIResponse.ContinuingExecution, args.Hypothesis));
        }
        private void speechRecognizer_RecognitionQualityDegrading(SpeechRecognizer sender, SpeechRecognitionQualityDegradingEventArgs args)
        {
            SpeechRecognitionAudioProblem = args.Problem;
        }
        private void speechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            SpeechRecognizerState = args.State;
        }
    }
}
