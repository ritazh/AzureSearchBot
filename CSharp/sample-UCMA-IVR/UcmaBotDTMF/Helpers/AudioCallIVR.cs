using Microsoft.CognitiveServices.SpeechRecognition;
using Microsoft.Rtc.Collaboration.AudioVideo;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using UcmaBotDTM;
using UcmaBotDTMF.Common;
using UcmaBotDTMF.Models;

namespace UcmaBotDTMF.Helpers
{
    public delegate void LoggingHandler(string displayName, string message);
    public class AudioCallIVR
    {
        private AudioVideoFlow flow;
        private Player player;
        private LoggingHandler callback;
        private DirectlineEndpoint directLine;
        private string conversationId;
        private DataRecognitionClient dataClient;
        private ToneController tone;
        private DirectlineMessage recentMessage = null;
        private AmazonText2Speech amazont2s;
        private Recorder recorder;
        private string latestwma = string.Empty;
        private bool IsRecorderStarted = false;
        private Random rnd = new Random();
        private string currentdtmf = string.Empty;
        private Timer timer;
        private string displayName;

        public AudioCallIVR(AudioVideoFlow flow, LoggingHandler callback)
        {
            this.flow = flow;
            displayName = flow.Call.RemoteEndpoint.Participant.DisplayName;
            this.callback = callback;
            directLine = new DirectlineEndpoint(OnMessagedReceivedFromDirectLine);
            player = new Player();
            player.AttachFlow(flow);
        }

        private void OnMessagedReceivedFromDirectLine(DirectlineMessage message)
        {

            recentMessage = message;
            recentMessage.PreviousPrompt = message.Prompt;
            
            HandlingIVRFlow(recentMessage);
        }
        private void IncrementPromptCount(DirectlineMessage message) {
            message.PromptSpokenCount = message.PromptSpokenCount + 1;
        }
        private void EndCall()
        {
            this.flow.Call.EndTerminate(this.flow.Call.BeginTerminate(null, this.flow.Call));

            callback(displayName, $"The active call terminated");
        }
        private void HandlingIVRFlow(DirectlineMessage message)
        {
            callback(displayName, $"{message.Prompt}");

            if(message.PromptSpokenCount > 2)
            {
                EndCall();
            }
            else
            {
                IncrementPromptCount(message);
            }

            if (message.PromptType == PromptType.EndDialog)
            {
                Speak(message.Prompt);
                return;
            }

            if (message.InputFormat == InputFormat.String)
            {
                Speak(message.Prompt);
                ListenAndRecord();
                timer.Enabled = true;
                return;
            }

            if (message.InputFormat == InputFormat.Number)
            {


                AttachTone();
                Speak(message.Prompt);
                timer.Enabled = true;
            }
            else
            {
                //AttachTone();
                Speak(message.Prompt);

                // ListenAndRecord();
                //timer.Start();
            }


        }
        private void ListenAndRecord()
        {
            if (recorder == null)
            {
                recorder = new Recorder();
                recorder.AttachFlow(flow);
                recorder.StateChanged += new EventHandler<RecorderStateChangedEventArgs>(Recorder_StateChanged);
                recorder.VoiceActivityChanged += Recorder_VoiceActivityChanged;
                latestwma = GetWmaFileName();
                var sink = new WmaFileSink(latestwma);
                recorder.SetSink(sink);
            }
            else
            {
                latestwma = GetWmaFileName();
                recorder.RemoveSink();
                var sink = new WmaFileSink(latestwma);
                recorder.SetSink(sink);
            }

            IsRecorderStarted = false;
            recorder.Start();

            //StartTimer();
        }

        private void Recorder_VoiceActivityChanged(object sender, VoiceActivityChangedEventArgs e)
        {
            if (!e.IsVoice)
            {

                if (recorder.State == RecorderState.Started && IsRecorderStarted)
                {

                    recorder.Stop();
                    
                    IsRecorderStarted = false;

                    ConvertWMAToWAV();

                }
            }
            else
            {
                IsRecorderStarted = true;
            }
        }
        private void ConvertWMAToWAV()
        {
            try
            {

                string wav = GetWavFileName(latestwma);

                using (var reader = new NAudio.Wave.AudioFileReader(latestwma))
                {

                    WaveFileWriter.CreateWaveFile(wav, reader);
                }

                SendAudioFIle(wav);
            }
            catch (Exception ex)
            {

            }
        }

