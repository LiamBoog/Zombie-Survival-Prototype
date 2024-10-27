using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Damageable))]
public class DeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private InputActionReference aim;
    [SerializeField] private InputActionReference primaryFire;
    [SerializeField] private InputActionReference secondaryFire;

    private void OnEnable()
    {
        GetComponent<Damageable>().Died += OnDeath;
    }

    private void OnDisable()
    {
        GetComponent<Damageable>().Died -= OnDeath;
    }

    private void OnDeath()
    {
        aim.action.Disable();
        primaryFire.action.Disable();
        secondaryFire.action.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        deathUI.SetActive(true);
    }
}
