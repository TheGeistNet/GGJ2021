using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_DimensionSwapObserverBase), true)]
[CanEditMultipleObjects]
public class EDIT_DimensionSwapObserverBase : Editor
{
    SCR_DimensionSwapObserverBase script;

    SerializedProperty m_StartSwapped;
    void OnEnable()
    {
        m_StartSwapped = serializedObject.FindProperty("m_StartSwapped");
    }

    public override void OnInspectorGUI()
    {
        script = target as SCR_DimensionSwapObserverBase;
        serializedObject.Update();
        //EditorGUILayout.PropertyField(m_StartSwapped);
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}