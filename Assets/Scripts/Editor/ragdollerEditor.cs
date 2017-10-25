using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Ragdoller))]
public class ragdollerEditor : Editor
{

    private ReorderableList _pieceList = null;
    private bool _showList = true;

    private void OnEnable()
    {
        _pieceList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("_pieces"),
                            true, true, true, true);

        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float singleLineHeightDoubled = singleLineHeight * 2;

        _pieceList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Pieces"); };

        _pieceList.elementHeight = singleLineHeightDoubled + singleLineHeightDoubled + singleLineHeight + 4;
        _pieceList.elementHeightCallback =
            (int index) =>
            {
                SerializedProperty element = _pieceList.serializedProperty.GetArrayElementAtIndex(index);
                if (element != null)
                {
                    SerializedProperty shape = element.FindPropertyRelative("_shape");
                    if (shape != null)
                        return singleLineHeightDoubled + singleLineHeightDoubled + 4 + ((shape.enumValueIndex == 1) ? singleLineHeight : 0);
                    else
                        return 1;
                }
                return 1;
            };

        _pieceList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = _pieceList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, singleLineHeight),
                                        element.FindPropertyRelative("_piece"),
                                        GUIContent.none);

                EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, singleLineHeight),
                                        element.FindPropertyRelative("_shape"));

                float line2Y = rect.y + singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, line2Y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_offset"));

                float line3Y = line2Y + singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, line3Y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_rotation"));
                
                float line4Y = line3Y + singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, line4Y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_radius"));

                float line5Y = line4Y + singleLineHeight;
                if (element.FindPropertyRelative("_shape").enumValueIndex == 1)     // capsule
                {
                    EditorGUI.PropertyField(new Rect(rect.x, line5Y, rect.width, singleLineHeight),
                                            element.FindPropertyRelative("_height"));
                }
            };

        _pieceList.onAddCallback = (ReorderableList list) =>
        {
            int i = _pieceList.serializedProperty.arraySize;
            _pieceList.serializedProperty.arraySize += 1;

            SerializedProperty element = _pieceList.serializedProperty.GetArrayElementAtIndex(i);

            SerializedProperty radius = element.FindPropertyRelative("_radius");
            if (radius != null)
                radius.floatValue = 0.15f;

            SerializedProperty piece = element.FindPropertyRelative("_piece");
            if (piece != null)
                piece.objectReferenceValue = null;

            SerializedProperty offset = element.FindPropertyRelative("_offset");
            if (offset != null)
                offset.vector3Value = Vector3.zero;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        
        // Create a toggle for displaying list
        _showList = EditorGUILayout.Toggle(_showList);
        Rect labelRect = GUILayoutUtility.GetLastRect();
        EditorGUI.LabelField(new Rect(labelRect.x + 16, labelRect.y, labelRect.width - 16, labelRect.height),
                                    "Show Pieces List");

        if (_showList && _pieceList != null)
        {
            EditorGUILayout.HelpBox("Note:\nCollider scene gizmos only display correctly when the gameobject's rotation is at Euler(0, 0, 0). The Ragdoller script's OnDrawGizmosSelected function needs to apply the correct matrix to the Gizmos class before drawing in order to fix this. This will be done later, but works well enough now to not worry about it.", MessageType.Info);
            _pieceList.DoLayoutList();
        }

        SerializedProperty startRagdolled = serializedObject.FindProperty("_startRagdolled");
        if (startRagdolled != null)
            EditorGUILayout.PropertyField(startRagdolled);

        serializedObject.ApplyModifiedProperties();
    }
}
