using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RopeController))]
public class ropeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        RopeController instance = target as RopeController;
        if (GUILayout.Button(new GUIContent("Regenerate")))
        {
            instance.RegenerateRope();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
