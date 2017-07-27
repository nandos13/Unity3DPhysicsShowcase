using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    // TODO: Create rope script to hold references to each rope segment. update positions in lateupdate
    [Range(0.1f, 0.4f), SerializeField]    private float _width = 0.15f;
    [SerializeField]    private Material _ropeMaterial = null;
    [SerializeField]    private Vector2 _materialScale = new Vector2(5, 1);
    private LineRenderer _line = null;

    [SerializeField, HideInInspector]   private List<Transform> _segmentTransforms = new List<Transform>();
    [SerializeField, HideInInspector]   private List<Vector3> _positionCache = new List<Vector3>();

	void Awake ()
    {
        // Create renderer for rope
        _line = gameObject.AddComponent<LineRenderer>();
        _line.startWidth = _width;
        _line.endWidth = _width;

        _line.positionCount = 0;
        _line.textureMode = LineTextureMode.RepeatPerSegment;

        if (_ropeMaterial != null)
        {
            _line.material = _ropeMaterial;
            _ropeMaterial.SetTextureScale("_MainTex", _materialScale);
        }
    }
	
	void Update ()
    {
        // Get all segment positions
        _positionCache.Clear();
        foreach (Transform t in _segmentTransforms)
        {
            _positionCache.Add(t.position);
        }

        // Set line renderer to draw over these segments
        _line.positionCount = _positionCache.Count;
        _line.SetPositions(_positionCache.ToArray());
	}

    private void AddSegmentTransform(Transform t)
    {
        _segmentTransforms.Add(t);
    }

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

        // Add a RopeController script
        RopeController ropeController = ropeEmpty.AddComponent<RopeController>();
        ropeController.AddSegmentTransform(ropeEmpty.transform);

        Rigidbody ropeRB = ropeEmpty.AddComponent<Rigidbody>();
        ropeRB.isKinematic = true;

        float segmentSpacing = length / (float)segments;

        // Create segments of rope
        Transform previousTransform = ropeEmpty.transform;
        Rigidbody previousRB = ropeRB;
        for (uint i = 0; i < segments; i++)
        {
            // Create a new gameobject for the segment
            Vector3 pos = new Vector3(0, -((i + 1) * segmentSpacing), 0) + previousTransform.position;

            GameObject segmentEnd = new GameObject();
            segmentEnd.transform.parent = ropeEmpty.transform;
            segmentEnd.name = "Segment " + i;
            segmentEnd.transform.position = pos;

            // Track this segment's transform
            ropeController.AddSegmentTransform(segmentEnd.transform);

            // Add a rigidbody, and a hinge joint connected to the previous segment
            Rigidbody segmentRB = segmentEnd.AddComponent<Rigidbody>();
            HingeJoint segmentHinge = segmentEnd.AddComponent<HingeJoint>();
            segmentHinge.connectedBody = previousRB;

            // Add collider
            SphereCollider collider = segmentEnd.AddComponent<SphereCollider>();
            collider.radius = segmentSpacing / 3;

            // Track this as previous rigidbody for use with next segment
            previousRB = segmentRB;
        }
    }
}
