using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_Switch))]
[CanEditMultipleObjects]
public class EDIT_Switch : Editor
{
    SCR_Switch script;

    SerializedProperty m_Retriggerable;
    SerializedProperty m_RetriggerDuration;
    SerializedProperty m_ObjectsToTrigger;
    SerializedProperty m_Filter;
    void OnEnable()
    {
        m_Retriggerable = serializedObject.FindProperty("m_Retriggerable");
        m_Filter = serializedObject.FindProperty("m_Filter");
        m_RetriggerDuration = serializedObject.FindProperty("m_RetriggerDuration");
        m_ObjectsToTrigger = serializedObject.FindProperty("m_ObjectsToTrigger");
    }

    public override void OnInspectorGUI()
    {
        script = target as SCR_Switch;
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Filter);
        EditorGUILayout.PropertyField(m_Retriggerable);
        if (m_Retriggerable.boolValue)
        {
            EditorGUILayout.PropertyField(m_RetriggerDuration);
        }
        EditorGUILayout.PropertyField(m_ObjectsToTrigger);
        serializedObject.ApplyModifiedProperties();
    }
}