using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_EDITOR && WINDOWS_UWP
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

public class ApiProxyUWP : IAudioConsumer
{
    private HttpClient _http;
    private MessageWebSocket _ws;
    private DataWriter _dataWriter;

    public event ReceiveHandler Received;

    const string speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=yue&features=texttospeech&voice=zh-HK-Danny&api-version=1.0";

    public async Task InitialiseAsync()
    {
        var token = await GetTokenAsync();
        _ws = new MessageWebSocket();
        _ws.SetRequestHeader("Authorization", "Bearer " + token);
        _ws.MessageReceived += _ws_MessageReceived;
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

    private void _ws_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
    {
        if (args.MessageType == SocketMessageType.Utf8)
        {
            // parse the text result that contains the recognition and translation
            // {"type":"final","id":"0","recognition":"Hello, can you hear me now?","translation":"Hallo, kannst du mich jetzt hören?"}
            string jsonOutput;
            using (var dataReader = args.GetDataReader())
            {
                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                jsonOutput = dataReader.ReadString(dataReader.UnconsumedBufferLength);
            }

            var result = JsonConvert.DeserializeObject<Result>(jsonOutput);
        }
        else if (args.MessageType == SocketMessageType.Binary)
        {
            // the binary output is the text to speech audio
            using (var dataReader = args.GetDataReader())
            {
                dataReader.ByteOrder = ByteOrder.LittleEndian;
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
