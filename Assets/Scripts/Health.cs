using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float value = 100.0f;

    [Tooltip("Time in seconds to die for when health reaches 0")]
    [SerializeField, Range(1.0f, 5.0f)]
    private float deathTime = 3.0f;
    private bool dead = false;

    public float health { get { return value; } }

    public delegate void DamageDelegate(Health sender);
    public event DamageDelegate OnHealthValueChanged;

    private void OnParticleCollision(GameObject g)
    {
        if (!dead)
        {
            value -= 15.0f * Time.deltaTime;

            if (value <= 0.0f)
                StartCoroutine(DeathCoroutine());

            if (OnHealthValueChanged != null)
                OnHealthValueChanged.Invoke(this);
        }
    }

    private void Die()
    {
        dead = true;
        value = 0.0f;
        if (OnHealthValueChanged != null)
            OnHealthValueChanged.Invoke(this);

        Ragdoller ragdoller = GetComponent<Ragdoller>();
        if (ragdoller)
            ragdoller.EnableRagdoll();
    }

    private void Resurrect()
    {
        dead = false;
        value = 100.0f;
        if (OnHealthValueChanged != null)
            OnHealthValueChanged.Invoke(this);

        Ragdoller ragdoller = GetComponent<Ragdoller>();
        if (ragdoller)
            ragdoller.DisableRagdoll();
    }

    private IEnumerator DeathCoroutine()
    {
        Die();
        yield return new WaitForSeconds(deathTime);
        Resurrect();
    }
}
