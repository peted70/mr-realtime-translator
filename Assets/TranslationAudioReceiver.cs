using UnityEngine;

public class TranslationAudioReceiver : AudioReceivedHandler
{
    public AudioSource audioSource;

    public override void OnAudioReceived(AudioDataReceivedEventArgs args)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            var clip = AudioClip.Create("TranslationResponse", args.Data.Length, 1, 16000, false);
            clip.SetData(args.Data, 0);
            audioSource.playOnAwake = true;
            audioSource.clip = clip;
            audioSource.volume = 1;
            audioSource.enabled = true;
            audioSource.Stop();
            audioSource.PlayOneShot(clip);
            Debug.Log("Playing audio");
        },
        false);
    }
}
