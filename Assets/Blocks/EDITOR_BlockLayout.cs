using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_BlockLayout))]
public class EDITOR_BlockLayout : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SCR_BlockLayout blockLayoutScript = (SCR_BlockLayout)target;

        if (GUILayout.Button("Layout Blocks"))
        {
            Undo.RecordObject(target, "Laid out block");
            blockLayoutScript.LayoutBlock();
        }
    }
}
