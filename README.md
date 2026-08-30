# Mixed Reality Real-time Translator

Note that this repository includes the Mixed Reality Toolkit for Unity as a submodule. See instructions here: https://github.com/mtirion/MRTKAsSubModule if you are not familiar with submodules or their use in a Unity project.

![Project headline](https://raw.github.com/peted70/mr-realtime-translator/master/img/headline.PNG)

A quick guide to the Unity components and how they can be used together and reused. For a detailed description see http://peted.azurewebsites.net/mixed-reality-real-time-translator/.

## Unity Components

### Microphone Audio Getter

This script retrieves audio data from the microphone. You can set the sample rate and chunk size here. It converts Unity's signed float audio data to signed 16-bit PCM data required by the Translator API. This could be extended to support more formats if required.

![Microphone getter](https://raw.github.com/peted70/mr-realtime-translator/master/img/micgetter.PNG)

The output can be routed to another component which implements the AudioConsumer abstract class. There are two in the project: one to send data to the Translator API (Translator API Consumer) and another to save the data to a WAV file (for testing).

### Translator API Consumer

This component handles all communication with the Translator API including authentication, streaming audio data over the WebSocket connection, and retrieving textual and audio responses. Languages and voice selection can be configured; note the included sample key is disabled — sign up for your own key using the instructions here: https://www.microsoft.com/en-us/translator/trial.aspx#get-started

![Translator API consumer screenshot](https://raw.github.com/peted70/mr-realtime-translator/master/img/translatorAPI.PNG)

For further details see http://peted.azurewebsites.net/mixed-reality-real-time-translator/

---

## License

This project is available under the MIT License. See [LICENSE](LICENSE) for details.
