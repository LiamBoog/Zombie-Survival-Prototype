using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float aimSensitivity = 75f;
    [SerializeField] private ExpMovingAverageVector3 position = new(0.01f);

    [SerializeField] private InputActionReference aim;

    public Vector3 HorizontalDirection => Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

    private void OnEnable()
    {
        Cursor.visible = false;
        aim.action.Enable();
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        aim.action.Disable();
    }

    private void LateUpdate()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        position.AddSample(target.position + offset, Time.deltaTime);
        transform.position = position;
    }

    private void Rotate()
    {
        Vector3 lookVector = transform.forward;
        float verticalAngle = Vector3.Angle(Vector3.up, lookVector);
        
        Vector2 aimDelta = aim.action.ReadValue<Vector2>();
        Vector3 eulerAngles = aimSensitivity * Time.deltaTime * new Vector3(
            Mathf.Clamp(-aimDelta.y, 0f - verticalAngle, 180f - verticalAngle), // clamp vertical angle to the forward direction
            aimDelta.x
        );
        
        // Rotate about global y-axis and local x-axis
        transform.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f) * transform.rotation * Quaternion.Euler(eulerAngles.x, 0f, 0f);
    }
}
