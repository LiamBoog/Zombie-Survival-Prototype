using System.Collections;
using TMPro;
using UnityEngine;

public class Lobber : Gun
{
    [SerializeField] private Rigidbody user;
    [SerializeField] private Transform userTransform;
    [SerializeField] private TMP_Text chargeIndicator;
    
    [SerializeField] private float stickyBombVelocity = 20f;
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private int maxCharges = 3;

    private int charges;

    private void OnEnable()
    {
        base.OnEnable();
        charges = maxCharges;
        chargeIndicator.text = charges.ToString();
    }

    protected override bool Cooldown()
    {
        if (charges <= 0)
            return false;

        charges--;
        chargeIndicator.text = charges.ToString();
        StartCoroutine(RefillCharge());
        return true;

        IEnumerator RefillCharge()
        {
            yield return new WaitForSeconds(cooldown);
            charges++;
            chargeIndicator.text = charges.ToString();
        }
    }

    protected override void InitializeProjectile(Projectile stickyBomb)
    {
        stickyBomb.transform.position = projectileSource.position;
        Vector3 userVelocity = Vector3.ProjectOnPlane(user.velocity, userTransform.right);
        stickyBomb.Rigidbody.velocity = userVelocity + stickyBombVelocity * projectileSource.forward;
    }
}
