using System;
using Microsoft.Rtc.Collaboration.AudioVideo;
using UcmaBotDtmf.Helpers;

namespace UcmaBotDtmf.Models
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
