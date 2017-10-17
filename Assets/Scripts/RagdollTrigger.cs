using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        Ragdoller ragdoller = c.GetComponentInChildren<Ragdoller>();
        if (ragdoller)
            ragdoller.RagdollForSeconds(3.0f);
    }
}
