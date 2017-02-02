using Microsoft.Rtc.Collaboration.AudioVideo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcmaBotDTMF.Helpers;

namespace UcmaBotDTMF.Models
{
    public class ActiveCallModel
    {
        public string SipUri { get; set; }

        public DateTime CallTime { get; set; }

        public string DisplayName { get; set; }

        public string PhoneUri { get; set; }

        public AudioVideoCall AVCall { get; set; }

        public NewAudioCallIVR IVR { get; set; }
    }
}
