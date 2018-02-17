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
    void Start()
    {
        var cfg = AudioSettings.GetConfiguration();
        cfg.dspBufferSize = 0;

#if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = cb;
#endif
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
        //UnityEngine.WSA.Application.InvokeOnAppThread(() => { text.text = val; }, false);
    }

    // stream 100ms of audio at a time
    private void Update()
    {
    }

#if UNITY_EDITOR
    private bool cb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
#endif
}

