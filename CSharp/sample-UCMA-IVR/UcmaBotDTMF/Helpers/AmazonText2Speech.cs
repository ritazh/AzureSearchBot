﻿using System;
using System.Configuration;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;

namespace UcmaBotDtmf.Helpers
{
    public class AmazonText2Speech
    {
        private AmazonPollyClient amazonPolicyClient;
        public AmazonText2Speech()
        {
            
            var credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["accesskey"], ConfigurationManager.AppSettings["secretkey"]);
            amazonPolicyClient = new AmazonPollyClient(credentials);
        }
        public SynthesizeSpeechResponse Speak(string text)
        {
            var speak =  amazonPolicyClient.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = text,
                VoiceId = "Justin",
            }).Result;

            return speak;
           /* using (var fileStream = File.Create(targetMp3FilePath))
            {
                speak.AudioStream.CopyTo(fileStream);
            }*/
        }
    }
}
