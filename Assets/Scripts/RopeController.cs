using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [Range(1, 100), SerializeField] private uint _segments = 5;
    [Range(1.0f, 100.0f), SerializeField]  private float _length = 5.0f;
    [SerializeField, HideInInspector]   private Rigidbody _rigidbody = null;

    [Range(0.1f, 0.4f), SerializeField]    private float _width = 0.15f;
    [SerializeField]    private Material _ropeMaterial = null;
    private LineRenderer _line = null;

    [SerializeField, HideInInspector]   private GameObject _firstSegment = null;
    [SerializeField, HideInInspector]   private List<Transform> _segmentTransforms = new List<Transform>();
    [SerializeField, HideInInspector]   private List<Vector3> _positionCache = new List<Vector3>();

	void Awake ()
    {
        // Create renderer for rope
        _line = gameObject.AddComponent<LineRenderer>();
        _line.startWidth = _width;
        _line.endWidth = _width;

        _line.numCapVertices = 5;
        _line.numCornerVertices = 6;

        _line.SetPositions(new Vector3[0]);
        _line.textureMode = LineTextureMode.Tile;

        // Retrieve rope material
        if (_ropeMaterial == null)
            _ropeMaterial = Resources.Load<Material>("Materials/RopeMaterial");

        if (_ropeMaterial != null)
            _line.material = _ropeMaterial;
    }
	
	void Update ()
    {
        // Get all segment positions
        _positionCache.Clear();
        _positionCache.Add(_firstSegment.transform.position);
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

    private void AddSegments(uint segments, float ropeLength)
    {
        if (!_firstSegment || !_rigidbody)
            return;

        float segmentSpacing = ropeLength / (float)segments;
        float segmentSpacingHalf = segmentSpacing / 2;
        float colliderRadius = Mathf.Min(segmentSpacing / 10, _width / 2);

        // Create segments of rope
        Transform previousTransform = _firstSegment.transform;
        Rigidbody previousRB = _rigidbody;
        for (uint i = 0; i < segments; i++)
        {
            // Create a new gameobject for the segment
            Vector3 pos = new Vector3(0, -((i + 1) * segmentSpacing), 0) + previousTransform.position;

            GameObject segmentEnd = new GameObject();
            segmentEnd.transform.parent = _firstSegment.transform;
            segmentEnd.name = "Segment " + i.ToString();
            segmentEnd.transform.position = pos;

            // Track this segment's transform
            AddSegmentTransform(segmentEnd.transform);

            // Add a rigidbody, and a hinge joint connected to the previous segment
            Rigidbody segmentRB = segmentEnd.AddComponent<Rigidbody>();
            HingeJoint segmentHinge = segmentEnd.AddComponent<HingeJoint>();
            segmentHinge.connectedBody = previousRB;

            // Add collider
            SphereCollider collider = segmentEnd.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;

            // Track this as previous rigidbody for use with next segment
            previousRB = segmentRB;
        }
    }

    public void RegenerateRope()
    {
        // Destroy all old segments
        foreach (Transform t in _segmentTransforms)
            DestroyImmediate(t.gameObject);

        _segmentTransforms.Clear();

        // Generate new segments
        AddSegments(_segments, _length);
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
        ropeController._firstSegment = ropeEmpty;

        // Add a rigidbody
        ropeController._rigidbody = ropeEmpty.AddComponent<Rigidbody>();
        ropeController._rigidbody.isKinematic = true;

        // Generate new segments
        ropeController.AddSegments(segments, length);
        
        // Load rope material
        ropeController._ropeMaterial = Resources.Load<Material>("Materials/RopeMaterial");
    }
}
