using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float acceleration = 10f;

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
    }

    private void FixedUpdate()
    {
        Vector2 directionInput = move.action.ReadValue<Vector2>();
        Vector3 targetDirection = transform.TransformDirection(new Vector3(directionInput.y, 0f, -directionInput.x));
        Vector3 targetVelocity = walkSpeed * targetDirection;
        
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
        Vector3 accelerationDirection = targetVelocity - horizontalVelocity;
        float accelerationMagnitude = Mathf.Min(accelerationDirection.magnitude / Time.fixedDeltaTime, acceleration);
        
        rigidbody.AddForce(accelerationMagnitude * accelerationDirection.normalized, ForceMode.Acceleration);
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
    }
}
