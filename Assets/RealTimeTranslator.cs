using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RealTimeTranslator : MonoBehaviour
{
    AudioClip _clip;
    string _mic;
    ClientWebSocket _ws;

    string key = Environment.GetEnvironmentVariable("SKYPE_KEY");
    string url = "wss://dev.microsofttranslator.com/speech/translate?from=en-US&to=ru&features=texttospeech&voice=ru-RU-Irina&api-version=1.0";

    const int SampleRate = 16000;
    WavFile _wav;

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

    // Use this for initialization
    async void Start()
    {
        var cfg = AudioSettings.GetConfiguration();
        cfg.dspBufferSize = 0;

        ServicePointManager.ServerCertificateValidationCallback = cb;
        _ws = new ClientWebSocket();
        _ws.Options.SetRequestHeader("Ocp-Apim-Subscription-Key", key);
        await _ws.ConnectAsync(new Uri(url), CancellationToken.None);
        //await _ws.ConnectAsync(new Uri("http://localhost:54545"), CancellationToken.None);
        Debug.Log("successfully connected");

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //Task.Run(ReceiveAsync);
        ReceiveAsync();

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        _wav = new WavFile();

        // as soon as we are connected send the WAVE header..
        ArraySegment<byte> data = new ArraySegment<byte>(GetWaveHeader(0));
        await _ws.SendAsync(data, WebSocketMessageType.Text, false, CancellationToken.None);
        Debug.Log("Sent WAVE header");

        // From here we can start streaming data from the mic...
        if (Microphone.devices.Length > 0)
        {
            _mic = Microphone.devices[0];

            Debug.Log("Num Microphones " + Microphone.devices.Length);
            foreach (var mic in Microphone.devices)
            {
                Debug.Log(mic);
            }

            // if we have a mic start capturing and streaming audio...
            _clip = Microphone.Start(_mic, true, 1, SampleRate);
            while (Microphone.GetPosition(null) < 0) { } // HACK from Riro
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.Log("No Microphone detected");
        }
    }

    int BufferConvertedData(float[] audioData, Stream stream)
    {
        // Can't just do a block copy here as we need to convert from float[-1.0f, 1.0f] to 16bit PCM
        int x = sizeof(UInt16);
        UInt16 maxValue = UInt16.MaxValue;

        int i = 0;
        while (i < audioData.Length)
        {
            stream.Write(BitConverter.GetBytes(Convert.ToInt16(audioData[i] * maxValue)), 0, x);
            ++i;
        }

        return audioData.Length;
    }

    int _lastRead = 0;

    // 100ms of data
    int ChunkSize = (int)((float)SampleRate / 100.0f);

    // stream 100ms of audio at a time
    private void Update()
    {
        if (_clip == null)
            return;
        //if (_wav == null)
        //    return;

        int currentPos = Microphone.GetPosition(_mic);
        if (currentPos < _lastRead)
        {
            _lastRead = 0;
        }
        if ((currentPos - _lastRead) == 0)
            return;

        // Every time we are called we need to buffer the audio data - otherwise
        // when the data loops it will be lost..
        // So copy and convert data here into a MemoryStream and then when  we hit a 
        // chunk boundary we can empty it and either send to the network or copy to 
        // disk..
        // remove this allocation by pre-allocating a buffer that is always big 
        // enough..
        audioData = new float[currentPos - _lastRead];

        //Debug.Log("audiodata length = " + audioData.Length + " last read = " + _lastRead);
        if (_clip.GetData(audioData, _lastRead))
        {
            _totalRead += BufferConvertedData(audioData, dataStream);
            _lastRead = currentPos;
        }

        // When we have collected enough data for a chunk, send it
        // and reset the collection..
        if (dataStream.Position >= ChunkSize)
        {
            dataStream.Flush();
            ArraySegment<byte> buffer = new ArraySegment<byte>();

            if (!dataStream.TryGetBuffer(out buffer))
            {
                Debug.Log("Couldn't read buffer");
            }
            if (_wav != null)
                _wav.WriteData(buffer.Array, buffer.Count);
            if (_ws != null)
                _ws.SendAsync(buffer, WebSocketMessageType.Binary, false, CancellationToken.None);

            dataStream = new MemoryStream();

            if (_wav != null && _totalRead > SampleRate * 10)
            {
                Debug.Log("Closing file");
                using (var debugFs = new FileStream("out.wav", FileMode.OpenOrCreate))
                {
                    _wav.Save(debugFs);
                    _wav = null;
                }
            }
        }
    }

    private async Task ReceiveAsync()
    {
        Debug.Log("state -> " + Enum.GetName(typeof(WebSocketState), _ws.State));

        var buffer = new byte[4096 * 20];
        while (_ws.State == WebSocketState.Open)
        {
            Debug.Log("ReceiveAsync");
            var response = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

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

    private bool cb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    const int NumSamplesInChunk = SampleRate / 10;
    float[] audioData = new float[NumSamplesInChunk];
    int _totalRead = 0;
    MemoryStream dataStream = new MemoryStream();

    /// <summary>
    /// Create a RIFF Wave Header for PCM 16bit 16kHz Mono
    /// </summary>
    /// <returns></returns>
    public static byte[] GetWaveHeader(uint dataSize = 0, uint totalFileSize = 0)
    {
        var channels = (short)1;
        var sampleRate = SampleRate;
        var bitsPerSample = (short)16;
        var extraSize = 0;
        var blockAlign = (short)(channels * (bitsPerSample / 8));
        var averageBytesPerSecond = sampleRate * blockAlign;

        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
            writer.Write(Encoding.UTF8.GetBytes("RIFF"));
            // Total file size - zero is fine for streaming format..
            writer.Write(dataSize);
            writer.Write(Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(Encoding.UTF8.GetBytes("fmt "));
            writer.Write((int)(18 + extraSize)); // wave format length 
            writer.Write((short)1);// PCM
            writer.Write((short)channels);
            writer.Write((int)sampleRate);
            writer.Write((int)averageBytesPerSecond);
            writer.Write((short)blockAlign);
            writer.Write((short)bitsPerSample);
            writer.Write((short)extraSize);

            writer.Write(Encoding.UTF8.GetBytes("data"));
            // dataSize - zero is fine for streaming format..
            writer.Write(dataSize);

            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    public class WavFile
    {
        private MemoryStream _ms;
        private BinaryWriter _bw;
        private bool _disposed;

        public WavFile()
        {
            _ms = new MemoryStream();
            _bw = new BinaryWriter(_ms);
        }

        public void WriteData(byte[] data, int length)
        {
            if (_disposed == false)
                _bw.Write(data, 0, length);
        }

        public void Save(Stream outStr)
        {
            uint headerSize = 44;
            uint dataSize = (uint)_ms.Length;

            var headerBytes = GetWaveHeader(dataSize, headerSize + dataSize);
            outStr.Write(headerBytes, 0, headerBytes.Length);

            _ms.Position = 0;
            using (var br = new BinaryReader(_ms))
            {
                outStr.Write(br.ReadBytes((int)_ms.Length), 0, (int)_ms.Length);
            }
            _bw.Dispose();
            _disposed = true;
        }
    }

    public class Result
    {
        public string type;
        public string id;
        public string recognition;
        public string translation;
        public int audioStreamPosition;
        public int audioSizeBytes;
        public long audioTimeOffset;
        public int audioTimeSize;
    }
}
