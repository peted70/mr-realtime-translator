using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_EDITOR && WINDOWS_UWP
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

public delegate void ReceiveHandler(string val);
public delegate void AudioDataReceivedHandler(AudioDataReceivedEventArgs data);

public class Languages
{
    public List<SpeechItem> languages { get; set; } = new List<SpeechItem>();
    public Dictionary<string, VoiceItem> Voices { get; set; } = new Dictionary<string, VoiceItem>();
}

public class SpeechItem
{
    public string name { get; set; }
    public string language { get; set; }
}

public class VoiceItem
{
    public string gender { get; set; }
    public string locale { get; set; }
    public string languageName { get; set; }
    public string displayName { get; set; }
    public string regionName { get; set; }
    public string language { get; set; }
}

public class ApiProxyUWP : IAudioConsumer
{
    private HttpClient _http;
    private MessageWebSocket _ws;
    private DataWriter _dataWriter;

    public event ReceiveHandler Received;
    public event AudioDataReceivedHandler AudioDataReceived;

    private AudioDataReceivedEventArgs _args = new AudioDataReceivedEventArgs();

    //const string speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=yue&features=texttospeech&voice=zh-HK-Danny&api-version=1.0";
    const string speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=es&features=texttospeech&voice=es-ES-Laura&api-version=1.0";

    public async Task InitialiseAsync()
    {
        var token = await GetTokenAsync();
        _ws = new MessageWebSocket();
        _ws.SetRequestHeader("Authorization", "Bearer " + token);
        _ws.MessageReceived += _ws_MessageReceived;

        // Note : do some of this in parallel..
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync("https://dev.microsofttranslator.com/languages?api-version=1.0&scope=text,tts,speech");
            // add header
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            dynamic obj = JsonConvert.DeserializeObject(jsonString);
            foreach (var speechItem in obj.speech)
            {

            }
        }

        await _ws.ConnectAsync(new Uri(speechurl));
        Debug.Log("successfully connected");

        // as soon as we are connected send the WAVE header..
        _dataWriter = new DataWriter(_ws.OutputStream);
        await WriteBytes(WavFile.GetWaveHeader(0));
        Debug.Log("Sent WAVE header");
    }

    private async Task WriteBytes(byte[] bytes)
    {
        _dataWriter.WriteBytes(bytes);
        await _dataWriter.StoreAsync();
        await _dataWriter.FlushAsync();
    }

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
#endif
