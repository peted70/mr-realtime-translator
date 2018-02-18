## mixed reality real-time translator ##

Did you know that there is an API that you can call which will accept as input an audio stream, say from a microphone, that will translate any spoken sentences into another language and return not only a final audio output file of the result but also partial textual information as the context of the meaning of the sentence being uttered changes? So whilst we speak our words can be understood by others who wouldn't usually understand us and all happening in real-time as we utter those sentences. The API in question is in the suite of Microsoft cogntive services hidden away amongst face detection and recognition and natural language understanding services. All of which add up to a thought-provoking mix of Artificial Intelligence functionality that can be used to power Mixed Reality scenarios. For true Mixed Reality not only is digital content blended seamlessly with the real world in a visual sense but the flow of information must travel back in the other direction with an evolving understanding of the physical environment.  

The cogntive services are mainly REST APIs and thus can be used from pretty much any computing device with a http stack, although the real-time speech also uses WebSockets to stream voice data over the internet. Since most of the Mixed Reality development I see is powered by Unity, particularly the HoloLens development, I am going to focus on that for this post. So the first hurdle is how do we make REST API calls from Unity?

REST in Unity

There are two main Unity classes for making http calls; WWW https://docs.unity3d.com/ScriptReference/WWW.html and UnityWebRequest https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html. WWW is slightly higher level than UnityWebRequest and actually uses it in it's implementation so you can think of it as a wrapper. As the scripting environment in Unity is .NET we could also use HttpWebRequest, WebClient or HttpClient. With the Experimental .NET 4.6 support in Unity 

Under Edit > Project Settings > Player
> Other Settings
"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\projplayersettings.PNG"
"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\expnet.PNG"

(see http://peted.azurewebsites.net/holograms-catalogue/ for more details) we can use the newer HttpClient with it's Task-based, asynchronous design meaning we can use async/await features which the experimental support unlocks for us. Since I write these posts for instructional purposes the interface is much cleaner and easier to understand but you could replace these calls using any of the other http clients previously mentioned. Here's an example of making an Http GET request with HttpClient:

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync("https://dev.microsofttranslator.com/languages?api-version=1.0&scope=text,tts,speech");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            dynamic obj = JsonConvert.DeserializeObject(jsonString);

            // Do some stuff with obj...
        } 

If you find that HttpClient is somehow not available and you get the error

error CS0234: The type or namespace name `Http' does not exist in the namespace `System.Net'. Are you missing `System.Net.Http' assembly reference?
"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\httpmissing.PNG"

in the Unity console
Then if you add a file called mcs.rsp with the contents: -r:System.Net.Http.dll in your Assets folder...

"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\mcscsp.PNG"

and then reload your project

## Unity Accessing the Microphone Data

If we are going to stream audio data to an API then we need a mechanism to retrieve the raw audio bits from the correct microphone device. Let's look at the Unity API way to do this:

"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\unitymic.PNG"

This shows the Microphone class which is a collection of static functions that we can use. So to select a particular device we can inspect the description strings in Microphone.devices and then the other static functions refer to that device by its string name or null for the default device. I'm just going to use the first mic in the list using 

_mic = Microphone.devices[0];

but we could, of course expose a choice to the user if there are multiple device.

To start recording from the device we need to associate an AudioClip object with the microphone with the required parameters, including sample rate, buffer length, etc. Once recording we can use AudioClip.GetData inside our update loop to retrieve buffered audio data generated from the microphone. When sending this data to an API we have two main considerations; 

- What is the format required by the API and will we need to convert (the data returned by Unity is a float array ranging from -1 to 1).
- How often do we want to send the data and how will we compose it into chunks for sending.

The Skype realtime API requires mono, signed 16bit PCM sampled at 16khz. Well, the data provided by Unity is already mono and we can request the sample rate when we initially called Microphone.Start so that just leaves us with a conversion from 

