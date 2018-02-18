using UnityEngine;

public abstract class AudioReceivedHandler : MonoBehaviour
{
    public abstract void OnAudioReceived(AudioDataReceivedEventArgs args);
}
