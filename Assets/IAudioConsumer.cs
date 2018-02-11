using System;
using System.Threading.Tasks;

public interface IAudioConsumer
{
    Task InitialiseAsync();
    void WriteData(ArraySegment<byte> data);
    Task WriteDataAsync(ArraySegment<byte> data);
    bool IsValid();
    Task SaveAsync();
    bool WriteSynchronous();
}