        private void SendAudioFIle(string wav)
        {
            using (FileStream fileStream = new FileStream(wav, FileMode.Open, FileAccess.Read))
            {
                // Note for wave files, we can just send data from the file right to the server.
                // In the case you are not an audio file in wave format, and instead you have just
                // raw data (for example audio coming over bluetooth), then before sending up any 
                // audio data, you must first send up an SpeechAudioFormat descriptor to describe 
                // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
                int bytesRead = 0;
                byte[] buffer = new byte[1024];

                try
                {
                    do
                    {
                        // Get more Audio data to send into byte buffer.
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                        // Send of audio data to service. 
                        dataClient.SendAudio(buffer, bytesRead);
                    }
                    while (bytesRead > 0);
                }
                finally
                {
                    // We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
                    dataClient.EndAudio();

                }
            }
        }
        private void Recorder_StateChanged(object sender, RecorderStateChangedEventArgs e)
        {

        }

        public void Speak(string text)
        {
            if (amazont2s == null)
                amazont2s = new AmazonText2Speech();
            // BingTextToSpeech = new BingTextToSpeechApi();

            // var respone = BingTextToSpeech.Speak(text);
            var response = amazont2s.Speak(text);



            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                //var httpStream = respone.Content.ReadAsStreamAsync().Result;
                var httpStream = response.AudioStream;

                string wav = GetNewWavFIleName();

                using (Stream sw = File.Create(wav))
                {
                    httpStream.CopyTo(sw);
                }


                WmaFileSource source = new WmaFileSource(wav);
                source.EndPrepareSource(source.BeginPrepareSource(MediaSourceOpenMode.Buffered, null, source));
                player.SetSource(source);
                player.Start();

                while (player.State != PlayerState.Stopped)
                {

                }
            }
        }
        public void Start()
        {

            /* speechSynthesisConnector = new SpeechSynthesisConnector();
             speechSynthesisConnector.AttachFlow(flow);

             speechSynthesizer = new SpeechSynthesizer();
             audioformat = new SpeechAudioFormatInfo(16000, AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono);
             speechSynthesizer.SetOutputToAudioStream(speechSynthesisConnector, audioformat);
             speechSynthesizer.SelectVoice("Microsoft Hazel Desktop"); //slightly more english    
             speechSynthesisConnector.Start();*/

            dataClient = SpeechRecognitionServiceFactory.CreateDataClient(SpeechRecognitionMode.ShortPhrase, MyAppSettings.BotLanguage.Local, ConfigurationSettings.AppSettings["subscriptionkey"]);

            dataClient.OnResponseReceived += DataClient_OnResponseReceived;
            dataClient.OnConversationError += DataClient_OnConversationError;
            dataClient.OnPartialResponseReceived += DataClient_OnPartialResponseReceived;
            conversationId = directLine.NewConversation();

            directLine.SendActivity("Hello", conversationId);

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 10000;
//            timer.Enabled = false;
        }

        private void DataClient_OnPartialResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {

        }

