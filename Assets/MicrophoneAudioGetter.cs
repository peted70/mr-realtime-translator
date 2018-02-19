using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MicrophoneAudioGetter : MonoBehaviour
{
    AudioClip _clip;
    string _mic;

    public int SampleRate = 16000;

    float[] audioData;
    int _totalRead = 0;

    int _lastRead = 0;

    // 100ms of data
    public int ChunkSize = 1600;

    MemoryStream dataStream = new MemoryStream();

    public List<AudioConsumer> consumers = new List<AudioConsumer>();
    ArraySegment<byte> _buffer = new ArraySegment<byte>();

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

    public bool IsRecording()
    {
        return _clip != null;
    }

    public void StartRecording()
    {
        // From here we can start streaming data from the mic...
        if (Microphone.devices.Length > 0)
        {
            _mic = Microphone.devices[0];

            Debug.Log("Num Microphones " + Microphone.devices.Length);
            foreach (var mic in Microphone.devices)
            {
                Debug.Log(mic);
            }

            int minFreq = 0;
            int maxFreq = 0;

            Microphone.GetDeviceCaps(_mic, out minFreq, out maxFreq);
            Debug.Log("Microphone min freq = " + minFreq + " max freq = " + maxFreq);

            // if we have a mic start capturing and streaming audio...
            _clip = Microphone.Start(_mic, true, 1, SampleRate);
            int numChannels = _clip.channels;
            while (Microphone.GetPosition(_mic) < 0) { } // HACK from Riro
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.Log("No Microphone detected");
        }
    }

    public void StopRecording()
    {
        _clip = null;
    }

    IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Recording started");
        StartRecording();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
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

            foreach (var consumer in consumers)
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

            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.SetLength(0);
        }
    }
}
