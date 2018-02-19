using HoloToolkit.Unity.Buttons;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public MicrophoneAudioGetter MicrophoneData;
    public GameObject recordingIcon;

    // Use this for initialization
    void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.OnButtonClicked += Button_OnButtonClicked;
    }

    private void Button_OnButtonClicked(GameObject obj)
    {
        if (MicrophoneData.IsRecording())
        {
            MicrophoneData.StopRecording();
            if (recordingIcon != null)
                recordingIcon.SetActive(false);
        }
        else
        {
            MicrophoneData.StartRecording();
            if (recordingIcon != null)
                recordingIcon.SetActive(true);
        }
    }
}
