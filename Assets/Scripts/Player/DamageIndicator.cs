using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Damageable))]
public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Volume postProcessing;
    [SerializeField] private Gradient damageGradient;
    [SerializeField] private AnimationCurve intensityCurve;

    private Vignette vignette;

    private void OnEnable()
    {
        if (!postProcessing.profile.TryGet(out vignette))
        {
            vignette = postProcessing.profile.Add<Vignette>();
        }
        GetComponent<Damageable>().HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        GetComponent<Damageable>().HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float healthPercent)
    {
        float damage = 1f - healthPercent;
        vignette.color.Override(damageGradient.Evaluate(damage));
        vignette.intensity.Override(intensityCurve.Evaluate(damage));
    }
}
