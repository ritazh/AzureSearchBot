using System;
using System.Configuration;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;

namespace UcmaBotDtmf.Helpers
{
    public class LyncUser
    {
      

        public event EventHandler<EventArgs> LyncServerReady = delegate { };
        public event EventHandler<CallReceivedEventArgs<InstantMessagingCall>> NewInstantMessagingCallReceived = delegate { };
        public event EventHandler<InstantMessageReceivedEventArgs> InstantMessageReceived = delegate { };
        public event EventHandler<CallReceivedEventArgs<AudioVideoCall>> AudioVideoCallReceived = delegate { };
        private UserEndpoint _endpoint; 
        private UCMASampleHelper helper;
        public void Start()
        {
             helper = new UCMASampleHelper();

            var userEndpointSettings = helper.ReadUserSettings(ConfigurationSettings.AppSettings["username"]);

            userEndpointSettings.AutomaticPresencePublicationEnabled = true;

            _endpoint = helper.CreateUserEndpoint(userEndpointSettings);                    

            _endpoint.RegisterForIncomingCall<InstantMessagingCall>(InstantMessagingCallReceived);

            _endpoint.RegisterForIncomingCall<AudioVideoCall>(OnAudioVideoCallReceived);

            helper.EstablishUserEndpoint(_endpoint);       

            
          

            LyncServerReady(this, new EventArgs());

        }

        private void OnAudioVideoCallReceived(object sender, CallReceivedEventArgs<AudioVideoCall> e)
        {
            AudioVideoCallReceived(sender, e);
        }

        private void InstantMessagingCallReceived(object sender, CallReceivedEventArgs<InstantMessagingCall> e)
        {
            e.Call.InstantMessagingFlowConfigurationRequested += Call_InstantMessagingFlowConfigurationRequested;

            e.Call.BeginAccept((ar) =>
            {
                e.Call.EndAccept(ar);
            }, e.Call);

            NewInstantMessagingCallReceived(sender, e);
        }

        private void Call_InstantMessagingFlowConfigurationRequested(object sender, InstantMessagingFlowConfigurationRequestedEventArgs e)
        {
            e.Flow.MessageReceived += Flow_MessageReceived;
        }

        private void Flow_MessageReceived(object sender, InstantMessageReceivedEventArgs e)
        {
            InstantMessageReceived(sender, e);
            
        }

       public void Stop()
        {
            helper.ShutdownPlatform();

            _endpoint.EndTerminate(_endpoint.BeginTerminate(null, _endpoint));
        }
    }
}
