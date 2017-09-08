using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour, IInteractable
{

    [Range(10, 1000), SerializeField]    private float _force = 500;
    [Range(1, 100), SerializeField]    private float _radius = 10;

    private void Explode()
    {
        Debug.Log("Exploding", this);

        // Get all colliders in radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        // Get all rigidbody components within this radius
        foreach (Collider c in colliders)
        {
            Rigidbody rb = c.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(_force, transform.position, _radius);
        }
    }

    public void Interact()
    {
        Explode();
    }

    void OnTriggerEnter()
    {
        Explode();
    }
}
