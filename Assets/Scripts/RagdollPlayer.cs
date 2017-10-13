using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Ragdoller))]
[RequireComponent(typeof(ThirdPersonUserControl))]
public class RagdollPlayer : MonoBehaviour
{
    private Ragdoller ragdoller = null;
    private ThirdPersonUserControl control = null;
    private ThirdPersonCharacter character = null;

    private void Awake()
    {
        ragdoller = GetComponent<Ragdoller>();
        control = GetComponent<ThirdPersonUserControl>();
        character = GetComponentInChildren<ThirdPersonCharacter>();
    }

    public void RagdollForSeconds(float duration)
    {
        if (duration <= 0) return;

        throw new System.NotImplementedException("Have not done this function yet");
    }

    public void EndRagdoll()
    {
        ragdoller.DisableRagdoll();
        control.enabled = true;
        character.enabled = true;
    }

    public void StartRagdoll()
    {
        ragdoller.EnableRagdoll();
        control.enabled = false;
        character.enabled = false;
    }
}
