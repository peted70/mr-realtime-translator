using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AudioConsumer : MonoBehaviour
{
    public abstract void WriteData(ArraySegment<byte> data);
    public abstract Task WriteDataAsync(ArraySegment<byte> data);
    public abstract bool IsValid();
    public abstract Task SaveAsync();
    public abstract bool WriteSynchronous();
}

