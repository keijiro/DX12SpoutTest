using UnityEngine;
using UnityEditor;

namespace Klak.Spout {

[CanEditMultipleObjects]
[CustomEditor(typeof(SpoutReceiver))]
sealed class SpoutReceiverEditor : Editor
{
    SerializedProperty _sourceName;

    static class Labels
    {
        public static readonly GUIContent Select = new GUIContent("Select");
    }

    // Create and show the source name dropdown.
    void ShowSourceNameDropdown(Rect rect)
    {
        var menu = new GenericMenu();
        foreach (var name in SpoutManager.GetSourceNames())
            menu.AddItem(new GUIContent(name), false, OnSelectSource, name);
        menu.DropDown(rect);
    }

    // Source name selection callback
    void OnSelectSource(object name)
    {
        serializedObject.Update();
        _sourceName.stringValue = (string)name;
        serializedObject.ApplyModifiedProperties();
    }

    void OnEnable()
    {
        _sourceName = serializedObject.FindProperty("_sourceName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        // Source name text field
        EditorGUILayout.DelayedTextField(_sourceName);

        // Source name dropdown
        var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(60));
        if (EditorGUI.DropdownButton(rect, Labels.Select, FocusType.Keyboard))
            ShowSourceNameDropdown(rect);

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace Klak.Spout
