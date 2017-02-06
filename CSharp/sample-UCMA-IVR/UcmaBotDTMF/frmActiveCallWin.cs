using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using UcmaBotDtmf.Common;
using UcmaBotDtmf.Helpers;
using UcmaBotDtmf.Models;
using UcmaBotDtmf.Repositories;

namespace UcmaBotDtmf
{

    public enum EndpointStatus
    {
        Stopped,
        Started
    }
    public partial class frmActiveCallWin : Form
    {
        public frmActiveCallWin()
        {
            InitializeComponent();
        }

       

        public LanguageRepository languageRep;    

        public BotLanguageModel currentLanguage;

        private UCMAStandalone endpoint;

        private EndpointStatus CurrentEndpointStatus;

        private ActiveCallModel cac4Debug;

        public static bool isDataGridLoading;

        private frmSettings settingsFrm;
        private void frmActiveCallWin_Load(object sender, EventArgs e)
        {
            ttsBing.Checked = true;

            MyAppSettings.ProjectTitle = ConfigurationManager.AppSettings["projectTitle"];

            this.Text = MyAppSettings.ProjectTitle;

            TextBox.CheckForIllegalCrossThreadCalls = false;

            languageRep = new LanguageRepository();          

            currentLanguage = languageRep.GetLanguage(ConfigurationManager.AppSettings["defaultLanguage"]);

            changeLanguage(currentLanguage);

            CurrentEndpointStatus = EndpointStatus.Stopped;

            addColumns();
        }

       

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
           
        }

