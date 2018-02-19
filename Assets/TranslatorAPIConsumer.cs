using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_EDITOR && WINDOWS_UWP
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public delegate void ReceiveHandler(string val);
public delegate void AudioDataReceivedHandler(AudioDataReceivedEventArgs data);

// to get supported languages make a get request to
// Accept: application/json
// https://dev.microsofttranslator.com/languages?api-version=1.0&scope=speech

// open a connection to...
// wss://dev.microsofttranslator.com/speech/translate

// Example:
// GET wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=it-IT&features=texttospeech&voice=it-IT-Elsa&api-version=1.0
// Ocp-Apim-Subscription-Key: {subscription key}
// X-ClientTraceId: {GUID}

// Once the connection is established, the client begins streaming audio to the service.The client sends audio in chunks.
// Each chunk is transmitted using a Websocket message of type Binary.
// Audio input is in the Waveform Audio File Format(WAVE, or more commonly known as WAV due to its filename extension). 
// The client application should stream single channel, signed 16bit PCM audio sampled at 16 kHz.
// The first set of bytes streamed by the client will include the WAV header.
// A 44-byte header for a single channel signed 16 bit PCM stream sampled at 16 kHz is:
public class TranslatorAPIConsumer : AudioConsumer
{
    // Public fields accessible in the Unity Editor
    public string ApiKey = "--- YOUR TRANSLATOR API KEY GOES HERE ---";

    public List<TextReceivedHandler> TextReceivers = new List<TextReceivedHandler>();
    public List<AudioReceivedHandler> AudioReceivers = new List<AudioReceivedHandler>();

    public SpeechItem FromLanguage;
    public SpeechItem ToLanguage;
    public VoiceItem Voice;

    private HttpClient _http;
#if !UNITY_EDITOR && WINDOWS_UWP
    private MessageWebSocket _ws = new MessageWebSocket();
    private DataWriter _dataWriter;
#endif
    private Languages _languages;
    
    public event ReceiveHandler Received;
    public event AudioDataReceivedHandler AudioDataReceived;

    private string _apiKey;
    string _speechurl;
    private bool _headerWritten;

    async void Start()
    {
        string from = FromLanguage.name;
        string to = ToLanguage.language;
        string voice = $"{Voice.locale}-{Voice.displayName}";

        _apiKey = ApiKey;
        _speechurl = $"wss://dev.microsofttranslator.com/speech/translate?from={from}&to={to}&features=texttospeech&voice={voice}&api-version=1.0";
        //_speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=es&features=texttospeech&voice=es-ES-Laura&api-version=1.0";
        // Retrieve the Auth token and also retrive the language support
        // data in parallel..
        //var getTokenTask = GetTokenAsync();
        //var getLanguageSupportTask = GetLanguageSupportAsync();
        //await Task.WhenAll(getTokenTask, getLanguageSupportTask);

        //var token = getTokenTask.Result;
        //_languages = getLanguageSupportTask.Result;

        var token = await GetTokenAsync(_apiKey);
        Debug.Log("retrieved token is " + token);

#if !UNITY_EDITOR && WINDOWS_UWP
        _ws = new MessageWebSocket();
        _ws.SetRequestHeader("Authorization", "Bearer " + token);
        _ws.MessageReceived += WebSocketMessageReceived;

        await _ws.ConnectAsync(new Uri(_speechurl));
        Debug.Log("successfully connected");

        // as soon as we are connected send the WAVE header..
        _dataWriter = new DataWriter(_ws.OutputStream);
        _dataWriter.ByteOrder = ByteOrder.LittleEndian;
        await WriteBytes(WavFile.GetWaveHeader(0));
        _headerWritten = true;
        Debug.Log("Sent WAVE header");
#endif
    }

    private async Task WriteBytes(byte[] bytes)
    {
#if !UNITY_EDITOR && WINDOWS_UWP
        _dataWriter.WriteBytes(bytes);
        await _dataWriter.StoreAsync();
        await _dataWriter.FlushAsync();
#endif
    }

    public async Task<Languages> GetLanguageSupportAsync()
    { 
        Languages ret = null;
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync("https://dev.microsofttranslator.com/languages?api-version=1.0&scope=text,tts,speech");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            ret = new Languages();
            foreach (var speechItem in obj.speech)
            {
                ret.languages.Add(new SpeechItem
                {
                    language = speechItem.Value.language,
                    displayname = speechItem.Value.name,
                    name = speechItem.Name,
                });
            }
            foreach (var voiceItem in obj.tts)
            {
                ret.Voices.Add(voiceItem.Name, new VoiceItem
                {
                    displayName = voiceItem.Value.displayName,
                    gender = voiceItem.Value.gender,
                    language = voiceItem.Value.language,
                    languageName = voiceItem.Value.languageName,
                    locale = voiceItem.Value.locale,
                    regionName = voiceItem.Value.regionName,
                });
            }
        }
        return ret;
    }

#if !UNITY_EDITOR && WINDOWS_UWP
    
    private AudioDataReceivedEventArgs _args = new AudioDataReceivedEventArgs();

    private void WebSocketMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
    {
        if (args.MessageType == SocketMessageType.Utf8)
        {
            string jsonOutput;
            using (var dataReader = args.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                jsonOutput = dataReader.ReadString(dataReader.UnconsumedBufferLength);
            }

            var result = JsonConvert.DeserializeObject<Result>(jsonOutput);
            foreach (var receiver in TextReceivers)
            {
                receiver.OnTextReceived(result);
            }
        }
        else if (args.MessageType == SocketMessageType.Binary)
        {
            // the binary output is the text to speech audio
            using (var dataReader = args.GetDataReader())
            {
                dataReader.ByteOrder = ByteOrder.LittleEndian;

                // Read the data from the audio returned - convert to [-1.0f, 1.0f]
                // then we can use an AudioClip in Unity and set the data into it and
                // play it back...
                var numBytes = dataReader.UnconsumedBufferLength;

                // Skip over the RIFF header for PCM 16bit 16kHz mono output
                var headerSize = 44;
                var bytes = new byte[headerSize];
                dataReader.ReadBytes(bytes);

                // skip the header
                var numSamples = (int)(numBytes - headerSize);

                float[] data = new float[numSamples];
                //var count = await dataReader.LoadAsync(numSamples);

                int numInt16s = numSamples / sizeof(Int16);
                for (int i = 0; i < numInt16s; i++)
                {
                    data[i] = dataReader.ReadInt16() / (float)Int16.MaxValue;
                }

                // Notify observers with this audio data.. (probably should notify the header
                // to future-proof but skip that for now - API data is signed 16bit PCM mono audio)
                _args.Data = data;
                foreach (var receiver in AudioReceivers)
                {
                    receiver.OnAudioReceived(_args);
                }
            }
        }
    }
#endif

    private async Task<string> GetTokenAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.Log("Please set an Api Key in the Unity Editor or in code");
            return string.Empty;
        }

        // First retrieve a time-restricted key to use to access the API (will need to write code
        // to refresh this later).
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
        Debug.Log(key);
        var url = string.Format("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        Debug.Log(url);
        var resp = await _http.PostAsync(url, null);
        resp.EnsureSuccessStatusCode();
        var token = await resp.Content.ReadAsStringAsync();
        return token;
    }

    public override bool IsValid()
    {
        return true;
    }

    public override Task SaveAsync()
    {
        return Task.FromResult(0);
    }

    public override void WriteData(ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public override async Task WriteDataAsync(ArraySegment<byte> data)
    {
        if (!_headerWritten)
            return;
        await WriteBytes(data.Array);
    }

    public override bool WriteSynchronous()
    {
        return false;
    }
}
