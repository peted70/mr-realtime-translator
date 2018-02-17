using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
#if UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RealTimeTranslator : MonoBehaviour
{
    AudioClip _clip;
    string _mic;
    public string ApiKey = "--- YOUR TRANSLATOR API KEY GOES HERE ---";
    public TextMesh text;

    public SpeechItem FromLanguage;
    public SpeechItem ToLanguage;
    public VoiceItem Voice;

    [HideInInspector]
    public ApiProxyUWP apiProxy;

    const int SampleRate = 16000;
    const int NumSamplesInChunk = SampleRate / 10;

    float[] audioData = new float[NumSamplesInChunk];
    int _totalRead = 0;
    MemoryStream dataStream = new MemoryStream();

    private List<IAudioConsumer> _consumers = new List<IAudioConsumer>();
    ArraySegment<byte> _buffer = new ArraySegment<byte>();

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

#if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = cb;
#endif
        //IAudioConsumer wavFile = new WavFile(SampleRate);
        //await wavFile.InitialiseAsync();
        //_consumers.Add(wavFile);
#if TESTING_AUDIO
        IAudioConsumer apiProxy = new ApiProxy();
        apiProxy.Received += ApiProxy_Received;
        await apiProxy.InitialiseAsync();
        _consumers.Add(apiProxy);
#endif
        apiProxy = new ApiProxyUWP(new ApiProxyParams
        {
            ApiKey = ApiKey,
            FromLanguage = FromLanguage.name,
            ToLanguage = ToLanguage.language,
            Voice = Voice.languageName,
        });

        apiProxy.AudioDataReceived += ApiProxy_AudioDataReceived;
        apiProxy.Received += ApiProxy_Received;
        await apiProxy.InitialiseAsync();
        _consumers.Add(apiProxy);

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
            while (Microphone.GetPosition(_mic) < 0) { } // HACK from Riro
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.Log("No Microphone detected");
        }
    }

    private void ApiProxy_AudioDataReceived(AudioDataReceivedEventArgs data)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            var audioSrc = GetComponent<AudioSource>();
            var clip = AudioClip.Create("TranslationResponse", data.Data.Length, 1, 16000, false);
            clip.SetData(data.Data, 0);
            audioSrc.playOnAwake = true;
            audioSrc.clip = clip;
            audioSrc.volume = 1;
            audioSrc.enabled = true;
            audioSrc.minDistance = 0;
            audioSrc.maxDistance = 1000;

            audioSrc.rolloffMode = AudioRolloffMode.Linear;
            audioSrc.Stop();
            audioSrc.PlayOneShot(clip, 100.0f);
            Debug.Log("Playing audio");
        }, 
        false);
    }

    private void ApiProxy_Received(string val)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() => { text.text = val; }, false);
    }

    /// <summary>
    /// Convert floating point audio data in the range -1.0 to 1.0 to signed 16-bit audio data
    /// </summary>
    /// <param name="audioData"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    int BufferConvertedData(float[] audioData, Stream stream)
    {
        // Can't just do a block copy here as we need to convert from float[-1.0f, 1.0f] to 16bit PCM
        int i = 0;
        while (i < audioData.Length)
        {
            stream.Write(BitConverter.GetBytes(Convert.ToInt16(audioData[i] * Int16.MaxValue)), 0, sizeof(Int16));
            ++i;
        }

        return audioData.Length;
    }

    int _lastRead = 0;

    // 100ms of data
    int ChunkSize = (int)((float)SampleRate / 10.0f);

    // stream 100ms of audio at a time
    private void Update()
    {
        if (_clip == null)
            return;

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
        // The sample count is determined by the length of the float array so we can't 
        // optimise this allocation
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

            if (!dataStream.TryGetBuffer(out _buffer))
            {
                Debug.Log("Couldn't read buffer");
                return;
            }

            foreach (var consumer in _consumers)
            {
                if (consumer.IsValid() == false)
                    continue;

                if (consumer.WriteSynchronous())
                {
                    consumer.WriteData(_buffer);
                }
                else
                {
                    consumer.WriteDataAsync(_buffer);
                }
            }

            //dataStream = new MemoryStream();
            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.SetLength(0);
        }
    }

#if UNITY_EDITOR
    private bool cb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
#endif
}