        private void DataClient_OnConversationError(object sender, SpeechErrorEventArgs e)
        {

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            var time = (Timer)sender;
            timer.Enabled = false;

            var tempStr = string.Empty;



            if (recentMessage.InputFormat == InputFormat.Number)
            {
                DetachTone();

                if (string.IsNullOrEmpty(currentdtmf))
                {
                    recentMessage.Prompt = "Sorry no input from you. " + recentMessage.PreviousPrompt;
                    HandlingIVRFlow(recentMessage);
                }
                else
                {
                    currentdtmf = string.Empty;

                    if (recentMessage.PromptType == PromptType.AccountNumber)
                    {

                        recentMessage.Prompt = GetSorryPrompt(recentMessage);
                        HandlingIVRFlow(recentMessage);
                    }
                    else if (recentMessage.PromptType == PromptType.CreditCard)
                    {
                        recentMessage.Prompt = GetSorryPrompt(recentMessage);
                        HandlingIVRFlow(recentMessage);
                    }
                    else if (recentMessage.PromptType == PromptType.ExpirationDate)
                    {
                        recentMessage.Prompt = GetSorryPrompt(recentMessage);
                        HandlingIVRFlow(recentMessage);
                    }
                    else if (recentMessage.PromptType == PromptType.CCV)
                    {
                        recentMessage.Prompt = GetSorryPrompt(recentMessage);
                        HandlingIVRFlow(recentMessage);
                    }
                    else if (recentMessage.PromptType == PromptType.Pin)
                    {
                        recentMessage.Prompt = GetSorryPrompt(recentMessage);
                        HandlingIVRFlow(recentMessage);
                    }
                }
            }
            else if (recentMessage.InputFormat == InputFormat.String)
            {
                recorder.Stop();

                recentMessage.Prompt = "Sorry no input from you. " + recentMessage.PreviousPrompt;
                HandlingIVRFlow(recentMessage);
            }
        }

        private void AttachTone()
        {
            tone = new ToneController();

            tone.AttachFlow(flow);

            // Subscribe to callback to receive tones
            tone.ToneReceived += new EventHandler<ToneControllerEventArgs>(tone_ToneReceived);

            // Subscribe to callback to receive fax tones
            tone.IncomingFaxDetected += new EventHandler<IncomingFaxDetectedEventArgs>(tone_IncomingFaxDetected);
        }

        private void DetachTone()
        {

            //tone.AttachFlow(flow);

            // Subscribe to callback to receive tones
            tone.ToneReceived -= new EventHandler<ToneControllerEventArgs>(tone_ToneReceived);

            // Subscribe to callback to receive fax tones
            tone.IncomingFaxDetected -= new EventHandler<IncomingFaxDetectedEventArgs>(tone_IncomingFaxDetected);

            tone.DetachFlow();
        }
        private void tone_IncomingFaxDetected(object sender, IncomingFaxDetectedEventArgs e)
        {

        }

        private void tone_ToneReceived(object sender, ToneControllerEventArgs e)
        {

            currentdtmf = currentdtmf + Convert.ToString(e.Tone);

            var patten = @"\d{" + Convert.ToString(recentMessage.Length) + "}";

            Regex ex = new Regex(patten);

            if (ex.IsMatch(currentdtmf))
            {
                timer.Enabled = false;
                DetachTone();
                directLine.SendActivity(currentdtmf, conversationId);
                callback(displayName, $"{currentdtmf}");
                currentdtmf = string.Empty;
            }
          
        }

        private void DataClient_OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            timer.Enabled = false;

            for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
            {
                callback(displayName, $"Confidence={e.PhraseResponse.Results[i].Confidence}, Text={e.PhraseResponse.Results[i].DisplayText}");

                string output = e.PhraseResponse.Results[i].InverseTextNormalizationResult;

                Confidence conf = e.PhraseResponse.Results[i].Confidence;


                callback(displayName, $"Directline input: {output}");

                directLine.SendActivity(output, conversationId);

            }
        }


        public string GetWavFileName(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);

            var dir = GetWorkingDirectory() + "\\Wav";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = dir + "\\" + filename + ".wav";

            return dir;
        }
        private string GetWmaFileName()
        {
            var dir = GetWorkingDirectory() + "\\Recorder";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = dir + "\\" + GetRandomNumber() + ".wma";
            return dir;
        }
        private string GetNewWavFIleName()
        {
            var dir = GetWorkingDirectory() + "\\TextToSpeech";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = dir + "\\" + GetRandomNumber() + ".wav";
            return dir;
        }
        private string GetRandomNumber() => rnd.Next(1000, 1000000).ToString();

        private string GetWorkingDirectory() => AppDomain.CurrentDomain.BaseDirectory;




        public string GetSorryPrompt(DirectlineMessage message)
        {
            string inputType = message.PromptType.ToString();

            string errorMessage = $"Please enter valid {message.Length} digit {Slit(inputType)}.";

            return errorMessage;
        }
        private string Slit(string source)
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z])"));
        }
    }
}

