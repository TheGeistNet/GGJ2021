using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_BlockLayout))]
[CanEditMultipleObjects]
public class EDITOR_BlockLayout : Editor
{
    // Serialized properties
    SerializedProperty middlePieceClassProp;
    SerializedProperty leftEndClassProp;
    SerializedProperty rightEndClassProp;
    SerializedProperty middlePieceLengthProp;
    SerializedProperty endLengthProp;
    SerializedProperty heightProp;

    private void OnEnable()
    {
        middlePieceClassProp = serializedObject.FindProperty("middlePieceClass");
        leftEndClassProp = serializedObject.FindProperty("leftEndClass");
        rightEndClassProp = serializedObject.FindProperty("rightEndClass");
        middlePieceLengthProp = serializedObject.FindProperty("middlePieceLength");
        endLengthProp = serializedObject.FindProperty("endLength");
        heightProp = serializedObject.FindProperty("height");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();
        if (!middlePieceClassProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(middlePieceClassProp);
        }
        if (!leftEndClassProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(leftEndClassProp);
        }
        if (!rightEndClassProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(rightEndClassProp);
        }
        if (!middlePieceLengthProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(middlePieceLengthProp);
        }
        if (!endLengthProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(endLengthProp);
        }
        if (!heightProp.hasMultipleDifferentValues)
        {
            EditorGUILayout.PropertyField(heightProp);
        }

        if (GUILayout.Button("Layout Blocks"))
        {
            foreach (SCR_BlockLayout blockLayoutScript in targets)
            {
                Undo.RecordObject(blockLayoutScript, "Laid out block");
                blockLayoutScript.LayoutBlock();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
