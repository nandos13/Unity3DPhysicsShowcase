using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float value = 100.0f;

    public float health { get { return value; } }

    public delegate void DamageDelegate(Health sender);
    public event DamageDelegate OnDamage;

    private void OnParticleCollision(GameObject g)
    {
        value -= 15.0f * Time.deltaTime;

        if (value <= 0.0f)
            value = 100.0f;

        if (OnDamage != null)
            OnDamage.Invoke(this);
    }
}
