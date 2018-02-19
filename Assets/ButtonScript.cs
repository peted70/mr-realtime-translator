using HoloToolkit.Unity.Buttons;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public MicrophoneAudioGetter MicrophoneData;

    // Use this for initialization
    void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.OnButtonClicked += Button_OnButtonClicked;
    }

    private void Button_OnButtonClicked(GameObject obj)
    {
        if (MicrophoneData.IsRecording())
            MicrophoneData.StopRecording();
        else
            MicrophoneData.StartRecording();
    }
}
