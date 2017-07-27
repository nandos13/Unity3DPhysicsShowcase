using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RopeCreator
{
    public static void CreateNewRope(Vector3 localPosition, GameObject parent = null, uint segments = 5, float length = 5)
    {
        // Create the top level object (anchor-point)
        GameObject ropeEmpty = new GameObject("Rope");
        if (parent)
        {
            ropeEmpty.transform.parent = parent.transform;
            ropeEmpty.transform.localPosition = localPosition;
        }
        else
            ropeEmpty.transform.position = localPosition;

        Rigidbody ropeRB = ropeEmpty.AddComponent<Rigidbody>();
        ropeRB.isKinematic = true;
        
        float segmentSpacing = length / (float)segments;

        // Create segments of rope
        Transform previousTransform = ropeEmpty.transform;
        Rigidbody previousRB = ropeRB;
        for (uint i = 0; i < segments; i++)
        {
            // Create a new gameobject and attach a rigidbody and hinge joint
            Vector3 pos = new Vector3(0, -((i + 1) * segmentSpacing), 0) + previousTransform.position;

            GameObject segmentEnd = new GameObject();
            segmentEnd.transform.parent = ropeEmpty.transform;
            segmentEnd.name = "Segment " + i;
            segmentEnd.transform.position = pos;

            Rigidbody segmentRB = segmentEnd.AddComponent<Rigidbody>();
            HingeJoint segmentHinge = segmentEnd.AddComponent<HingeJoint>();

            // Set hinge joint's attached object to the previous rigidbody
            segmentHinge.connectedBody = previousRB;

            // Track this as previous rigidbody for use with next segment
            previousRB = segmentRB;
        }
    }
}
