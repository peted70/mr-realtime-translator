using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ApiProxy : IAudioConsumer
{
    const string speechurl = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=ru&features=texttospeech&voice=ru-RU-Irina&api-version=1.0";
    private HttpClient _http;
    private ClientWebSocket _ws;

    public event ReceiveHandler Received;

    public async Task InitialiseAsync()
    {
        string key = Environment.GetEnvironmentVariable("SKYPE_KEY");
        if (string.IsNullOrEmpty(key))
        {
            Debug.Log("Please set an environment variable named 'SKYPE_KEY' to your Skype api key");
            return;
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
        _ws = new ClientWebSocket();
        Debug.Log(token);
        _ws.Options.SetRequestHeader("Authorization", "Bearer " + token);

        try
        {
            await _ws.ConnectAsync(new Uri(speechurl), CancellationToken.None);
        }
        catch (WebSocketException ex)
        {
            Debug.Log(ex.Message + " error code: " + ex.WebSocketErrorCode);
            return;
        }
        //await _ws.ConnectAsync(new Uri("http://localhost:54545"), CancellationToken.None);
        Debug.Log("successfully connected");

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //Task.Run(ReceiveAsync);
        ReceiveAsync();

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // as soon as we are connected send the WAVE header..
        ArraySegment<byte> data = new ArraySegment<byte>(WavFile.GetWaveHeader(0));
        await _ws.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        Debug.Log("Sent WAVE header");
    }

    private void OnReceived(string val)
    {
        if (Received != null)
            Received(val);
    }

    private async Task ReceiveAsync()
    {
        Debug.Log("state -> " + Enum.GetName(typeof(WebSocketState), _ws.State));

        var buffer = new byte[4096 * 20];
        while (_ws.State == WebSocketState.Open)
        {
            Debug.Log("ReceiveAsync");
            var response = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            OnReceived(response.ToString());

            Debug.Log("detected msg -> " + Enum.GetName(typeof(WebSocketMessageType), response.MessageType));

            if (response.MessageType == WebSocketMessageType.Close)
            {
                Debug.Log("Received Sokcet CLOSE");
                await
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close response received",
                        CancellationToken.None);
            }
            else if (response.MessageType == WebSocketMessageType.Text)
            {
                var resultStr = Encoding.UTF8.GetString(buffer);
                var result = JsonUtility.FromJson<Result>(resultStr);

                Debug.Log("Result " + result.type);
            }
            else if (response.MessageType == WebSocketMessageType.Binary)
            {
                // This will be audio data if we requested it...
                Debug.Log("We got returned some audio");
            }
        }
        Debug.Log("OOPS - connection no longer open");
    }

    public bool IsValid()
    {
        return true;
    }

    public Task SaveAsync()
    {
        return Task.FromResult(0);
    }

    public void WriteData(ArraySegment<byte> data, int length)
    {
        throw new NotImplementedException();
    }

    public async Task WriteDataAsync(ArraySegment<byte> data, int length)
    {
        if (_ws != null)
        {
            await _ws.SendAsync(data, WebSocketMessageType.Binary, false, CancellationToken.None);
            Debug.Log("sent " + data.Count);
        }
    }

    public bool WriteSynchronous()
    {
        return false;
    }
}
