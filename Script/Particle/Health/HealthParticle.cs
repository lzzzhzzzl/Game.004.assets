using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthParticle : MonoBehaviour
{
    public ParticleSystemRenderer particleSystemRenderer;
    public TMP_Text text;
    private void Awake()
    {
        particleSystemRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
        text = gameObject.GetComponent<TMP_Text>();

        particleSystemRenderer.mesh = text.mesh;
    }
    public void ChangeHealthText(float healthDamage)
    {
        text.text = healthDamage.ToString();
    }

}
