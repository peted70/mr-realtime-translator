using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpeechItem))]
public class SpeechItemDrawer : PropertyDrawer
{
    static SpeechItem[] languages =
    {
        new SpeechItem
        {
            name = "ar-EG",
            displayname = "Arabic",
            language = "ar",
        },
        new SpeechItem
        {
            name = "de-DE",
            displayname = "German",
            language = "de",
        },
        new SpeechItem
        {
            name = "en-US",
            displayname = "English",
            language = "en",
        },
        new SpeechItem
        {
            name = "es-ES",
            displayname = "Spanish",
            language = "es",
        },
        new SpeechItem
        {
            name = "fr-FR",
            displayname = "French",
            language = "fr",
        },
        new SpeechItem
        {
            name = "it-IT",
            displayname = "Italian",
            language = "it",
        },
        new SpeechItem
        {
            name = "ja-JP",
            displayname = "Japanese",
            language = "ja",
        },
        new SpeechItem
        {
            name = "pt-BR",
            displayname = "Portuguese",
            language = "pt",
        },
        new SpeechItem
        {
            name = "ru-RU",
            displayname = "Russian",
            language = "ru",
        },
        new SpeechItem
        {
            name = "zh-CN",
            displayname = "Chinese Simplified",
            language = "zh-Hans",
        },
        new SpeechItem
        {
            name = "zh-TW",
            displayname = "Chinese Traditional",
            language = "zh-Hant",
        },
    };

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var translatorObject = (RealTimeTranslator)property.serializedObject.targetObject;
        var pi = translatorObject.GetType().GetField(property.name);

        var propValue = (SpeechItem)pi.GetValue(translatorObject);
        var langIdx = Array.FindIndex(languages, l => l.name == propValue.name);

        var options = languages.Select(l => l.displayname).ToArray();
        var selectedRect = new Rect(position.x, position.y, position.width, position.height);

