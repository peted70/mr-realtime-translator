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

        "tts": {
        "ar-EG-Hoda": {
            "gender": "female",
            "locale": "ar-EG",
            "languageName": "Arabic",
            "displayName": "Hoda",
            "regionName": "Egypt",
            "language": "ar"
        },
        "ar-SA-Naayf": {
            "gender": "male",
            "locale": "ar-SA",
            "languageName": "Arabic",
            "displayName": "Naayf",
            "regionName": "Saudi Arabia",
            "language": "ar"
        },
        "ca-ES-Herena": {
            "gender": "female",
            "locale": "ca-ES",
            "languageName": "Catalan",
            "displayName": "Herena",
            "regionName": "Spain",
            "language": "ca"
        },
        "cs-CZ-Vit": {
            "gender": "male",
            "locale": "cs-CZ",
            "languageName": "Czech",
            "displayName": "Vit",
            "regionName": "Czech Republic",
            "language": "cs"
        },
        "da-DK-Helle": {
            "gender": "female",
            "locale": "da-DK",
            "languageName": "Danish",
            "displayName": "Helle",
            "regionName": "Denmark",
            "language": "da"
        },
        "de-AT-Michael": {
            "gender": "male",
            "locale": "de-AT",
            "languageName": "German",
            "displayName": "Michael",
            "regionName": "Austria",
            "language": "de"
        },
        "de-CH-Karsten": {
            "gender": "male",
            "locale": "de-CH",
            "languageName": "German",
            "displayName": "Karsten",
            "regionName": "Switzerland",
            "language": "de"
        },
        "de-DE-Hedda": {
            "gender": "female",
            "locale": "de-DE",
            "languageName": "German",
            "displayName": "Hedda",
            "regionName": "Germany",
            "language": "de"
        },
        "de-DE-Stefan": {
            "gender": "male",
            "locale": "de-DE",
            "languageName": "German",
            "displayName": "Stefan",
            "regionName": "Germany",
            "language": "de"
        },
        "de-DE-Katja": {
            "gender": "female",
            "locale": "de-DE",
            "languageName": "German",
            "displayName": "Katja",
            "regionName": "Germany",
            "language": "de"
        },
        "el-GR-Stefanos": {
            "gender": "male",
            "locale": "el-GR",
            "languageName": "Greek",
            "displayName": "Stefanos",
            "regionName": "Greece",
            "language": "el"
        },
        "en-AU-Catherine": {
            "gender": "female",
            "locale": "en-AU",
            "languageName": "English",
            "displayName": "Catherine",
            "regionName": "Australia",
            "language": "en"
        },
        "en-AU-James": {
            "gender": "male",
            "locale": "en-AU",
            "languageName": "English",
            "displayName": "James",
            "regionName": "Australia",
            "language": "en"
        },
        "en-CA-Linda": {
            "gender": "female",
            "locale": "en-CA",
            "languageName": "English",
            "displayName": "Linda",
            "regionName": "Canada",
            "language": "en"
        },
        "en-CA-Richard": {
            "gender": "male",
            "locale": "en-CA",
            "languageName": "English",
            "displayName": "Richard",
            "regionName": "Canada",
            "language": "en"
        },
        "en-GB-George": {
            "gender": "male",
            "locale": "en-GB",
            "languageName": "English",
            "displayName": "George",
            "regionName": "United Kingdom",
            "language": "en"
        },
        "en-GB-Susan": {
            "gender": "female",
            "locale": "en-GB",
            "languageName": "English",
            "displayName": "Susan",
            "regionName": "United Kingdom",
            "language": "en"
        },
        "en-IN-Heera": {
            "gender": "female",
            "locale": "en-IN",
            "languageName": "English",
            "displayName": "Heera",
            "regionName": "India",
            "language": "en"
        },
        "en-IN-Ravi": {
            "gender": "male",
            "locale": "en-IN",
            "languageName": "English",
            "displayName": "Ravi",
            "regionName": "India",
            "language": "en"
        },
        "en-US-BenjaminRUS": {
            "gender": "male",
            "locale": "en-US",
            "languageName": "English",
            "displayName": "BenjaminRUS",
            "regionName": "United States",
            "language": "en"
        },
        "en-US-JessaRUS": {
            "gender": "female",
            "locale": "en-US",
            "languageName": "English",
            "displayName": "JessaRUS",
            "regionName": "United States",
            "language": "en"
        },
        "en-US-Mark": {
            "gender": "male",
            "locale": "en-US",
            "languageName": "English",
            "displayName": "Mark",
            "regionName": "United States",
            "language": "en"
        },
        "en-US-Zira": {
            "gender": "female",
            "locale": "en-US",
            "languageName": "English",
            "displayName": "Zira",
            "regionName": "United States",
            "language": "en"
        },
        "en-US-ZiraRUS": {
            "gender": "female",
            "locale": "en-US",
            "languageName": "English",
            "displayName": "ZiraRUS",
            "regionName": "United States",
            "language": "en"
        },
        "es-ES-Laura": {
            "gender": "female",
            "locale": "es-ES",
            "languageName": "Spanish",
            "displayName": "Laura",
            "regionName": "Spain",
            "language": "es"
        },
        "es-ES-Pablo": {
            "gender": "male",
            "locale": "es-ES",
            "languageName": "Spanish",
            "displayName": "Pablo",
            "regionName": "Spain",
            "language": "es"
        },
        "es-MX-Raul": {
            "gender": "male",
            "locale": "es-MX",
            "languageName": "Spanish",
            "displayName": "Raul",
            "regionName": "Mexico",
            "language": "es"
        },
        "es-MX-Sabina": {
            "gender": "female",
            "locale": "es-MX",
            "languageName": "Spanish",
            "displayName": "Sabina",
            "regionName": "Mexico",
            "language": "es"
        },
        "fi-FI-Heidi": {
            "gender": "female",
            "locale": "fi-FI",
            "languageName": "Finnish",
            "displayName": "Heidi",
            "regionName": "Finland",
            "language": "fi"
        },
        "fr-CA-Caroline": {
            "gender": "female",
            "locale": "fr-CA",
            "languageName": "French",
            "displayName": "Caroline",
            "regionName": "Canada",
            "language": "fr"
        },
        "fr-CA-Claude": {
            "gender": "male",
            "locale": "fr-CA",
            "languageName": "French",
            "displayName": "Claude",
            "regionName": "Canada",
            "language": "fr"
        },
        "fr-FR-Julie": {
            "gender": "female",
            "locale": "fr-FR",
            "languageName": "French",
            "displayName": "Julie",
            "regionName": "France",
            "language": "fr"
        },
        "fr-FR-Paul": {
            "gender": "male",
            "locale": "fr-FR",
            "languageName": "French",
            "displayName": "Paul",
            "regionName": "France",
            "language": "fr"
        },
        "he-IL-Asaf": {
            "gender": "male",
            "locale": "he-IL",
            "languageName": "Hebrew",
            "displayName": "Asaf",
            "regionName": "Israel",
            "language": "he"
        },
        "hi-IN-Hemant": {
            "gender": "male",
            "locale": "hi-IN",
            "languageName": "Hindi",
            "displayName": "Hemant",
            "regionName": "India",
            "language": "hi"
        },
        "hi-IN-Kalpana": {
            "gender": "female",
            "locale": "hi-IN",
            "languageName": "Hindi",
            "displayName": "Kalpana",
            "regionName": "India",
            "language": "hi"
        },
        "hu-HU-Szabolcs": {
            "gender": "male",
            "locale": "hu-HU",
            "languageName": "Hungarian",
            "displayName": "Szabolcs",
            "regionName": "Hungary",
            "language": "hu"
        },
        "id-ID-Andika": {
            "gender": "male",
            "locale": "id-ID",
            "languageName": "Indonesian",
            "displayName": "Andika",
            "regionName": "Indonesia",
            "language": "id"
        },
        "it-IT-Cosimo": {
            "gender": "male",
            "locale": "it-IT",
            "languageName": "Italian",
            "displayName": "Cosimo",
            "regionName": "Italy",
            "language": "it"
        },
        "it-IT-Elsa": {
            "gender": "female",
            "locale": "it-IT",
            "languageName": "Italian",
            "displayName": "Elsa",
            "regionName": "Italy",
            "language": "it"
        },
        "ja-JP-Ayumi": {
            "gender": "female",
            "locale": "ja-JP",
            "languageName": "Japanese",
            "displayName": "Ayumi",
            "regionName": "Japan",
            "language": "ja"
        },
        "ja-JP-Ichiro": {
            "gender": "male",
            "locale": "ja-JP",
            "languageName": "Japanese",
            "displayName": "Ichiro",
            "regionName": "Japan",
            "language": "ja"
        },
        "ja-JP-Watanabe": {
            "gender": "female",
            "locale": "ja-JP",
            "languageName": "Japanese",
            "displayName": "Watanabe",
            "regionName": "Japan",
            "language": "ja"
        },
        "ko-KR-Minjoon": {
            "gender": "male",
            "locale": "ko-KR",
            "languageName": "Korean",
            "displayName": "Minjoon",
            "regionName": "Korea",
            "language": "ko"
        },
        "ko-KR-Seohyun": {
            "gender": "female",
            "locale": "ko-KR",
            "languageName": "Korean",
            "displayName": "Seohyun",
            "regionName": "Korea",
            "language": "ko"
        },
        "nb-NO-Jon": {
            "gender": "male",
            "locale": "nb-NO",
            "languageName": "Norwegian",
            "displayName": "Jon",
            "regionName": "Norway",
            "language": "nb"
        },
        "nb-NO-Nina": {
            "gender": "female",
            "locale": "nb-NO",
            "languageName": "Norwegian",
            "displayName": "Nina",
            "regionName": "Norway",
            "language": "nb"
        },
        "nl-NL-Frank": {
            "gender": "male",
            "locale": "nl-NL",
            "languageName": "Dutch",
            "displayName": "Frank",
            "regionName": "Netherlands",
            "language": "nl"
        },
        "nl-NL-Marijke": {
            "gender": "female",
            "locale": "nl-NL",
            "languageName": "Dutch",
            "displayName": "Marijke",
            "regionName": "Netherlands",
            "language": "nl"
        },
        "pl-PL-Adam": {
            "gender": "male",
            "locale": "pl-PL",
            "languageName": "Polish",
            "displayName": "Adam",
            "regionName": "Poland",
            "language": "pl"
        },
        "pl-PL-Paulina": {
            "gender": "female",
            "locale": "pl-PL",
            "languageName": "Polish",
            "displayName": "Paulina",
            "regionName": "Poland",
            "language": "pl"
        },
        "pt-BR-Daniel": {
            "gender": "male",
            "locale": "pt-BR",
            "languageName": "Portuguese",
            "displayName": "Daniel",
            "regionName": "Brazil",
            "language": "pt"
        },
        "pt-BR-Maria": {
            "gender": "female",
            "locale": "pt-BR",
            "languageName": "Portuguese",
            "displayName": "Maria",
            "regionName": "Brazil",
            "language": "pt"
        },
        "pt-PT-Helia": {
            "gender": "female",
            "locale": "pt-PT",
            "languageName": "Portuguese",
            "displayName": "Helia",
            "regionName": "Portugal",
            "language": "pt"
        },
        "ro-RO-Andrei": {
            "gender": "male",
            "locale": "ro-RO",
            "languageName": "Romanian",
            "displayName": "Andrei",
            "regionName": "Romania",
            "language": "ro"
        },
        "ru-RU-Irina": {
            "gender": "female",
            "locale": "ru-RU",
            "languageName": "Russian",
            "displayName": "Irina",
            "regionName": "Russia",
            "language": "ru"
        },
        "ru-RU-Pavel": {
            "gender": "male",
            "locale": "ru-RU",
            "languageName": "Russian",
            "displayName": "Pavel",
            "regionName": "Russia",
            "language": "ru"
        },
        "sk-SK-Filip": {
            "gender": "male",
            "locale": "sk-SK",
            "languageName": "Slovak",
            "displayName": "Filip",
            "regionName": "Slovakia",
            "language": "sk"
        },
        "sv-SE-Bengt": {
            "gender": "male",
            "locale": "sv-SE",
            "languageName": "Swedish",
            "displayName": "Bengt",
            "regionName": "Sweden",
            "language": "sv"
        },
        "sv-SE-Karin": {
            "gender": "female",
            "locale": "sv-SE",
            "languageName": "Swedish",
            "displayName": "Karin",
            "regionName": "Sweden",
            "language": "sv"
        },
        "th-TH-Pattara": {
            "gender": "male",
            "locale": "th-TH",
            "languageName": "Thai",
            "displayName": "Pattara",
            "regionName": "Thailand",
            "language": "th"
        },
        "tr-TR-Seda": {
            "gender": "female",
            "locale": "tr-TR",
            "languageName": "Turkish",
            "displayName": "Seda",
            "regionName": "Turkey",
            "language": "tr"
        },
        "tr-TR-Tolga": {
            "gender": "male",
            "locale": "tr-TR",
            "languageName": "Turkish",
            "displayName": "Tolga",
            "regionName": "Turkey",
            "language": "tr"
        },
        "zh-CN-Kangkang": {
            "gender": "male",
            "locale": "zh-CN",
            "languageName": "Chinese Simplified",
            "displayName": "Kangkang",
            "regionName": "People's Republic of China",
            "language": "zh-Hans"
        },
        "zh-CN-Yaoyao": {
            "gender": "female",
            "locale": "zh-CN",
            "languageName": "Chinese Simplified",
            "displayName": "Yaoyao",
            "regionName": "People's Republic of China",
            "language": "zh-Hans"
        },
        "zh-HK-Danny": {
            "gender": "male",
            "locale": "zh-HK",
            "languageName": "Cantonese (Traditional)",
            "displayName": "Danny",
            "regionName": "Hong Kong S.A.R.",
            "language": "yue"
        },
        "zh-HK-Tracy": {
            "gender": "female",
            "locale": "zh-HK",
            "languageName": "Cantonese (Traditional)",
            "displayName": "Tracy",
            "regionName": "Hong Kong S.A.R.",
            "language": "yue"
        },
        "zh-TW-Yating": {
            "gender": "female",
            "locale": "zh-TW",
            "languageName": "Chinese Traditional",
            "displayName": "Yating",
            "regionName": "Taiwan",
            "language": "zh-Hant"
        },
        "zh-TW-Zhiwei": {
            "gender": "male",
            "locale": "zh-TW",
            "languageName": "Chinese Traditional",
            "displayName": "Zhiwei",
            "regionName": "Taiwan",
            "language": "zh-Hant"
        }

    public int selected = 0;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        //var to = (RealTimeTranslator)property.serializedObject.targetObject;
        

        //var languages = to.languages.languages;
        //if (languages.Count == 0)
        //{
        //    var supported = to.apiProxy.GetLanguageSupportAsync().Result;
        //    languages = supported.languages;
        //}

        var options = languages.Select(l => l.displayname).ToArray();
        var selectedRect = new Rect(position.x, position.y, position.width, position.height);
        selected = EditorGUI.Popup(selectedRect, property.displayName, selected, options, EditorStyles.popup);

        //// Draw label
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        //// Don't make child fields be indented
        //var indent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;

        //// Calculate rects
        //var amountRect = new Rect(position.x, position.y, 30, position.height);
        //var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
        //var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

        //// Draw fields - passs GUIContent.none to each so they are drawn without labels
        //EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
        //EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
        //EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

        //// Set indent back to what it was
        //EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

//[CustomEditor(typeof(SpeechItem))]
//public class SpeechItemEditor : Editor
//{
//    SerializedProperty name;

//    void OnEnable()
//    {
//        name = serializedObject.FindProperty("name");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        EditorGUILayout.PropertyField(name);
//        int selected = 0;
//        string[] options = new string[]
//        {
//            "Start With", "End With", "Contains",
//        };

//        selected = EditorGUILayout.Popup("Awesome Drop down:", selected, options, EditorStyles.popup);
//        serializedObject.ApplyModifiedProperties();
//        //if (lookAtPoint.vector3Value.y > (target as LookAtPoint).transform.position.y)
//        //{
//        //    EditorGUILayout.LabelField("(Above this object)");
//        //}
//        //if (lookAtPoint.vector3Value.y < (target as LookAtPoint).transform.position.y)
//        //{
//        //    EditorGUILayout.LabelField("(Below this object)");
//        //}
//    }
//}