        private void tbs_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

           
        }

        private void tsmiEnglish_Click(object sender, EventArgs e)
        {
            var tsi = (ToolStripItem)sender;

            if(CurrentEndpointStatus == EndpointStatus.Started)
            {
                MessageBox.Show("You can't change bot language because the endpoint is running", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentLanguage = languageRep.GetLanguage(tsi.Text);
            changeLanguage(currentLanguage);
        }

        private void tsmiSpanish_Click(object sender, EventArgs e)
        {
           
            var tsi = (ToolStripItem)sender;
            if (CurrentEndpointStatus == EndpointStatus.Started)
            {
                MessageBox.Show("You can't change bot language because the endpoint is running", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            currentLanguage = languageRep.GetLanguage(tsi.Text);
            changeLanguage(currentLanguage);
        }

        private void tsmiStart_Click(object sender, EventArgs e)
        {
            if (CurrentEndpointStatus == EndpointStatus.Started)
                return;

            MyAppSettings.BotLanguage = currentLanguage;

            TextBox.CheckForIllegalCrossThreadCalls = false;

            DataGridView.CheckForIllegalCrossThreadCalls = false;

            Label.CheckForIllegalCrossThreadCalls = false;

            endpoint = new UCMAStandalone();

            endpoint.AudioVideoCallReceived += Endpoint_AudioVideoCallReceived;

            endpoint.Start();

            tsslEndpointUri.Text = "Endpoint Uri : " + endpoint.SipUri;

            CurrentEndpointStatus = EndpointStatus.Started;

            updateEndpointStatus(CurrentEndpointStatus);
        }

        private void Endpoint_AudioVideoCallReceived(object sender, CallReceivedEventArgs<AudioVideoCall> e)
        {
            //var call = sender as AudioVideoCall;

            if (e.Call.State != CallState.Incoming)
                return;


            addActiveCall(e.Call);

            e.Call.AudioVideoFlowConfigurationRequested += Call_AudioVideoFlowConfigurationRequested;

            e.Call.StateChanged += Call_StateChanged;


           
            e.Call.BeginAccept((ar) =>
            {

                if(e.Call.State == CallState.Establishing)
                {
                    e.Call.EndAccept(ar);
                }              

            }, e.Call);
        }

        private void Call_StateChanged(object sender, CallStateChangedEventArgs e)
        {
            var call = sender as AudioVideoCall;

            if (e.State == CallState.Terminated)
            {
                //var call = (AudioVideoCall)sender;
                isDataGridLoading = true;

                string displayName = call.RemoteEndpoint.Participant.DisplayName;

                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = call.RemoteEndpoint.Uri;
                }

                NewActiveCallRepository.DeleteCall(displayName);

                loadActiveCalls();
            }
            else if(e.State == CallState.Establishing)
            {
               
            }
            else if(e.State == CallState.Established)
            {
                if(call.Flow.State == MediaFlowState.Active)
                {                   

                /*    var remoteCall = NewActiveCallRepository.GetActiveCall(call.RemoteEndpoint.Uri);

                    if (remoteCall == null)
                        return;

                    if(dgvActiveCalls.Rows.Count == 1)
                    {
                        remoteCall.IVR.CanLog = true;
                    }

                    remoteCall.IVR.Start(call.Flow, logMessage);*/
                }
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

                var remoteCall = NewActiveCallRepository.GetActiveCall(flow.Call.RemoteEndpoint.Uri);

                if (remoteCall == null)
                    return;


            
                

                remoteCall.IVR.Start(flow, logMessage);
            }

            logMessage("Tester", $" Media Element State : {Convert.ToString(e.State)}");
        }
        public void logMessage(string displayName, string message)
        {
            var row = $"Caller Name : {displayName} - {message} {Environment.NewLine}";

            txtTraceOutput.AppendText(row);
        }
        private void tsmiStop_Click(object sender, EventArgs e)
        {

            if(dgvActiveCalls.Rows.Count > 0)
            {
                MessageBox.Show($"You can't shutdown standalone application. Because there are {dgvActiveCalls.Rows.Count} active calls", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (CurrentEndpointStatus == EndpointStatus.Stopped)
                return;

            endpoint.Stop();

            CurrentEndpointStatus = EndpointStatus.Stopped;

            updateEndpointStatus(CurrentEndpointStatus);

            txtTraceOutput.Text = "";

        }
        private void updateEndpointStatus(EndpointStatus es )
        {
            if(es == EndpointStatus.Started)
            {
                tsmiStart.Checked = true;
                tsmiStop.Checked = false;
                tsslIvrStatus.Text = "Ivr Staus : Started";
            }
            else if(es == EndpointStatus.Stopped)
            {
                tsmiStart.Checked = false;
                tsmiStop.Checked = true;
                tsslIvrStatus.Text = "Ivr Staus : Stopped";
            }
        }
        private void addColumns()
        {
            DataGridViewColumn callerName = new DataGridViewTextBoxColumn();
            callerName.HeaderText = "Caller Name";
            callerName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            callerName.DataPropertyName = "DisplayName";
            callerName.Width = 120;            
            dgvActiveCalls.Columns.Add(callerName);

            DataGridViewColumn sipUri = new DataGridViewTextBoxColumn();
            sipUri.HeaderText = "Uri";
            sipUri.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            sipUri.Width = 200;
            sipUri.DataPropertyName = "SipUri";
            dgvActiveCalls.Columns.Add(sipUri);

            DataGridViewColumn phoneUri = new DataGridViewTextBoxColumn();
            phoneUri.HeaderText = "Phone Uri";
            phoneUri.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            phoneUri.Width = 170;
            phoneUri.DataPropertyName = "PhoneUri";
            dgvActiveCalls.Columns.Add(phoneUri);

            DataGridViewColumn time = new DataGridViewTextBoxColumn();
            time.HeaderText = "Time";
            time.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            time.Width = 100;
            time.DataPropertyName = "CallTime";
            dgvActiveCalls.Columns.Add(time);
            dgvActiveCalls.AutoGenerateColumns = false;
        }
        private void addActiveCall(AudioVideoCall call)
        {
            isDataGridLoading = true;

            string displayName = call.RemoteEndpoint.Participant.DisplayName;

            if (string.IsNullOrEmpty(displayName))
            {
                displayName = call.RemoteEndpoint.Uri;
            }
            var active = new ActiveCallModel() { DisplayName = displayName, CallTime = DateTime.Now, PhoneUri = call.RemoteEndpoint.Participant.PhoneUri, SipUri = call.RemoteEndpoint.Uri, AVCall = call, IVR = new NewAudioCallIVR() };

            NewActiveCallRepository.Add(active);

            loadActiveCalls();
        }

        private void deleteActiveCall(AudioVideoCall call)
        {
            //var active = new ActiveCallModel() { DisplayName = call.Conversation.RemoteParticipants[0].DisplayName, CallTime = DateTime.Now, PhoneUri = call.Conversation.RemoteParticipants[0].PhoneUri, SipUri = call.Conversation.RemoteParticipants[0].Uri };

           // NewActiveCallRepository.Add(active);

            loadActiveCalls();
        }
        public void loadActiveCalls()
        {

            this.Invoke((MethodInvoker)delegate
            {
                dgvActiveCalls.DataSource = NewActiveCallRepository.GetActiveCalls();

                tsslActiveCalls.Text = "Active Calls : " + Convert.ToString(dgvActiveCalls.Rows.Count);

                if (dgvActiveCalls.Rows.Count == 0)
                {
                    cac4Debug = null;
                    return;
                }

                if (cac4Debug == null)
                {
                    cac4Debug = NewActiveCallRepository.GetActiveCalls().FirstOrDefault();

                    if (cac4Debug == null)
                        return;

                    cac4Debug.IVR.CanLog = true;
                }
                else
                {
                    var debguCall = NewActiveCallRepository.GetActiveCall(cac4Debug.SipUri);

                    if (debguCall != null)
                        debguCall.IVR.CanLog = true;
                }

                isDataGridLoading = false;

            });



           
        }

        private void  changeLanguage(BotLanguageModel botLanguage)
        {
            if(botLanguage.Language == "English")
            {
                tsmiEnglish.Checked = true;
                tsmiSpanish.Checked = false;
                tsslLanguage.Text = "Language : English";
            }
            else if (botLanguage.Language == "Spanish")
            {
                tsmiEnglish.Checked = false;
                tsmiSpanish.Checked = true;
                tsslLanguage.Text = "Language : Spanish";
            }
        }

        private void dgvActiveCalls_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void dgvActiveCalls_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected)
                return;

            if (!isDataGridLoading)
            {
                var data = (ActiveCallModel)e.Row.DataBoundItem;

                if (data == null)
                    return;

                txtTraceOutput.Text = "";

                if(cac4Debug != null)
                {
                    cac4Debug.IVR.CanLog = false;
                    
                }
                data.IVR.CanLog = true;
                cac4Debug = data;
            }
            




        }

        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            settingsFrm = new frmSettings();

            var result = settingsFrm.ShowDialog();

            if(result == DialogResult.OK)
            {
                if(CurrentEndpointStatus == EndpointStatus.Started)
                {
                    MessageBox.Show("You can't update  bot secret. Because the endpoint is running", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var languages = languageRep.GetBotLanguages();

                var strLanguage = settingsFrm.cmbLanguages.Text;

                var languageToBeUpdate = languages.FirstOrDefault(l => l.Language.ToLower() == strLanguage.ToLower());

                if (languageToBeUpdate == null)
                    return;

                languageToBeUpdate.Local = settingsFrm.txtLocal.Text;

                languageToBeUpdate.Secret = settingsFrm.txtSecret.Text;

                languageRep.Update(languages);

                currentLanguage = languageRep.GetBotLanguages().FirstOrDefault(l => l.Language.ToLower() == currentLanguage.Language.ToLower());
            }
        }

        private void tsmiQuit_Click(object sender, EventArgs e)
        {
            if (dgvActiveCalls.Rows.Count > 0)
            {
                MessageBox.Show($"You can't close this application. Because there are {dgvActiveCalls.Rows.Count} active calls",MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (CurrentEndpointStatus == EndpointStatus.Started)
            {
                endpoint.Stop();
            }
            Application.Exit();
        }

        private void ttsBing_Click(object sender, EventArgs e)
        {
            MyAppSettings.IsAmazonText2Speechenabled = false;

            ttsBing.Checked = true;
            tsmnAmazon.Checked = false;
        }

        private void tsmnAmazon_Click(object sender, EventArgs e)
        {
            MyAppSettings.IsAmazonText2Speechenabled = true;

            ttsBing.Checked = false;
            tsmnAmazon.Checked = true;
        }

        private void tsmiEndpoint_Click(object sender, EventArgs e)
        {

        }
    }

    
}
