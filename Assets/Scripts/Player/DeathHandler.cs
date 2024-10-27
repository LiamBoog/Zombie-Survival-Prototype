using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Damageable))]
public class DeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private InputActionAsset inputs;

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
        inputs.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        deathUI.SetActive(true);
    }
}
