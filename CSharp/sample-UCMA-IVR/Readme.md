# UCMA Bridge sample
This sample windows app uses the UCMA SDK to listen to audio from the a phone line, converts speech to text and text to speech using the bing Speech API, can forwards the messages to a bot using the Direct Line channel.

## Prerequisites
The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* Download and install the UCMA 5.0 SDK from [here](https://www.microsoft.com/en-us/download/details.aspx?id=47345)
* For testing from you machine, download a softphone from [XLite](http://www.counterpath.com/x-lite-download/)
* 


## X-Lite softphone client configuration
1. Set the account values  
![](Solution%20Items\Docs\Images\XLiteSettingsAccount.PNG)
2. Set the transport to TCP  
![](Solution%20Items\Docs\Images\XLiteSettingsTransport.PNG)
3. Check the SIP Outbound checkbos in the Advanced tab  
![](Solution%20Items\Docs\Images\XLiteSettingsAdvanced.PNG)