float [-1.0, 1.0] -> short [-32768, 32767]

Here's the conversion function which populates a provided Stream:

<script src="https://gist.github.com/peted70/aeb9f26e8b52da357369139f5dbf9100.js"></script>

Once the provided stream contains enough data to be considered a 'chunk' we can send it to the API. 

Note. In the sample code I have abstracted where the audio data gets sent so I can provide a list of audio consumers. I used this whilst developing the sample to test by checking that I could reconstruct a valid WAV file from the supplied data. That code is still in the sample for testing purposes.

## Translator API
You can get an overview of the Translator APIs here https://docs.microsofttranslator.com/. This is a summary of what's on offer:

"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\unitymic.PNG"

We are going to focus on the speech part of things the details are here https://docs.microsofttranslator.com/speech-translate.html in the official documentation.

I'm just going to pick out the significant parts based on building the sample that goes with this post for more details and setting up the Azure backend to be able to use the service please refer to the documentation referenced above.

1) Azure Subscription
2) Activate a Translator Speech API on the Azure portal
3) Copy the Keys from that service
4) Set up a WebSocket communication with the API with details of 'from' and 'to' languages
5) Stream audio data over the WebSocket
6) Retrieve results from the WebSocket
    i) Text Results including partial and final results
    ii) Final audio file with translation

### Authentication
Let's have a look at some of the main code used to drive the sample starting with Auth:

<script src="https://gist.github.com/peted70/302a667437f782e6bbef9471ad0efd31.js"></script>

You can acquire an access token either using your API key directly in the request header or you can make a seperate request to https://api.cognitive.microsoft.com/sts/v1.0/issueToken to retrieve a time-limited access token. Either way in a produciton app you should avoid keeping your API key in your app and protect it by using it via a server.

### WebSockets
Initially I tried to set the WebSocket connection up using the System.Net.WebSockets class but although I could create a connection to the Translator API I never retrieved any results back. As a result I implemented the connection using the UWP WebSockets implementation and as a result the sample won't work in the Unity editor. I hope to revisit this and fix but for now I just want the sample to run on a HoloLens so didn't worry about it too much.

#### Streaming Audio


#### Retrieving Translated Results 

# Architecture of the Sample Code
In order to support some reusability of components like the microphone audio data access the code has been factored accordingly. There is a producer/consumer-like model whereby the MicrophoneAudioGetter component is a Monobehaviour into which you can plug consumers. In the sample, there is only one consumer and that is another component which proxies the audio data to the Translator API. Any number of consumers could be plugged in by creating a C# script and implementing the abstract class AudioConsumer and then adding the script component to a GameObject and referencing that in the consumers list of the Microphone Audio Getter.

"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\microphone-audio-getter.PNG"

The Translator API consumer is the one used in the sample and that provides properties via the Editor to configure the API; Api Key, From Language, To Language and Voice:

"C:\Users\Pete D\Pictures\Blog\RealTimeSkype\APIConsumerEditor.PNG"

There are ten languages to choose from and various different male/female voices to choose from depending on the 'to' language selected.

Now, the Translator API sends out a couple of events; one when textual tranlation is received and one for audio. So again we have configured two lists of receivers, one for each event. So in order to receive wither event it is necessary to add a C# script which implements either AudioReceivedHandler or TextReceivedHandler and add a reference to the attached GameObject to the appropriate property in the Translator API behaviour.

<Component and Sample project description>
<reference to the UWP sample>
<video of usage>
<screenshots>
<link to instructions of how to submodule reference MRTK>

The sample project can be found here https://github.com/peted70/mr-realtime-translator. In summary, this is just how to wire up the APIs and hopefully something reusable. I can imagine a whole host of ways in which this could help improve Mixed Reality apps for example, by translating input to APIs that have been written expecting one specific language and also using contextual information such as gaze and proximity to translate audio voice in real-time in a collaborative Mixed Reality scenario.  