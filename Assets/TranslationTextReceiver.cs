using UnityEngine;

public class TranslationTextReceiver : TextReceivedHandler
{
    public TextMesh text;
    public override void OnTextReceived(Result result)
    {
        UnityEngine.WSA.Application.InvokeOnAppThread(() => { text.text = result.translation; }, false);
    }
}
