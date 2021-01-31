using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SCR_ScoreboardEntry))]
[CanEditMultipleObjects]
public class EDIT_ScoreboardEntry : Editor
{
    SCR_ScoreboardEntry script;
    static bool rankingOpen = false;

    SerializedProperty index;
    SerializedProperty maxSwaps;
    SerializedProperty maxCollected;
    SerializedProperty rankingStrings;
    SerializedProperty rankingValues;
    void OnEnable()
    {
        index = serializedObject.FindProperty("index");
        maxSwaps = serializedObject.FindProperty("maxSwaps");
        maxCollected = serializedObject.FindProperty("maxCollected");
        rankingStrings = serializedObject.FindProperty("rankingStrings");
        rankingValues = serializedObject.FindProperty("rankingValues");
    }

    public override void OnInspectorGUI()
    {
        script = target as SCR_ScoreboardEntry;
        serializedObject.Update();
        EditorGUILayout.PropertyField(index);
        EditorGUILayout.PropertyField(maxSwaps);
        EditorGUILayout.PropertyField(maxCollected);
        rankingOpen = EditorGUILayout.Foldout(rankingOpen, "Rankings");
        if (rankingOpen)
        {
            for(int x = 0; x < rankingStrings.arraySize; ++x)
            {
                string s = rankingStrings.GetArrayElementAtIndex(x).stringValue;
                int i = rankingValues.GetArrayElementAtIndex(x).intValue;
                GUILayout.BeginHorizontal();
                s = EditorGUILayout.TextField(s);
                i = EditorGUILayout.DelayedIntField(i);
                rankingStrings.GetArrayElementAtIndex(x).stringValue = s;
                rankingValues.GetArrayElementAtIndex(x).intValue = i;
                if(GUILayout.Button("Remove"))
                {
                    rankingStrings.DeleteArrayElementAtIndex(x);
                    rankingValues.DeleteArrayElementAtIndex(x);
                    --x;
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add"))
            {
                rankingStrings.InsertArrayElementAtIndex(rankingStrings.arraySize);
                rankingStrings.GetArrayElementAtIndex(rankingStrings.arraySize - 1).stringValue = "";
                rankingValues.InsertArrayElementAtIndex(rankingValues.arraySize);
                rankingValues.GetArrayElementAtIndex(rankingValues.arraySize - 1).intValue = 0;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}