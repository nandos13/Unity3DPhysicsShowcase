using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollButtonInteractor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Ragdoller ragdoller;

    void IInteractable.Interact()
    {
        if (ragdoller)
            ragdoller.EnableRagdoll();
    }
}
