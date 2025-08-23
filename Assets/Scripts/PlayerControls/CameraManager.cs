using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;  
    public Transform playerTransform;
    public Transform cameraPivot;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    [Header("Camera Movement and Rotation")]
    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 0.1f;
    public float cameraPivotSpeed = 0.1f;
    public float lookAngle;
    public float pivotAngle;
    public float minimumPivot = -30f;
    public float maximumPivot = -30f;

    void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        playerTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }
    void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraFollowVelocity, cameraFollowSpeed);
    }
    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        // restrict camera angle
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

    }

}
