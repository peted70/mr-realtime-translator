using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WavFile : AudioConsumer
{
    private MemoryStream _ms;
    private BinaryWriter _bw;
    private bool _disposed;
    private int _samplesUntilSave;

    public int CaptureLength = 10;
    public int SampleRate = 16000;

    void Start()
    {
        _samplesUntilSave = CaptureLength * SampleRate;

        _ms = new MemoryStream();
        _bw = new BinaryWriter(_ms); 
    }

    public async override Task WriteDataAsync(MemoryStream stream, int count)
    {
        await WriteDataAsync(stream.ToArray(), count);
    }

    public override void WriteData(MemoryStream stream, int count)
    {
        WriteData(stream.ToArray(), count);
    }

    public override void WriteData(byte[] data, int count)
    {
        if (_disposed == false)
            WriteDataInternal(data, count);
    }

    public override void WriteData(ArraySegment<byte> data, int count)
    {
        if (_disposed == false)
            WriteDataInternal(data.Array, count);
    }

    public override Task WriteDataAsync(byte[] data, int count)
    {
        if (_disposed == false)
            return Task.Run(() => WriteDataInternal(data, count));
        return null;
    }

    public override Task WriteDataAsync(ArraySegment<byte> data, int count)
    {
        if (_disposed == false)
            return Task.Run(() => WriteDataInternal(data.Array, count));
        return null;
    }

    int _totalWritten = 0;
    private readonly int _sampleRate;

    private void WriteDataInternal(byte[] data, int count)
    {
        _bw.Write(data, 0, count);
        _totalWritten += data.Length;
        Debug.Log("total written = " + _bw.BaseStream.Position);
        if (_totalWritten > _samplesUntilSave)
        {
            SaveAsync();
        }
    }

    public override bool IsValid()
    {
        return !_disposed;
    }

    bool _saving = false;
    public override async Task SaveAsync()
    {
        if (_saving == true)
            return;

        _saving = true;
        Debug.Log("Closing file");
        try
        {
            string file = Application.persistentDataPath + Path.DirectorySeparatorChar + "out.wav";

            using (var debugFs = new FileStream(file, FileMode.OpenOrCreate))
            {
                uint headerSize = 44;
                uint dataSize = (uint)_ms.Length;

                var headerBytes = GetWaveHeader(dataSize, headerSize + dataSize);
                await debugFs.WriteAsync(headerBytes, 0, headerBytes.Length);

                _ms.Position = 0;
                using (var br = new BinaryReader(_ms))
                {
                    await debugFs.WriteAsync(br.ReadBytes((int)_ms.Length), 0, (int)_ms.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return;
        }
        finally
        {
            _bw.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Create a RIFF Wave Header for PCM 16bit 16kHz Mono
    /// </summary>
    /// <returns></returns>
    public static byte[] GetWaveHeader(uint dataSize = 0, uint totalFileSize = 0, int sr = 16000)
    {
        var channels = (short)1;
        var sampleRate = sr;
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

    public override bool WriteSynchronous()
    {
        return true;
    }
}

