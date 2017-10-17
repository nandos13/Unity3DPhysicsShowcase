using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Ragdoller : MonoBehaviour
{
    [System.Serializable]
    private class RagdollBodyPiece
    {
        public enum COLLIDERTYPE { Sphere, Capsule };

        public COLLIDERTYPE _shape;

        public RagdollBodyPiece _parent = null;

        public Transform _piece = null;

        public Vector3 _offset = new Vector3();
        public Vector3 _rotation = new Vector3();

        [Range(0.01f, 1.0f)]    public float _radius = 0.15f;
        [Range(0.01f, 2.0f)]    public float _height = 1.0f;

        private Collider _collider = null;
        public Collider collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public CharacterJoint _joint = null;
    }

    [SerializeField]
    private List<RagdollBodyPiece> _pieces = new List<RagdollBodyPiece>();

    [SerializeField]
    private bool _startRagdolled = false;
    private bool _currentRagdollState = false;

    private List<Rigidbody> _rigidbodyComponents = new List<Rigidbody>();

    private Rigidbody topRB = null;

    private ThirdPersonUserControl userControl = null;
    private Animator animator = null;

    void Awake()
    {
        userControl = GetComponentInChildren<ThirdPersonUserControl>();
        Animator animator = GetComponentInChildren<Animator>();

        topRB = GetComponentInParent<Rigidbody>();
    }

    void Start()
    {
        if (_startRagdolled)
            EnableRagdoll();
        else
            DisableRagdoll();
    }

    private Transform TopRagdollPiece()
    {
        if (_pieces.Count == 0)
            return null;

        RagdollBodyPiece p = _pieces[0];
        while (p._parent != null)
            p = p._parent;

        return p._piece;
    }

    private Collider GenerateCollider(RagdollBodyPiece p)
    {
        if (p == null) return null;
        if (!p._piece) return null;

        // Create object to allow correct collider rotation & make it a child of the current piece
        GameObject g = new GameObject("Ragdoll Collider");
        g.hideFlags = HideFlags.HideInInspector;
        g.transform.rotation = Quaternion.Euler(p._rotation);
        g.transform.position = p._piece.position;
        g.transform.parent = p._piece;

        switch (p._shape)
        {
            // TODO: Add offset
            case RagdollBodyPiece.COLLIDERTYPE.Sphere:
                {
                    SphereCollider c = g.AddComponent<SphereCollider>();
                    c.radius = p._radius;
                    c.center = p._offset;
                    return c;
                }

            case RagdollBodyPiece.COLLIDERTYPE.Capsule:
                {
                    CapsuleCollider c = g.AddComponent<CapsuleCollider>();
                    c.radius = p._radius;
                    c.height = p._height + p._radius * 2;
                    c.center = c.transform.worldToLocalMatrix * p._offset;
                    return c;
                }

            default:
                throw new System.Exception("No shape defined for body piece.");
        }
    }

    private void AddPart(RagdollBodyPiece p)
    {
        Transform t = p._piece;

        RagdollBodyPiece parentPiece = null;
        // Find parent piece
        while (t.parent != null)
        {
            // Check if parent has a ragdoll piece
            RagdollBodyPiece piece = _pieces.Find(x => x._piece == t.parent);

            if (piece != null)
            {
                parentPiece = piece;
                break;
            }

            t = t.parent;
        }

        if (parentPiece != null)
        {
            p._parent = parentPiece;

            // Get parent's collider
            Collider parentCollider = parentPiece.collider;
            Rigidbody parentRB = parentPiece._piece.gameObject.GetComponent<Rigidbody>();
            if (!parentRB)
                parentRB = parentPiece._piece.gameObject.AddComponent<Rigidbody>();

            // Track parent's rigidbody
            if (!_rigidbodyComponents.Contains(parentRB))
                _rigidbodyComponents.Add(parentRB);

            // Add a character joint
            CharacterJoint j = p._piece.gameObject.AddComponent<CharacterJoint>();
            j.connectedBody = parentRB;
            p._joint = j;

            // Track rigidbody
            Rigidbody thisRB = p._piece.gameObject.GetComponent<Rigidbody>();
            if (thisRB)
            {
                if (!_rigidbodyComponents.Contains(thisRB))
                    _rigidbodyComponents.Add(thisRB);
            }
        }
    }

    public void EnableRagdoll()
    {
        if (topRB)
            topRB.isKinematic = true;

        if (_currentRagdollState == false)
        {
            if (userControl)
            {
                userControl.Character.Move(Vector3.zero, false, false);
                userControl.enabled = false;
                userControl.Character.enabled = false;
            }

            if (animator)
                animator.enabled = false;

            // Create colliders for each body piece
            for (int i = 0; i < _pieces.Count; i++)
            {
                // Get this piece
                RagdollBodyPiece p = _pieces[i];

                // Check piece is valid
                if (p == null)
                {
                    _pieces.RemoveAt(i);
                    i--;
                    continue;
                }
                if (p._piece == null)
                {
                    _pieces.RemoveAt(i);
                    i--;
                    continue;
                }

                // Generate collider
                Collider c = GenerateCollider(p);

                // Let body piece track it's collider
                p.collider = c;
            }

            // Attach pieces to parent pieces via joints
            foreach (RagdollBodyPiece p in _pieces)
                AddPart(p);

            _currentRagdollState = true;
        }
    }

    public void DisableRagdoll()
    {
        if (topRB)
            topRB.isKinematic = false;

        if (_currentRagdollState == true)
        {
            if (userControl)
            {
                userControl.enabled = true;
                userControl.Character.enabled = true;
            }

            if (animator)
                animator.enabled = true;

            // Destroy all ragdoll colliders and joints
            foreach (RagdollBodyPiece p in _pieces)
            {
                Destroy(p.collider);
                Destroy(p._joint);
            }

            _currentRagdollState = false;
            foreach (Rigidbody rb in _rigidbodyComponents)
                Destroy(rb);
        }

        Transform topPiece = TopRagdollPiece();
        if (topPiece != null)
            transform.position = topPiece.position;
    }

    public void ToggleRagdollState()
    {
        if (_currentRagdollState)
            DisableRagdoll();
        else
            EnableRagdoll();
    }

    public void SetRagdollState(bool state)
    {
        if (state)
            EnableRagdoll();
        else
            DisableRagdoll();
    }

    public void RagdollForSeconds(float seconds)
    {
        StartCoroutine(TimedRagdoll(seconds));
    }

    private IEnumerator TimedRagdoll(float seconds)
    {
        SetRagdollState(true);
        yield return new WaitForSeconds(seconds);
        SetRagdollState(false);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            foreach (RagdollBodyPiece p in _pieces)
            {
                if (p._piece != null)
                {
                    if (p._shape == RagdollBodyPiece.COLLIDERTYPE.Sphere)
                    {
                        // Draw sphere gizmo
                        Gizmos.DrawWireSphere(p._piece.position + p._offset, p._radius);
                    }
                    else if (p._shape == RagdollBodyPiece.COLLIDERTYPE.Capsule)
                    {
                        // Draw capsule gizmo

                        Quaternion rotation = Quaternion.Euler(p._rotation);

                        Vector3 pieceUp = rotation * Vector3.up;
                        Vector3 pieceRight = rotation * Vector3.right;
                        Vector3 pieceForward = rotation * Vector3.forward;

                        Vector3 center = p._piece.position + p._offset;
                        Vector3 topCenter = center + pieceUp * p._height / 2;
                        Vector3 bottomCenter = center + -pieceUp * p._height / 2;

                        // Draw 4 lines
                        Vector3 rightOffset = pieceRight * p._radius;
                        Gizmos.DrawLine(bottomCenter + rightOffset, topCenter + rightOffset);

                        Vector3 leftOffset = -pieceRight * p._radius;
                        Gizmos.DrawLine(bottomCenter + leftOffset, topCenter + leftOffset);

                        Vector3 forwardOffset = pieceForward * p._radius;
                        Gizmos.DrawLine(bottomCenter + forwardOffset, topCenter + forwardOffset);

                        Vector3 backOffset = -pieceForward * p._radius;
                        Gizmos.DrawLine(bottomCenter + backOffset, topCenter + backOffset);

                        // Draw ends
                        Gizmos.DrawWireSphere(topCenter, p._radius);
                        Gizmos.DrawWireSphere(bottomCenter, p._radius);
                    }
                }
            }
        }
    }
}
