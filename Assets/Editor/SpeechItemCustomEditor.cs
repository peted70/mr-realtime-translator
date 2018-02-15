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

        //var to = (RealTimeTranslator)property.serializedObject.targetObject;
        

        //var languages = to.languages.languages;
        //if (languages.Count == 0)
        //{
        //    var supported = to.apiProxy.GetLanguageSupportAsync().Result;
        //    languages = supported.languages;
        //}

        int selected = 0;
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
