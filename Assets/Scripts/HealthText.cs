using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    private TMPro.TMP_Text text = null;

    private void Awake()
    {
        text = GetComponent<TMPro.TMP_Text>();

        Health health = FindObjectOfType<Health>();
        if (health)
            health.OnDamage += OnDamage;
    }

    private void OnDamage(Health sender)
    {
        if (text)
        {
            text.text = string.Format("Health: {0}", sender.health.ToString("F2"));
            text.rectTransform.anchoredPosition = Vector3.zero;
        }
    }
}
