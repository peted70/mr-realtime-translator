using System;
using System.Collections.Generic;

[Serializable]
public class Languages
{
    public List<SpeechItem> languages = new List<SpeechItem>();
    public Dictionary<string, VoiceItem> Voices { get; set; } = new Dictionary<string, VoiceItem>();
}

[Serializable]
public class SpeechItem
{
    public string name;
    public string displayname;
    public string language;
}

[Serializable]
public class VoiceItem
{
    public string gender;
    public string locale;
    public string languageName;
    public string displayName;
    public string regionName;
    public string language;
}
