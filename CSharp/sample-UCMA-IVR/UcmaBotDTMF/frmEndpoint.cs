using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using UcmaBotDtmf.Common;
using UcmaBotDtmf.Helpers;
using UcmaBotDtmf.Models;
using UcmaBotDtmf.Repositories;

namespace UcmaBotDtmf
{
    public partial class frmEndpoint : Form
    {
        public frmEndpoint()
        {
            InitializeComponent();
        }
        private LyncUser endpoint;
        private DirectlineEndpoint directLine;
        private LanguageRepository languageRep;
        private static ActiveCallRepository activeCallRep;
        private void btnStart_Click(object sender, EventArgs e)
        {

            var language = (BotLanguageModel)cmbLanguages.SelectedItem;

            if (language == null)
                return;

            if (string.IsNullOrEmpty(language.Secret))
            {
                MessageBox.Show("Directline secret are required");
                return;
            }

            txtTraceOutput.Text = "";
            MyAppSettings.BotLanguage = language;            
            btnStart.Enabled = false;
            TextBox.CheckForIllegalCrossThreadCalls = false;
            endpoint.AudioVideoCallReceived += Endpoint_AudioVideoCallReceived;
            endpoint.Start();

            btnStop.Enabled = true;

            cmbActiveCalls.Enabled = true;

            //LogMessage("Standalone application started successfully...");
        }

        private void Endpoint_AudioVideoCallReceived(object sender, CallReceivedEventArgs<AudioVideoCall> e)
        {
           // LogMessage("Incoming call received...");

           

            e.Call.AudioVideoFlowConfigurationRequested += Call_AudioVideoFlowConfigurationRequested;

            e.Call.StateChanged += Call_StateChanged;

            e.Call.BeginAccept((ar) =>
            {
                e.Call.EndAccept(ar);

                //LogMessage("Incoming call accepted...");

                AddCallInActiveCall(e.Call, cmbActiveCalls);

            }, e.Call);
        }
        public static void AddCallInActiveCall(AudioVideoCall call, ComboBox cb)
        {
           var active = new ActiveCallModel() { DisplayName = call.Conversation.RemoteParticipants[0].DisplayName, CallTime = DateTime.Now, PhoneUri = call.Conversation.RemoteParticipants[0].PhoneUri, SipUri = call.Conversation.RemoteParticipants[0].Uri };

            activeCallRep.Add(active);

            PopulateActiveCall(cb);
        }

        private  static void PopulateActiveCall(ComboBox cb)
        {
            cb.Text = string.Empty;

            cb.DisplayMember = "DisplayName";

            cb.DataSource = activeCallRep.GetActiveCalls();

          
        }
        private void Call_StateChanged(object sender, CallStateChangedEventArgs e)
        {
            if(e.State == CallState.Terminated)
            {

                var call = (AudioVideoCall)sender;

                activeCallRep.DeleteCall(call.RemoteEndpoint.Participant.DisplayName);

                PopulateActiveCall(cmbActiveCalls);

            }
        }

        private void Call_AudioVideoFlowConfigurationRequested(object sender, AudioVideoFlowConfigurationRequestedEventArgs e)
        {
            e.Flow.StateChanged += Flow_StateChanged;
        }

        private void Flow_StateChanged(object sender, MediaFlowStateChangedEventArgs e)
        {
            if (e.State == Microsoft.Rtc.Collaboration.MediaFlowState.Active)
            {
                var flow = sender as AudioVideoFlow;

                var ivr = new AudioCallIVR(flow, LogMessage);

                ivr.Start();
            }
        }

        public void LogMessage(string displayName, string message)
        {

            string row = $"{message}{Environment.NewLine}";

            var call = (ActiveCallModel)cmbActiveCalls.SelectedItem;

            if (call == null)
                return;

            if (call.DisplayName != displayName)
                return;

            txtTraceOutput.AppendText(row);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmEndpoint_Load(object sender, EventArgs e)
        {
            lblCallDetails.Text = string.Empty;

            TextBox.CheckForIllegalCrossThreadCalls = false;

            languageRep = new LanguageRepository();

            activeCallRep = new ActiveCallRepository();

            cmbLanguages.DisplayMember = "Language";

            cmbLanguages.DataSource = languageRep.GetBotLanguages();

            txtDirectlineSecret.Text = ConfigurationManager.AppSettings["DirectLineSecret"];

            endpoint = new LyncUser();

            //txtSipUri.Text = endpoint.SipUri + ":" + endpoint.PortNumber;

            txtSipUri.Text = ConfigurationManager.AppSettings["uri"];

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            endpoint.Stop();
            btnStart.Enabled = true;           
            txtDirectlineSecret.Enabled = true;
            btnStop.Enabled = false;            
            //LogMessage("Endpoint stopped successfully...");
        }

       

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbox = (ComboBox)sender;

            var language = (BotLanguageModel)cbox.SelectedItem;

            if (language == null)
                return;

            txtDirectlineSecret.Text = language.Secret;

        }

        private void cmbActiveCalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;

            lblCallDetails.Text = string.Empty;

            if (cb.SelectedItem == null)
                return;

            var call = (ActiveCallModel)cb.SelectedItem;

            if (call == null)
                return;

            lblCallDetails.Text = $"Name - {call.DisplayName}{Environment.NewLine}Uri - {call.SipUri}{Environment.NewLine}Phone Uri - {call.PhoneUri} {Environment.NewLine}TIme - {call.CallTime.ToString("hh:mm tt")}";

            txtTraceOutput.Text = string.Empty;
        }
    }
}
