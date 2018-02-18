using System.Net;
using System.Net.Security;
#if UNITY_EDITOR
using System.Security.Cryptography.X509Certificates;
#endif
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RealTimeTranslator : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var cfg = AudioSettings.GetConfiguration();
        cfg.dspBufferSize = 0;

#if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = cb;
#endif
    }

#if UNITY_EDITOR
    private bool cb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
#endif
}

