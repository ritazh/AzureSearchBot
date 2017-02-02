/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

using System;
using System.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;
using System.Configuration;


namespace UcmaBotDTMF
{
    public class UCMASampleHelper
    {
        private static ManualResetEvent _sampleFinished = new ManualResetEvent(false);

        // The name of this application, to be used as the outgoing user agent string.
        // The user agent string is put in outgoing message headers to indicate the Application used.
        private static string _applicationName = "UCMASampleCode";

        const string _sipPrefix = "sip:";

        // These strings will be used as keys into the App.Config file to get information to avoid prompting. For most of these,
        // Suffixes 1-N will be used on each subsequent call. eg. UserName1 will be used for the first user, UserName2 for the second, etc.
        private static String _serverFQDNPrompt = "ServerFQDN";
        private static String _userNamePrompt = "UserName";
        private static String _userDomainPrompt = "UserDomain";
        private static String _userURIPrompt = "UserURI";

        // Construct the network credential that the UserEndpoint will use to authenticate to the Microsoft Lync Server.
        private string _userName; // = "acd"; // User name and password pair of a user enabled for Microsoft Lync Server. 
        private string _userPassword; // = "aPkm135!";
        private string _userDomain; // = "lynclabs.com"; // Domain that this user is logging into. Note: This is the AD domain, not the portion of the SIP URI following the at sign.
        private System.Net.NetworkCredential _credential;

        // The URI and connection server of the user used.
        private string _userURI;// = "sip:acd@lynclabs.com"; // This should be the URI of the user given above.

        // The Server FQDN used.
        private static string _serverFqdn;// = "2013std.lynclabs.com";// The Microsoft Lync Server to log in to.

        // Transport type used to communicate with your Microsoft Lync Server instance.
        private Microsoft.Rtc.Signaling.SipTransportType _transportType = Microsoft.Rtc.Signaling.SipTransportType.Tls;

        private static CollaborationPlatform _collabPlatform;
        private static bool _isPlatformStarted;
        private static CollaborationPlatform _serverCollabPlatform;
        private AutoResetEvent _platformStartupCompleted = new AutoResetEvent(false);
        private AutoResetEvent _endpointInitCompletedEvent = new AutoResetEvent(false);
        private AutoResetEvent _platformShutdownCompletedEvent = new AutoResetEvent(false);
        private UserEndpoint _userEndpoint;
        private ApplicationEndpoint _applicationEndpoint;

        // The FQDN of the machine that the application contact is targeted at.
        private string _applicationHostFQDN;

        // The port that the application contact is configured to receive data on.
        private int _applicationPort = 6099;

        // The GRUU assigned to the application contact.
        private string _applicationGruu = "sip:dev02.lynclabs.com@lynclabs.com;gruu;opaque=srvr:ucma01:PX8uaJlGKlOeOodynJx1VwAA";

        // The URI of the contact being used.
        private string _applicationContactURI;

        private string _certificateFriendlyName = "dev01";

        private bool _useSuppliedCredentials;
        private static int _appContactCount;
        private static int _userCount = 1;

        // Method to read user settings from app.config file or from the console prompts
        // This method returns a UserEndpointSettings object. If you do not want to monitor LocalOwnerPresence, you may 
        // want to call the CreateEstablishedUserEndpoint method directly. Otherwise, you may call ReadUserSettings
        // followed by CreateUserEndpoint, followed by EstablishUserEndpoint methods.

