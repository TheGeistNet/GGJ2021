using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_StaticSpikes))]
[CanEditMultipleObjects]
public class EDIT_StaticSpikes : Editor
{
    SCR_StaticSpikes script;
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        script = target as SCR_StaticSpikes;
        serializedObject.Update();
        DrawDefaultInspector();
        if (GUILayout.Button("Layout"))
        {
            script.LayoutSingleSpikes();
        }
        serializedObject.ApplyModifiedProperties();
    }
}