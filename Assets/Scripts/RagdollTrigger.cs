using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        RagdollPlayer ragdollPlayer = c.GetComponentInChildren<RagdollPlayer>();
        if (ragdollPlayer)
        {
            ragdollPlayer.StartRagdoll();
        }
    }
}
