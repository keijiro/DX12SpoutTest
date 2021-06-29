using UnityEngine;
using UnityEditor;

namespace Klak.Spout.Editor {

[CanEditMultipleObjects]
[CustomEditor(typeof(SpoutSender))]
sealed class SpoutSenderEditor : UnityEditor.Editor
{
    SerializedProperty _spoutName;
    SerializedProperty _enableAlpha;
    SerializedProperty _captureMethod;
    SerializedProperty _sourceCamera;
    SerializedProperty _sourceTexture;

    static class Labels
    {
        public static Label SpoutName = "Spout Name";
    }

    void OnEnable()
    {
        var finder = new PropertyFinder(serializedObject);
        _spoutName = finder["_spoutName"];
        _enableAlpha = finder["_enableAlpha"];
        _captureMethod = finder["_captureMethod"];
        _sourceCamera = finder["_sourceCamera"];
        _sourceTexture = finder["_sourceTexture"];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.DelayedTextField(_spoutName, Labels.SpoutName);
        var modName = EditorGUI.EndChangeCheck();

        EditorGUILayout.PropertyField(_enableAlpha);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_captureMethod);
        var modMethod = EditorGUI.EndChangeCheck();

        EditorGUI.indentLevel++;

        if (_captureMethod.hasMultipleDifferentValues ||
            _captureMethod.enumValueIndex == (int)CaptureMethod.Camera)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sourceCamera);
            modMethod |= EditorGUI.EndChangeCheck();
        }

        if (_captureMethod.hasMultipleDifferentValues ||
            _captureMethod.enumValueIndex == (int)CaptureMethod.Texture)
            EditorGUILayout.PropertyField(_sourceTexture);

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}

} // namespace Klak.Spout.Editor