        public UCMASampleHelper()
        {
            _userName = ConfigurationSettings.AppSettings["username"];
            _userPassword = ConfigurationSettings.AppSettings["password"];
            _userDomain = ConfigurationSettings.AppSettings["domain"];
            _userURI = ConfigurationSettings.AppSettings["uri"];
            _serverFqdn = ConfigurationSettings.AppSettings["fqdn"];
          

        }
        public UserEndpointSettings ReadUserSettings(string userFriendlyName)
        {

            
            UserEndpointSettings userEndpointSettings = null;

            /*
            string prompt = string.Empty;
            if (string.IsNullOrEmpty(userFriendlyName))
            {
                userFriendlyName = "Default User";
            }

            try
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine("Creating User Endpoint for {0}...", userFriendlyName);
                Console.WriteLine();

                if (ConfigurationManager.AppSettings[_serverFQDNPrompt + _userCount] != null)
                {
                    _serverFqdn = ConfigurationManager.AppSettings[_serverFQDNPrompt + _userCount];

                    //changing this to preserve confidentiality in video demo
                    //Console.WriteLine("Using {0} as Microsoft Lync Server", _serverFqdn);
                    Console.WriteLine("Setting Microsoft Lync Server as specified in App.config");
                }
                else
                {
                    // Prompt user for server FQDN. If server FQDN was entered before, then let the user use the saved value.
                    string localServer;
                    StringBuilder promptBuilder = new StringBuilder();
                    if (!string.IsNullOrEmpty(_serverFqdn))
                    {
                        promptBuilder.Append("Current Microsoft Lync Server = ");
                        promptBuilder.Append(_serverFqdn);
                        promptBuilder.AppendLine(". Please hit ENTER to retain this setting - OR - ");
                    }

                    promptBuilder.Append("Please enter the FQDN of the Microsoft Lync Server that the ");
                    promptBuilder.Append(userFriendlyName);
                    promptBuilder.Append(" endpoint is homed on => ");
                    localServer = PromptUser(promptBuilder.ToString(), null);

                    if (!String.IsNullOrEmpty(localServer))
                    {
                        _serverFqdn = localServer;
                    }
                }

                // Prompt user for user name
                prompt = String.Concat("Please enter the User Name for ",
                                        userFriendlyName,
                                        " (or hit the ENTER key to use current credentials)\r\n" +
                                        "Please enter the User Name => ");
                _userName = PromptUser(prompt, _userNamePrompt + _userCount);

                // If user name is empty, use current credentials
                if (string.IsNullOrEmpty(_userName))
                {
                    Console.WriteLine("Username was empty - using current credentials...");
                    _useSuppliedCredentials = true;
                }
                else
                {
                    // Prompt for password
                    prompt = String.Concat("Enter the User Password for ", userFriendlyName, " => ");
                    _userPassword = PromptUser(prompt, null);

                    prompt = String.Concat("Please enter the User Domain for ", userFriendlyName, " => ");
                    _userDomain = PromptUser(prompt, _userDomainPrompt + _userCount);
                }

                // Prompt user for user URI
                prompt = String.Concat("Please enter the User URI for ", userFriendlyName, " in the User@Host format => ");
                _userURI = PromptUser(prompt, _userURIPrompt + _userCount);
                if (!(_userURI.ToLower().StartsWith("sip:") || _userURI.ToLower().StartsWith("tel:")))
                    _userURI = "sip:" + _userURI;

                // Increment the last user number
                _userCount++;

                // Initalize and register the endpoint, using the credentials of the user the application will be acting as.
                // NOTE: the _userURI should always be of the form "sip:user@host"

            */




                userEndpointSettings = new UserEndpointSettings(_userURI, _serverFqdn);

                if (!_useSuppliedCredentials)
                {
                    _credential = new System.Net.NetworkCredential(_userName, _userPassword, _userDomain);
                    userEndpointSettings.Credential = _credential;
                }
                else
                {
                    userEndpointSettings.Credential = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
           // }
           // catch (InvalidOperationException iOpEx)
            //{
                // Invalid Operation Exception should only be thrown on poorly-entered input.
              //  Console.WriteLine("Invalid Operation Exception: " + iOpEx.ToString());
            //}

            return userEndpointSettings;
        }

        // Method to create an endpoint given a UserEndpointSettings object.
        // This method returns a UserEndpoint object so that you can wire up Endpoint-specific event handlers. 
        // If you do not want to get endpoint specific event information at the time the endpoint is established, you may 
        // want to call the CreateEstablishedUserEndpoint method directly. Otherwise, you may call ReadUserSettings
        // followed by CreateUserEndpoint, followed by EstablishUserEndpoint methods.
        public UserEndpoint CreateUserEndpoint(UserEndpointSettings userEndpointSettings)
        {
            // Reuse platform instance so that all endpoints share the same platform.
            if (_collabPlatform == null)
            {
                // Initalize and startup the platform.
                ClientPlatformSettings clientPlatformSettings = new ClientPlatformSettings(_applicationName, _transportType);
                _collabPlatform = new CollaborationPlatform(clientPlatformSettings);
            }

            _userEndpoint = new UserEndpoint(_collabPlatform, userEndpointSettings);
            return _userEndpoint;
        }

        // Method to establish an already created UserEndpoint.
        // This method returns an established UserEndpoint object. If you do not want to monitor LocalOwnerPresence, you may 
        // want to call the CreateEstablishedUserEndpoint method directly. Otherwise, you may call ReadUserSettings
        // followed by CreateUserEndpoint, followed by EstablishUserEndpoint methods.
        public bool EstablishUserEndpoint(UserEndpoint userEndpoint)
        {
            // Startup the platform, if not already
            if (_isPlatformStarted == false)
            {
                userEndpoint.Platform.BeginStartup(EndPlatformStartup, userEndpoint.Platform);

                // Sync; wait for the platform startup to complete.
                _platformStartupCompleted.WaitOne();
                Console.WriteLine("Platform started...");

                

                _isPlatformStarted = true;
            }
            // Establish the user endpoint
            userEndpoint.BeginEstablish(EndEndpointEstablish, userEndpoint);

            // Sync; wait for the registration to complete.
            _endpointInitCompletedEvent.WaitOne();

            Console.WriteLine("Endpoint established...");

            
            return true;
        }

        // Method to create an established UserEndpoint.
        // This method returns an established UserEndpoint object. If you do not want to monitor LocalOwnerPresence, you may 
        // want to call this CreateEstablishedUserEndpoint method directly. Otherwise, you may call ReadUserSettings
        // followed by CreateUserEndpoint, followed by EstablishUserEndpoint methods.
        public UserEndpoint CreateEstablishedUserEndpoint(string endpointFriendlyName)
        {
            UserEndpointSettings userEndpointSettings;
            UserEndpoint userEndpoint = null;
            try
            {
                // Read user settings
                userEndpointSettings = ReadUserSettings(endpointFriendlyName);
                userEndpointSettings.AutomaticPresencePublicationEnabled = true;
                // Create User Endpoint
                userEndpoint = CreateUserEndpoint(userEndpointSettings);

                // Establish the user endpoint
                EstablishUserEndpoint(userEndpoint);
            }
            catch (InvalidOperationException iOpEx)
            {
                // Invalid Operation Exception should only be thrown on poorly-entered input.
                Console.WriteLine("Invalid Operation Exception: " + iOpEx.ToString());
            }

            return userEndpoint;

        }

        public UserEndpoint CreateUserEndpointWithServerPlatform(string endpointFriendlyName)
        {
            string prompt = string.Empty;
            if (string.IsNullOrEmpty(endpointFriendlyName))
            {
                endpointFriendlyName = "Default User";
            }

            try
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine("Creating User Endpoint for {0}...", endpointFriendlyName);
                Console.WriteLine();

                if (ConfigurationSettings.AppSettings[_serverFQDNPrompt + _userCount] != null)
                {
                    if (ReadGenericApplicationContactConfiguration())
                    {
                        Console.WriteLine("Using {0} as Microsoft Lync Server", _serverFqdn);
                    }
                    else
                    {
                        Console.WriteLine("Error. Could not read AppSettings");
                    }
                }
                else
                {
                    Console.WriteLine("Please fill in the App.config file.");
                }

                // Prompt user for user name
                prompt = String.Concat(
                    "Please enter the User Name for ",
                    endpointFriendlyName,
                    " (or hit the ENTER key to use current credentials)\r\n" +
                    "Please enter the User Name => ");
                _userName = PromptUser(prompt, _userNamePrompt + _userCount);

                // If user name is empty, use current credentials
                if (string.IsNullOrEmpty(_userName))
                {
                    Console.WriteLine("Username was empty - using current credentials...");
                    _useSuppliedCredentials = true;
                }
                else
                {
                    // Prompt for password
                    prompt = String.Concat("Enter the User Password for ", endpointFriendlyName, " => ");
                    _userPassword = PromptUser(prompt, null);

                    prompt = String.Concat("Please enter the User Domain for ", endpointFriendlyName, " => ");
                    _userDomain = PromptUser(prompt, _userDomainPrompt + _userCount);
                }

                // Prompt user for user URI
                prompt = String.Concat("Please enter the User URI for ", endpointFriendlyName, " in the User@Host format => ");
                _userURI = PromptUser(prompt, _userURIPrompt + _userCount);
                if (!(_userURI.ToLower().StartsWith("sip:") || _userURI.ToLower().StartsWith("tel:")))
                    _userURI = "sip:" + _userURI;

                // Reuse platform instance so that all endpoints share the same platform.
                if (_serverCollabPlatform == null)
                {
                    CreateAndStartServerPlatform();
                }

                // Increment the last user number
                _userCount++;

                // Initalize and register the endpoint, using the credentials of the user the application will be acting as.
                // NOTE: the _userURI should always be of the form "sip:user@host"
                UserEndpointSettings userEndpointSettings = new UserEndpointSettings(_userURI, _serverFqdn);
                if (!_useSuppliedCredentials)
                {
                    _credential = new System.Net.NetworkCredential(_userName, _userPassword, _userDomain);
                    userEndpointSettings.Credential = _credential;
                }
                else
                {
                    userEndpointSettings.Credential = System.Net.CredentialCache.DefaultNetworkCredentials;
                }

                _userEndpoint = new UserEndpoint(_serverCollabPlatform, userEndpointSettings);
                _endpointInitCompletedEvent.Reset();
                _userEndpoint.BeginEstablish(EndEndpointEstablish, _userEndpoint);

                // Sync; wait for the registration to complete.
                _endpointInitCompletedEvent.WaitOne();
                Console.WriteLine("{0} endpoint established...", endpointFriendlyName);
            }
            catch (InvalidOperationException iOpEx)
            {
                // Invalid Operation Exception should only be thrown on poorly-entered input.
                Console.WriteLine("Invalid Operation Exception: " + iOpEx.ToString());
            }

            return _userEndpoint;

        }

