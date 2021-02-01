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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

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
