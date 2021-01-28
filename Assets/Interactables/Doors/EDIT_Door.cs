using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_Door))]
[CanEditMultipleObjects]
public class EDIT_Door : Editor
{
    SCR_Door script;

    SerializedProperty m_StartPosition;
    SerializedProperty m_EndPosition;
    SerializedProperty m_Time;
    void OnEnable()
    {
        m_StartPosition = serializedObject.FindProperty("m_StartPosition");
        m_EndPosition = serializedObject.FindProperty("m_EndPosition");
        m_Time = serializedObject.FindProperty("m_Time");
    }

    public override void OnInspectorGUI()
    {
        script = target as SCR_Door;
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_StartPosition);
        if(GUILayout.Button("Set Start Position To Current"))
        {
            m_StartPosition.vector3Value = script.transform.position;
        }
        EditorGUILayout.PropertyField(m_EndPosition);
        if (GUILayout.Button("Set End Position To Current"))
        {
            m_EndPosition.vector3Value = script.transform.position;
        }
        if (GUILayout.Button("Reset To Start Position"))
        {
            script.transform.position = m_StartPosition.vector3Value;
        }
        EditorGUILayout.PropertyField(m_Time);
        serializedObject.ApplyModifiedProperties();
    }
}