        private bool ReadGenericApplicationContactConfiguration()
        {
            try
            {
                _serverFqdn = ConfigurationSettings.AppSettings["ServerFQDN"];
                _certificateFriendlyName = ConfigurationSettings.AppSettings["CertificateFriendlyName"];
                _applicationHostFQDN = ConfigurationSettings.AppSettings["ApplicationHostFQDN"];
                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ApplicationPort"]))
                {
                    _applicationPort = int.Parse(ConfigurationSettings.AppSettings["ApplicationPort"], CultureInfo.InvariantCulture);
                }
                _applicationGruu = ConfigurationSettings.AppSettings["ApplicationGRUU"];
            }
            catch (Exception ex)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_serverFqdn) || string.IsNullOrEmpty(_certificateFriendlyName) ||
                (_applicationPort <= 0) || string.IsNullOrEmpty(_applicationGruu))
                return false;

            return true;
        }

        private bool ReadApplicationContactConfiguration()
        {
            _applicationPort = -1;
            ReadGenericApplicationContactConfiguration();

            try
            {
                _appContactCount++;
                string contactCount = _appContactCount.ToString(CultureInfo.InvariantCulture);

                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ApplicationHostFQDN" + contactCount]))
                {
                    _applicationHostFQDN = ConfigurationSettings.AppSettings["ApplicationHostFQDN" + contactCount];
                }

                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ApplicationPort" + contactCount]))
                {
                    _applicationPort = int.Parse(ConfigurationSettings.AppSettings["ApplicationPort" + contactCount], CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ApplicationGRUU" + contactCount]))
                {
                    _applicationGruu = ConfigurationSettings.AppSettings["ApplicationGRUU" + contactCount];
                }

                _applicationContactURI = ConfigurationSettings.AppSettings["ApplicationContactURI" + contactCount];
                if (!_applicationContactURI.Trim().StartsWith(_sipPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    _applicationContactURI = _sipPrefix + _applicationContactURI.Trim();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_serverFqdn) || string.IsNullOrEmpty(_certificateFriendlyName) || string.IsNullOrEmpty(_applicationHostFQDN) ||
                (_applicationPort <= 0) || string.IsNullOrEmpty(_applicationGruu) || string.IsNullOrEmpty(_applicationContactURI))
                return false;

            return true;
        }

        public ApplicationEndpoint CreateApplicationEndpoint(string contactFriendlyName)
        {
            string prompt = string.Empty;

           /* if (string.IsNullOrEmpty(contactFriendlyName))
            {
                contactFriendlyName = "Default Contact";
            }

            Console.WriteLine();
            Console.WriteLine("Creating Application Endpoint {0}...", contactFriendlyName);
            Console.WriteLine();

            // If application settings are provided via the app.config file, then use them
            // Else prompt user for details
            if (!ReadApplicationContactConfiguration())
            {
                // Prompt user for server FQDN. If server FQDN was entered before, then let the user use the saved value.
                if (string.IsNullOrEmpty(_serverFqdn))
                {
                    prompt = "Please enter the FQDN of the Microsoft Lync Server for this topology => ";
                    _serverFqdn = PromptUser(prompt, null);
                }

                if (string.IsNullOrEmpty(_applicationHostFQDN))
                {
                    prompt = "Please enter the FQDN of the Machine that the application service is configured to => ";
                    _applicationHostFQDN = PromptUser(prompt, null);
                }
                if (0 >= _applicationPort)
                {
                    // Prompt user for contact port
                    prompt = "Please enter the port that the application service is configured to => ";
                    _applicationPort = int.Parse(PromptUser(prompt, null), CultureInfo.InvariantCulture);
                }

                if (string.IsNullOrEmpty(_applicationGruu))
                {
                    // Prompt user for Contact GRUU
                    prompt = "Please enter the GRUU assigned to the application service => ";
                    _applicationGruu = PromptUser(prompt, null);
                }

                if (string.IsNullOrEmpty(_applicationContactURI))
                {
                    // Prompt user for contact URI
                    prompt = "Please enter the Contact URI in the User@Host format => ";
                    _applicationContactURI = PromptUser(prompt, null);
                    if (!_applicationContactURI.Trim().StartsWith(_sipPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        _applicationContactURI = _sipPrefix + _applicationContactURI.Trim();
                    }
                }

                if (string.IsNullOrEmpty(_certificateFriendlyName))
                {
                    // Prompt user for contact URI
                    prompt = "Please enter the friendly name of the certificate to be used => ";
                    _certificateFriendlyName = PromptUser(prompt, null);
                }
            }
            */
            // Reuse platform instance so that all endpoints share the same platform.
            if (_serverCollabPlatform == null)
            {
                CreateAndStartServerPlatform();
            }

            // Initalize and register the endpoint.
            // NOTE: the _applicationContactURI should always be of the form "sip:user@host"
            ApplicationEndpointSettings appEndpointSettings = new ApplicationEndpointSettings(_applicationContactURI, _serverFqdn, 5061);
            _applicationEndpoint = new ApplicationEndpoint(_serverCollabPlatform, appEndpointSettings);
            _endpointInitCompletedEvent.Reset();

            Console.WriteLine("Establishing the endpoint...");
            _applicationEndpoint.BeginEstablish(EndEndpointEstablish, _applicationEndpoint);

            // Sync; wait for the registration to complete.
            _endpointInitCompletedEvent.WaitOne();
            Console.WriteLine("Application Endpoint established...");
            return _applicationEndpoint;
        }

        private void CreateAndStartServerPlatform()
        {
            var certToUse = GetLocalCertificate(_certificateFriendlyName);

            ServerPlatformSettings serverPlatformSettings = new ServerPlatformSettings(_applicationName, _applicationHostFQDN, _applicationPort, _applicationGruu, certToUse);
            _serverCollabPlatform = new CollaborationPlatform(serverPlatformSettings);

            // Startup the platform.
            _serverCollabPlatform.BeginStartup(EndPlatformStartup, _serverCollabPlatform);

            // Sync; wait for the startup to complete.
            _platformStartupCompleted.WaitOne();
            Console.WriteLine("Platform started...");
        }

        /// <summary>
        /// If the 'key' is not found in app config, prompts the user using prompt text.
        /// </summary>
        /// <param name="promptText">If key is not found in app.Config, the user will be prompted for input using this parameter.</param>
        /// <param name="key">Searches for this key in app.Config and returns if found. Pass null to always prompt.</param>
        /// <returns>String value either from App.Config or user input.</returns>
        public static string PromptUser(string promptText, string key)
        {
            String value;
            if (String.IsNullOrEmpty(key) || ConfigurationSettings.AppSettings[key] == null)
            {
                Console.WriteLine(string.Empty);
                Console.Write(promptText);
                value = Console.ReadLine();
            }
            else
            {
                value = ConfigurationSettings.AppSettings[key];
                
                //change this to preserve confidentiality in video demo
                //Console.WriteLine("Using keypair {0} - {1} from AppSettings...", key, value);
                Console.WriteLine("Using keypair {0} from AppSettings...");
            }

            return value;
        }

        /// <summary>
        /// Displays <paramref name="textToDisplay"/> and pauses the console to for easier viewing of logs.
        /// </summary>
        /// <param name="textToDisplay">Text to display with whitespace around it.</param>
        public static void PauseBeforeContinuing(string textToDisplay)
        {
            Console.WriteLine("\n\n********************");
            Console.WriteLine(textToDisplay);
            Console.WriteLine("********************\n\n");
            Console.ReadLine();
        }

        private void EndPlatformStartup(IAsyncResult ar)
        {
            CollaborationPlatform collabPlatform = ar.AsyncState as CollaborationPlatform;
            try
            {
                // The platform should now be started.
                collabPlatform.EndStartup(ar);
                // It should be noted that all the re-thrown exceptions will crash the application. This is intentional.
                // Ideal exception handling would report the error and shut down nicely. In production code, consider using 
                // an IAsyncResult implementation to report the error instead of throwing or put the implementation
                // in this try block.
            }
            catch (OperationFailureException opFailEx)
            {
                // OperationFailureException will be thrown when the platform cannot establish, here, usually due to invalid data.
                Console.WriteLine(opFailEx.Message);
                throw;
            }
            catch (ConnectionFailureException connFailEx)
            {
                // ConnectionFailureException will be thrown when the platform cannot connect.
                // ClientPlatforms will not throw this exception on startup.
                Console.WriteLine(connFailEx.Message);
                throw;
            }
            catch (RealTimeException realTimeEx)
            {
                // RealTimeException may be thrown as a result of any UCMA operation.
                Console.WriteLine(realTimeEx.Message);
                throw;
            }
            finally
            {
                // Again, just for sync. reasons.
                _platformStartupCompleted.Set();
            }

        }

        private void EndEndpointEstablish(IAsyncResult ar)
        {
            LocalEndpoint currentEndpoint = ar.AsyncState as LocalEndpoint;
            try
            {
                currentEndpoint.EndEstablish(ar);
            }
            catch (AuthenticationException authEx)
            {
                // AuthenticationException will be thrown when the credentials are invalid.
                Console.WriteLine(authEx.Message);
                throw;
            }
            catch (ConnectionFailureException connFailEx)
            {
                // ConnectionFailureException will be thrown when the endpoint cannot connect to the server, or the credentials are invalid.
                Console.WriteLine(connFailEx.Message);
                throw;
            }
            catch (InvalidOperationException iOpEx)
            {
                // InvalidOperationException will be thrown when the endpoint is not in a valid state to connect. To connect, the platform must be started and the Endpoint Idle.
                Console.WriteLine(iOpEx.Message);
                throw;
            }
            finally
            {
                // Again, just for sync. reasons.
                _endpointInitCompletedEvent.Set();
            }
        }

        public void ShutdownPlatform()
        {
            if (_collabPlatform != null)
            {
                _collabPlatform.BeginShutdown(EndPlatformShutdown, _collabPlatform);
            }

            if (_serverCollabPlatform != null)
            {
                _serverCollabPlatform.BeginShutdown(EndPlatformShutdown, _serverCollabPlatform);
            }

            //Again, just for synchronous reasons.
            _platformShutdownCompletedEvent.WaitOne();
        }

        private void EndPlatformShutdown(IAsyncResult ar)
        {
            CollaborationPlatform collabPlatform = ar.AsyncState as CollaborationPlatform;

            try
            {
                //Shutdown actions will not throw.
                collabPlatform.EndShutdown(ar);
                Console.WriteLine("The platform is now shut down.");
            }
            finally
            {
                _platformShutdownCompletedEvent.Set();
            }
        }

        /// <summary>
        /// Read the local store for the certificate to use when creating the platform. This is necessary to establish a connection to the Server.
        /// </summary>
        /// <param name="friendlyName">The friendly name of the certificate to use.</param>
        /// <returns>The certificate instance.</returns>
        public static X509Certificate2 GetLocalCertificate(string friendlyName)
        {
            X509Store store = new X509Store(StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates;
            store.Close();

            foreach (X509Certificate2 certificate in certificates)
            {
                if (certificate.FriendlyName.Equals(friendlyName, StringComparison.OrdinalIgnoreCase))
                {
                    return certificate;
                }
            }
            return null;
        }

        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public static void WriteErrorLine(string line)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void WriteException(Exception ex)
        {
            WriteErrorLine(ex.ToString());
        }

        /// <summary>
        /// Prompts the user to press a key, unblocking any waiting calls to the
        /// <code>WaitForSampleFinish</code> method
        /// </summary>
        public static void FinishSample()
        {
            Console.WriteLine("Please hit any key to end the sample.");
            Console.ReadKey();
            _sampleFinished.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void WaitForSampleFinish()
        {
            _sampleFinished.WaitOne();
        }
    }
}
