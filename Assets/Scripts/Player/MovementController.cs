using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputBuffer
{
    [SerializeField] private float duration;
    
    public float LastInputTime { get; set; } = float.MinValue;

    public bool Flush()
    {
        if (Time.time - LastInputTime > duration)
            return false;

        LastInputTime = float.MinValue;
        return true;
    }
}

public class MovementController : MonoBehaviour
{
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private LayerMask groundCheckMask;
    [SerializeField] private new FirstPersonCamera camera;
    
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private InputBuffer jumpBuffer;
    [SerializeField] private float groundCheckOffset = 0.1f;

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;

    private bool appliedJumpBuffer;

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();

        jump.action.performed += OnJump;
    }
    
    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        
        jump.action.performed -= OnJump;
    }

    private void OnJump(InputAction.CallbackContext _)
    {
        jumpBuffer.LastInputTime = Time.time;
    }

    private bool GroundCheck()
    {
        return Physics.OverlapSphere(
                transform.position - (0.5f * collider.height - collider.radius + groundCheckOffset) * Vector3.up,  
                collider.radius, 
                groundCheckMask)
            .Any();
    }

    private void FixedUpdate()
    {
        Move();
        TryJump();
    }

    private void Move()
    {
        Vector2 directionInput = move.action.ReadValue<Vector2>();
        Vector3 forward = camera.HorizontalDirection;
        Vector3 targetDirection = directionInput.y * forward + directionInput.x * Vector3.Cross(forward, Vector3.down);
        Vector3 targetVelocity = walkSpeed * targetDirection;
        
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
        Vector3 accelerationDirection = targetVelocity - horizontalVelocity;
        float accelerationMagnitude = Mathf.Min(accelerationDirection.magnitude / Time.fixedDeltaTime, acceleration);
        
        rigidbody.AddForce(accelerationMagnitude * accelerationDirection.normalized, ForceMode.Acceleration);
    }

    private void TryJump()
    {
        if (!appliedJumpBuffer && !(GroundCheck() && jumpBuffer.Flush()))
            return;

        rigidbody.velocity += Mathf.Sqrt(2f * Physics.gravity.magnitude * jumpHeight) * Vector3.up;
        appliedJumpBuffer = false;
    }

    private void OnCollisionEnter(Collision _)
    {
        appliedJumpBuffer = GroundCheck() && jumpBuffer.Flush();
    }
}
