using UnityEngine;

public class TranslationTextReceiver : TextReceivedHandler
{
    public TextMesh TranslationText;
    public TextMesh RecognitionText;
    public override void OnTextReceived(Result result)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() => 
        {
            TranslationText.text = result.translation;
            RecognitionText.text = result.recognition;

        }, false);
    }
}
