using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_AreaMessage_Trigger))]
[CanEditMultipleObjects]
public class EDITOR_AreaMessage_Trigger : Editor
{
    // Serialized properties
    SerializedProperty messageControllerProp;
    SerializedProperty messageProp;
    SerializedProperty displayTimeProp;
    SerializedProperty canBeReTriggeredProp;

    private void OnEnable()
    {
        messageControllerProp = serializedObject.FindProperty("messageController");
        messageProp = serializedObject.FindProperty("message");
        displayTimeProp = serializedObject.FindProperty("displayTime");
        canBeReTriggeredProp = serializedObject.FindProperty("canBeReTriggered");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        if (GUILayout.Button("Find Message Controller"))
        {
            Undo.RecordObject(serializedObject.targetObject, "Searched for Message Controller");
            messageControllerProp.objectReferenceValue = FindObjectOfType<SCR_InGameMessageController>();
        }
        serializedObject.ApplyModifiedProperties();
    }


}
