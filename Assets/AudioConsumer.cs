using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AudioConsumer : MonoBehaviour
{
    public abstract void WriteData(ArraySegment<byte> data, int count);
    public abstract void WriteData(byte[] data, int count);
    public abstract void WriteData(MemoryStream stream, int count);
    public abstract Task WriteDataAsync(byte[] data, int count);
    public abstract Task WriteDataAsync(ArraySegment<byte> data, int count);
    public abstract Task WriteDataAsync(MemoryStream stream, int count);
    public abstract bool IsValid();
    public abstract Task SaveAsync();
    public abstract bool WriteSynchronous();
}

