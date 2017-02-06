using System;
using System.Configuration;
using System.Net;
using System.Threading;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Collaboration.AudioVideo;
using Microsoft.Rtc.Signaling;

namespace UcmaBotDtmf.Helpers
{
    public class UCMAStandalone
    {
        private static readonly AutoResetEvent autoEvent = new AutoResetEvent(false);

        private ApplicationEndpoint endpoint;
        private ApplicationEndpointSettings endpointSettings;

        private CollaborationPlatform platform;
        public int PortNumber => Convert.ToInt32(ConfigurationManager.AppSettings["appPort"]);
        private string HostName => Dns.GetHostEntry("localhost").HostName;
        public string SipUri => $"sip:default@{HostName}";

        private void StartPlatform()
        {
            Console.WriteLine($"Standalone UCMA listening port {PortNumber}");

            ServerPlatformSettings settings = new ServerPlatformSettings("standalone", HostName, PortNumber, string.Empty);

            platform = new CollaborationPlatform(settings);

            try
            {
                platform.BeginStartup(PlatformStartedCallback, platform);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void PlatformStartedCallback(IAsyncResult ar)
        {
            try
            {
                platform.EndStartup(ar);

                Console.WriteLine("Platform started.");

                StartEndpoint();
            }
            catch (RealTimeException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void StartEndpoint()
        {
            //string sip = $"sip:default@{Dns.GetHostEntry("localhost").HostName}";

            Console.WriteLine($"Endpoint Uri: {SipUri}");
            // Create a placeholder URI for the endpoint.
            endpointSettings = new ApplicationEndpointSettings(SipUri);
            // Make this a default routing endpoint, so that
            // all requests sent to the listening port on this IP,
            // regardless of To URI, will come to the endpoint.
            endpointSettings.IsDefaultRoutingEndpoint = true;

            // Create a new endpoint and register for AV calls.
            endpoint = new ApplicationEndpoint(platform, endpointSettings);

            endpoint.RegisterForIncomingCall<AudioVideoCall>(OnCallReceived);

            try
            {
                endpoint.BeginEstablish(EndpointEstablishedCallback, endpoint);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void EndpointEstablishedCallback(IAsyncResult ar)
        {
            try
            {
                endpoint.EndEstablish(ar);

                //Console.WriteLine("Endpoint started.");

                autoEvent.Set();
            }
            catch (RealTimeException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnCallReceived(object sender, CallReceivedEventArgs<AudioVideoCall> e)
        {
            Console.WriteLine("Incoming call received");

            AudioVideoCallReceived?.Invoke(sender, e);
        }

        private void EndpointTerminatedCallback(IAsyncResult ar)
        {
            endpoint.EndTerminate(ar);

            Console.WriteLine("Endpoint terminated.");

            ShutDownPlatform();
        }

        private void ShutDownPlatform()
        {
            try
            {
                platform.BeginShutdown(PlatformShutdownCallback, platform);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void PlatformShutdownCallback(IAsyncResult ar)
        {
            platform.EndShutdown(ar);

            //Console.WriteLine("Platform shut down.");

            autoEvent.Set();
        }

        public event EventHandler<CallReceivedEventArgs<AudioVideoCall>> AudioVideoCallReceived = delegate { };

        public void Start()
        {
            StartPlatform();

            autoEvent.WaitOne();
        }

        public void Stop()
        {
            endpoint.BeginTerminate(EndpointTerminatedCallback, endpoint);

            autoEvent.WaitOne();
        }
    }
}