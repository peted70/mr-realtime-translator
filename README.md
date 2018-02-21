# Mixed Reality Real-time Translator
Note that this repository includes the Mixed Reality Toolkit for Unity as a submodule. See instructions here https://github.com/mtirion/MRTKAsSubModule if you are not familiar with submodules or their use in a Unity project.

![alt tag](https://raw.github.com/peted70/mr-realtime-translator/master/img/headline.PNG)

A quick guide of the different Unity components and how they can be used together and reused. For a detailed description see http://peted.azurewebsites.net/mixed-reality-real-time-translator/  

## Unity Components
### Microphone Audio Getter
This script is the one that retrieves the audio data from the microphone. You can set the sample rate and chunk size here. Currently it takes the signed float audio data procided by Unity and converts it to signed 16bit PCM data as required by the Translator API. This could be extended to support more formats if required.
  
![alt tag](https://raw.github.com/peted70/mr-realtime-translator/master/img/micgetter.PNG)

The output can be routed to another component which implements the AudioConsumer abstract class. There are two of those in the project; one to send the data to the Translator API (Translator API Consumer) and another to save the data into a WAV file (for testing).

### Translator API Consumer
This component handles all communication with the Translator API including the Authentication, streaming the Audio data over the WebSocket connection and retrieving the textual and audio responses. To and From languages can be set here along with a selection of voices for the resulting audio file. Note - although there is a key there I have disabled it so you will need to sign up for your own key which you can do using the instructions here https://www.microsoft.com/en-us/translator/trial.aspx#get-started

![alt tag](https://raw.github.com/peted70/mr-realtime-translator/master/img/translatorAPI.PNG)

Again, components can be plugged in to respond to the responses from the Translator API. These are plugged in with the AudioReceivedHandler and TextReceivedHandler abstract classes. There are implementations for those in the sample and they look like this:

![alt tag](https://raw.github.com/peted70/mr-realtime-translator/master/img/unityreceivers.PNG)

For further details see http://peted.azurewebsites.net/mixed-reality-real-time-translator/

