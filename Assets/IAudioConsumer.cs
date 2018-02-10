using System;
using System.Threading.Tasks;

public delegate void ReceiveHandler(string val);

public interface IAudioConsumer
{
    event ReceiveHandler Received;

    Task InitialiseAsync();
    void WriteData(ArraySegment<byte> data, int length);
    Task WriteDataAsync(ArraySegment<byte> data, int length);
    bool IsValid();
    Task SaveAsync();
    bool WriteSynchronous();
}