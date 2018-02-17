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

[Serializable]
public class Languages
{
    public List<SpeechItem> languages = new List<SpeechItem>();
    public Dictionary<string, VoiceItem> Voices { get; set; } = new Dictionary<string, VoiceItem>();
}

[Serializable]
public class SpeechItem
{
    public string name;
    public string displayname { get; set; }
    public string language { get; set; }
}

[Serializable]
public class VoiceItem
{
    public string gender { get; set; }
    public string locale { get; set; }
    public string languageName { get; set; }
    public string displayName { get; set; }
    public string regionName { get; set; }
    public string language { get; set; }
}


public delegate void ReceiveHandler(string val);
public delegate void AudioDataReceivedHandler(AudioDataReceivedEventArgs data);

public class ApiProxyParams
{
    public string ApiKey;
    public string FromLanguage;
    public string ToLanguage;
    public string Voice;

    // Only need to check the API key - other params will just take default 
    // values.
    public void EnsureValid()
    {
        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentException("Invalid API Key", "ApiKey");
    }
};

public class ApiProxyUWP : IAudioConsumer
{
    private HttpClient _http;
#if !UNITY_EDITOR && WINDOWS_UWP
    private MessageWebSocket _ws;
    private DataWriter _dataWriter;
#endif
    public Languages _languages;

    public event ReceiveHandler Received;
    public event AudioDataReceivedHandler AudioDataReceived;

    //const string speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=es&features=texttospeech&voice=es-ES-Laura&api-version=1.0";
    string _speechurl;

    public ApiProxyUWP(ApiProxyParams parameters)
    {
        _speechurl = $"wss://dev.microsofttranslator.com/speech/translate?from={parameters.FromLanguage}&to={parameters.ToLanguage}&features=texttospeech&voice={parameters.Voice}&api-version=1.0";
    }

    public async Task InitialiseAsync()
    {
        // Retrieve the Auth token and also retrive the language support
        // data in parallel..
        //var getTokenTask = GetTokenAsync();
        //var getLanguageSupportTask = GetLanguageSupportAsync();
        //await Task.WhenAll(getTokenTask, getLanguageSupportTask);

        //var token = getTokenTask.Result;
        //_languages = getLanguageSupportTask.Result;

        var token = await GetTokenAsync();

#if !UNITY_EDITOR && WINDOWS_UWP
        await _ws.ConnectAsync(new Uri(_speechurl));
        Debug.Log("successfully connected");

        // as soon as we are connected send the WAVE header..
        _dataWriter = new DataWriter(_ws.OutputStream);
        await WriteBytes(WavFile.GetWaveHeader(0));
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

    private async void _ws_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
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
            Received?.Invoke(result.translation);
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
                AudioDataReceived?.Invoke(_args);
            }
        }
    }
#endif

    private async Task<string> GetTokenAsync()
    {
        string key = "8336ba69dc244d52bfe17d4257161410";// Environment.GetEnvironmentVariable("SKYPE_KEY");
        if (string.IsNullOrEmpty(key))
        {
            Debug.Log("Please set an environment variable named 'SKYPE_KEY' to your Skype api key");
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

    public bool IsValid()
    {
        return true;
    }

    public Task SaveAsync()
    {
        return Task.FromResult(0);
    }

    public void WriteData(ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public async Task WriteDataAsync(ArraySegment<byte> data)
    {
        await WriteBytes(data.Array);
    }

    public bool WriteSynchronous()
    {
        return false;
    }
}
