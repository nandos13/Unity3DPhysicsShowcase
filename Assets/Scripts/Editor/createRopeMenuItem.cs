using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class createRopeMenuItem
{
    [MenuItem("GameObject/3D Object/Physics Objects/Rope", false, 1)]
    public static void CreateNewRopeMenuItem()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected != null)   Debug.Log("Creating rope attached to : " + selected.name);

        RopeCreator.CreateNewRope(Vector3.zero, selected);
    }
}
