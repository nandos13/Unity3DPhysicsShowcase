using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class RagdollTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        StartCoroutine(RagdollForSeconds(3.0f, c.transform));
    }

    private IEnumerator RagdollForSeconds(float seconds, Transform t)
    {
        SetRagdoll(t, true);
        yield return new WaitForSeconds(seconds);
        SetRagdoll(t, false);
    }

    private void SetRagdoll(Transform t, bool state)
    {
        ThirdPersonUserControl userControl = t.GetComponentInChildren<ThirdPersonUserControl>();
        if (userControl)
        {
            if (state)
                userControl.Character.Move(Vector3.zero, false, false);
            userControl.enabled = !state;
            userControl.Character.enabled = !state;
        }

        Animator animator = t.GetComponentInChildren<Animator>();
        if (animator)
            animator.enabled = !state;

        Ragdoller ragdoller = t.GetComponentInChildren<Ragdoller>();
        if (ragdoller)
        {
            if (state)
                ragdoller.EnableRagdoll();
            else
                ragdoller.DisableRagdoll();
        }
    }
}