        langIdx = EditorGUI.Popup(selectedRect, property.displayName, langIdx, options, EditorStyles.popup);
        if (langIdx > -1 && langIdx < languages.Length)
        {
            // Set the value back into our object - when this happens we also 
            // need to update the voice item..
            pi.SetValue(translatorObject, languages[langIdx]);
        }

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(VoiceItem))]
public class VoiceItemDrawer : PropertyDrawer
{
    private static Dictionary<string, VoiceItem> VoiceDict = new Dictionary<string, VoiceItem>
    {
        {
            "ar-EG-Hoda",
            new VoiceItem
            {
                gender = "female",
                locale = "ar-EG",
                languageName = "Arabic",
                displayName = "Hoda",
                regionName = "Egypt",
                language = "ar",
            }
        },
        {
            "ar-SA-Naayf",
            new VoiceItem
            {
                gender = "male",
                locale = "ar-SA",
                languageName = "Arabic",
                displayName = "Naayf",
                regionName = "Saudi Arabia",
                language = "ar"
            }
        },
        {
            "ca-ES-Herena",
            new VoiceItem
            {
                gender = "female",
                locale = "ca-ES",
                languageName = "Catalan",
                displayName = "Herena",
                regionName = "Spain",
                language = "ca"
            }
        },
        {
            "cs-CZ-Vit",
            new VoiceItem
            {
                gender = "male",
                locale = "cs-CZ",
                languageName = "Czech",
                displayName = "Vit",
                regionName = "Czech Republic",
                language = "cs"
            }
        },
        {
            "da-DK-Helle",
            new VoiceItem
            {
                gender = "female",
                locale = "da-DK",
                languageName = "Danish",
                displayName = "Helle",
                regionName = "Denmark",
                language = "da"
            }
        },
        {
            "de-AT-Michael",
            new VoiceItem
            {
                gender = "male",
                locale = "de-AT",
                languageName = "German",
                displayName = "Michael",
                regionName = "Austria",
                language = "de"
            }
        },
        {
            "de-CH-Karsten",
            new VoiceItem
            {
                gender = "male",
                locale = "de-CH",
                languageName = "German",
                displayName = "Karsten",
                regionName = "Switzerland",
                language = "de"
            }
        },
        {
            "de-DE-Hedda",
            new VoiceItem
            {
                gender = "female",
                locale = "de-DE",
                languageName = "German",
                displayName = "Hedda",
                regionName = "Germany",
                language = "de"
            }
        },
        {
            "de-DE-Stefan",
            new VoiceItem
            {
                gender = "male",
                locale = "de-DE",
                languageName = "German",
                displayName = "Stefan",
                regionName = "Germany",
                language = "de"
            }
        },
        {
            "de-DE-Katja",
            new VoiceItem
            {
                gender = "female",
                locale = "de-DE",
                languageName = "German",
                displayName = "Katja",
                regionName = "Germany",
                language = "de"
            }
        },
        {
            "el-GR-Stefanos",
            new VoiceItem
            {
                gender = "male",
                locale = "el-GR",
                languageName = "Greek",
                displayName = "Stefanos",
                regionName = "Greece",
                language = "el"
            }
        },
        {
            "en-AU-Catherine",
            new VoiceItem
            {
                gender = "female",
                locale = "en-AU",
                languageName = "English",
                displayName = "Catherine",
                regionName = "Australia",
                language = "en"
            }
        },
        {
            "en-AU-James",
            new VoiceItem
            {
                gender = "male",
                locale = "en-AU",
                languageName = "English",
                displayName = "James",
                regionName = "Australia",
                language = "en"
            }
        },
        {
            "en-CA-Linda",
            new VoiceItem
            {
                gender = "female",
                locale = "en-CA",
                languageName = "English",
                displayName = "Linda",
                regionName = "Canada",
                language = "en"
            }
        },
        {
            "en-CA-Richard",
            new VoiceItem
            {
            gender = "male",
            locale = "en-CA",
            languageName = "English",
            displayName = "Richard",
            regionName = "Canada",
            language = "en"
            }
        },
        {
        "en-GB-George",
            new VoiceItem
            {
            gender = "male",
            locale = "en-GB",
            languageName = "English",
            displayName = "George",
            regionName = "United Kingdom",
            language = "en"
            }
        },
        {
        "en-GB-Susan",
            new VoiceItem
            {
            gender = "female",
            locale = "en-GB",
            languageName = "English",
            displayName = "Susan",
            regionName = "United Kingdom",
            language = "en"
            }
        },
        {
        "en-IN-Heera",
            new VoiceItem
            {
            gender = "female",
            locale = "en-IN",
            languageName = "English",
            displayName = "Heera",
            regionName = "India",
            language = "en"
            }
        },
        {
        "en-IN-Ravi",
            new VoiceItem
            {
            gender = "male",
            locale = "en-IN",
            languageName = "English",
            displayName = "Ravi",
            regionName = "India",
            language = "en"
            }
        },
        {
        "en-US-BenjaminRUS",
            new VoiceItem
            {
            gender = "male",
            locale = "en-US",
            languageName = "English",
            displayName = "BenjaminRUS",
            regionName = "United States",
            language = "en"
            }
        },
        {
        "en-US-JessaRUS",
            new VoiceItem
            {
            gender = "female",
            locale = "en-US",
            languageName = "English",
            displayName = "JessaRUS",
            regionName = "United States",
            language = "en"
            }
        },
        {
        "en-US-Mark",
            new VoiceItem
            {
            gender = "male",
            locale = "en-US",
            languageName = "English",
            displayName = "Mark",
            regionName = "United States",
            language = "en"
            }
        },
        {
        "en-US-Zira",
            new VoiceItem
            {
            gender = "female",
            locale = "en-US",
            languageName = "English",
            displayName = "Zira",
            regionName = "United States",
            language = "en"
            }
        },
        {
        "en-US-ZiraRUS",
            new VoiceItem
            {
            gender = "female",
            locale = "en-US",
            languageName = "English",
            displayName = "ZiraRUS",
            regionName = "United States",
            language = "en"
            }
        },
        {
        "es-ES-Laura",
            new VoiceItem
            {
            gender = "female",
            locale = "es-ES",
            languageName = "Spanish",
            displayName = "Laura",
            regionName = "Spain",
            language = "es"
            }
        },
        {
        "es-ES-Pablo",
            new VoiceItem
            {
            gender = "male",
            locale = "es-ES",
            languageName = "Spanish",
            displayName = "Pablo",
            regionName = "Spain",
            language = "es"
            }
        },
        {
        "es-MX-Raul",
            new VoiceItem
            {
            gender = "male",
            locale = "es-MX",
            languageName = "Spanish",
            displayName = "Raul",
            regionName = "Mexico",
            language = "es"
            }
        },{
        "es-MX-Sabina",
            new VoiceItem
            {
            gender = "female",
            locale = "es-MX",
            languageName = "Spanish",
            displayName = "Sabina",
            regionName = "Mexico",
            language = "es"
            }
        },
        {
        "fi-FI-Heidi",
            new VoiceItem
            {
            gender = "female",
            locale = "fi-FI",
            languageName = "Finnish",
            displayName = "Heidi",
            regionName = "Finland",
            language = "fi"
            }
        },
        {
        "fr-CA-Caroline",
            new VoiceItem
            {
            gender = "female",
            locale = "fr-CA",
            languageName = "French",
            displayName = "Caroline",
            regionName = "Canada",
            language = "fr"
            }
        },
        {
        "fr-CA-Claude",
            new VoiceItem
            {
            gender = "male",
            locale = "fr-CA",
            languageName = "French",
            displayName = "Claude",
            regionName = "Canada",
            language = "fr"
            }
        },
        {
        "fr-FR-Julie",
            new VoiceItem
            {
            gender = "female",
            locale = "fr-FR",
            languageName = "French",
            displayName = "Julie",
            regionName = "France",
            language = "fr"
            }
        },
        {
        "fr-FR-Paul",
            new VoiceItem
            {
            gender = "male",
            locale = "fr-FR",
            languageName = "French",
            displayName = "Paul",
            regionName = "France",
            language = "fr"
            }
        },
        {
        "he-IL-Asaf",
            new VoiceItem
            {
            gender = "male",
            locale = "he-IL",
            languageName = "Hebrew",
            displayName = "Asaf",
            regionName = "Israel",
            language = "he"
            }
        },
        {
        "hi-IN-Hemant",
            new VoiceItem
            {
            gender = "male",
            locale = "hi-IN",
            languageName = "Hindi",
            displayName = "Hemant",
            regionName = "India",
            language = "hi"
            }
        },
        {
        "hi-IN-Kalpana",
            new VoiceItem
            {
            gender = "female",
            locale = "hi-IN",
            languageName = "Hindi",
            displayName = "Kalpana",
            regionName = "India",
            language = "hi"
            }
        },
        {
        "hu-HU-Szabolcs",
            new VoiceItem
            {
            gender = "male",
            locale = "hu-HU",
            languageName = "Hungarian",
            displayName = "Szabolcs",
            regionName = "Hungary",
            language = "hu"
            }
        },
        {
        "id-ID-Andika",
            new VoiceItem
            {
            gender = "male",
            locale = "id-ID",
            languageName = "Indonesian",
            displayName = "Andika",
            regionName = "Indonesia",
            language = "id"
            }
        },
        {
        "it-IT-Cosimo",
            new VoiceItem
            {
            gender = "male",
            locale = "it-IT",
            languageName = "Italian",
            displayName = "Cosimo",
            regionName = "Italy",
            language = "it"
            }
        },
        {
        "it-IT-Elsa",
            new VoiceItem
            {
            gender = "female",
            locale = "it-IT",
            languageName = "Italian",
            displayName = "Elsa",
            regionName = "Italy",
            language = "it"
            }
        },
        {
        "ja-JP-Ayumi",
            new VoiceItem
            {
            gender = "female",
            locale = "ja-JP",
            languageName = "Japanese",
            displayName = "Ayumi",
            regionName = "Japan",
            language = "ja"
            }
        },
        {
        "ja-JP-Ichiro",
            new VoiceItem
            {
            gender = "male",
            locale = "ja-JP",
            languageName = "Japanese",
            displayName = "Ichiro",
            regionName = "Japan",
            language = "ja"
            }
        },
        {
        "ja-JP-Watanabe",
            new VoiceItem
            {
            gender = "female",
            locale = "ja-JP",
            languageName = "Japanese",
            displayName = "Watanabe",
            regionName = "Japan",
            language = "ja"
            }
        },
        {
        "ko-KR-Minjoon",
            new VoiceItem
            {
            gender = "male",
            locale = "ko-KR",
            languageName = "Korean",
            displayName = "Minjoon",
            regionName = "Korea",
            language = "ko"
            }
        },
        {
        "ko-KR-Seohyun",
            new VoiceItem
            {
            gender = "female",
            locale = "ko-KR",
            languageName = "Korean",
            displayName = "Seohyun",
            regionName = "Korea",
            language = "ko"
            }
        },
        {
        "nb-NO-Jon",
            new VoiceItem
            {
            gender = "male",
            locale = "nb-NO",
            languageName = "Norwegian",
            displayName = "Jon",
            regionName = "Norway",
            language = "nb"
            }
        },
        {
        "nb-NO-Nina",
            new VoiceItem
            {
            gender = "female",
            locale = "nb-NO",
            languageName = "Norwegian",
            displayName = "Nina",
            regionName = "Norway",
            language = "nb"
            }
        },
        {
        "nl-NL-Frank",
            new VoiceItem
            {
            gender = "male",
            locale = "nl-NL",
            languageName = "Dutch",
            displayName = "Frank",
            regionName = "Netherlands",
            language = "nl"
            }
        },
        {
        "nl-NL-Marijke",
            new VoiceItem
            {
            gender = "female",
            locale = "nl-NL",
            languageName = "Dutch",
            displayName = "Marijke",
            regionName = "Netherlands",
            language = "nl"
            }
        },
        {
        "pl-PL-Adam",
            new VoiceItem
            {
            gender = "male",
            locale = "pl-PL",
            languageName = "Polish",
            displayName = "Adam",
            regionName = "Poland",
            language = "pl"
            }
        },
        {
        "pl-PL-Paulina",
            new VoiceItem
            {
            gender = "female",
            locale = "pl-PL",
            languageName = "Polish",
            displayName = "Paulina",
            regionName = "Poland",
            language = "pl"
            }
        },
        {
        "pt-BR-Daniel",
            new VoiceItem
            {
            gender = "male",
            locale = "pt-BR",
            languageName = "Portuguese",
            displayName = "Daniel",
            regionName = "Brazil",
            language = "pt"
            }
        },
        {
        "pt-BR-Maria",
            new VoiceItem
            {
            gender = "female",
            locale = "pt-BR",
            languageName = "Portuguese",
            displayName = "Maria",
            regionName = "Brazil",
            language = "pt"
            }
        },
        {
        "pt-PT-Helia",
            new VoiceItem
            {
            gender = "female",
            locale = "pt-PT",
            languageName = "Portuguese",
            displayName = "Helia",
            regionName = "Portugal",
            language = "pt"
            }
        },
        {
        "ro-RO-Andrei",
            new VoiceItem
            {
            gender = "male",
            locale = "ro-RO",
            languageName = "Romanian",
            displayName = "Andrei",
            regionName = "Romania",
            language = "ro"
            }
        },
        {
        "ru-RU-Irina",
            new VoiceItem
            {
            gender = "female",
            locale = "ru-RU",
            languageName = "Russian",
            displayName = "Irina",
            regionName = "Russia",
            language = "ru"
            }
        },
        {
        "ru-RU-Pavel",
            new VoiceItem
            {
            gender = "male",
            locale = "ru-RU",
            languageName = "Russian",
            displayName = "Pavel",
            regionName = "Russia",
            language = "ru"
            }
        },
        {
        "sk-SK-Filip",
            new VoiceItem
            {
            gender = "male",
            locale = "sk-SK",
            languageName = "Slovak",
            displayName = "Filip",
            regionName = "Slovakia",
            language = "sk"
            }
        },
        {
        "sv-SE-Bengt",
            new VoiceItem
            {
            gender = "male",
            locale = "sv-SE",
            languageName = "Swedish",
            displayName = "Bengt",
            regionName = "Sweden",
            language = "sv"
            }
        },
        {
        "sv-SE-Karin",
            new VoiceItem
            {
            gender = "female",
            locale = "sv-SE",
            languageName = "Swedish",
            displayName = "Karin",
            regionName = "Sweden",
            language = "sv"
            }
        },
        {
        "th-TH-Pattara",
            new VoiceItem
            {
            gender = "male",
            locale = "th-TH",
            languageName = "Thai",
            displayName = "Pattara",
            regionName = "Thailand",
            language = "th"
            }
        },
        {
        "tr-TR-Seda",
            new VoiceItem
            {
            gender = "female",
            locale = "tr-TR",
            languageName = "Turkish",
            displayName = "Seda",
            regionName = "Turkey",
            language = "tr"
            }
        },
        {
        "tr-TR-Tolga",
            new VoiceItem
            {
            gender = "male",
            locale = "tr-TR",
            languageName = "Turkish",
            displayName = "Tolga",
            regionName = "Turkey",
            language = "tr"
            }
        },
        {
        "zh-CN-Kangkang",
            new VoiceItem
            {
            gender = "male",
            locale = "zh-CN",
            languageName = "Chinese Simplified",
            displayName = "Kangkang",
            regionName = "People's Republic of China",
            language = "zh-Hans"
            }
        },
        {
        "zh-CN-Yaoyao",
            new VoiceItem
            {
            gender = "female",
            locale = "zh-CN",
            languageName = "Chinese Simplified",
            displayName = "Yaoyao",
            regionName = "People's Republic of China",
            language = "zh-Hans"
            }
        },
        {
        "zh-HK-Danny",
            new VoiceItem
            {
            gender = "male",
            locale = "zh-HK",
            languageName = "Cantonese (Traditional)",
            displayName = "Danny",
            regionName = "Hong Kong S.A.R.",
            language = "yue"
            }
        },
        {
        "zh-HK-Tracy",
            new VoiceItem
            {
            gender = "female",
            locale = "zh-HK",
            languageName = "Cantonese (Traditional)",
            displayName = "Tracy",
            regionName = "Hong Kong S.A.R.",
            language = "yue"
            }
        },
        {
        "zh-TW-Yating",
            new VoiceItem
            {
            gender = "female",
            locale = "zh-TW",
            languageName = "Chinese Traditional",
            displayName = "Yating",
            regionName = "Taiwan",
            language = "zh-Hant"
            }
        },
        {
        "zh-TW-Zhiwei",
            new VoiceItem
            {
            gender = "male",
            locale = "zh-TW",
            languageName = "Chinese Traditional",
            displayName = "Zhiwei",
            regionName = "Taiwan",
            language = "zh-Hant"
            }
        }
    };

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var translatorObject = (RealTimeTranslator)property.serializedObject.targetObject;
        var toLanguage = translatorObject.ToLanguage;
        var voiceOptions = VoiceDict.Where(v => v.Value.locale == toLanguage.name).Select(p => p.Value.displayName).ToArray();

        var voice = translatorObject.Voice;
        var voiceIdx = Array.FindIndex(voiceOptions, v => v == voice.displayName);

        var selectedRect = new Rect(position.x, position.y, position.width, position.height);

        voiceIdx = EditorGUI.Popup(selectedRect, property.displayName, voiceIdx, voiceOptions, EditorStyles.popup);

        if (voiceIdx > -1 && voiceIdx < voiceOptions.Length)
        {
            // Set the value back into our object - when this happens we also 
            // need to update the voice item..
            var newVoice = VoiceDict.Where(v => v.Value.displayName == voiceOptions[voiceIdx]).Single().Value;
            Debug.Log("Voice Property Drawer" + newVoice.displayName);
            translatorObject.Voice = newVoice;
        }

        EditorGUI.EndProperty();
    }
}
