using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoystickMovement : MonoBehaviour
{
    [Header("Joystick Settings")]
    public FloatingJoystick joystick;
    public Transform cameraTransform;
    public CharacterController cC;
    public Animator animator;

    [Header("Surface Check")]
    public Transform surfaceCheck;
    public float surfaceDistance = 0.6f;
    public LayerMask surfaceMask;
    private bool onSurface;

    private Vector3 velocity;
    public float gravity = -9.81f;
    public float jumpForce = 2f;

    [Header("Stats")]
    public PlayerStats playerStats;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private bool jumpRequested = false;
    private bool isJumping = false;

    private bool punchInProgress = false;

    void Update()
    {
        GroundCheck();
        ApplyGravity();
      //  MovePlayer();
        HandleJump();
    }

    void GroundCheck()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);
    }

    // void MovePlayer()
    // {
    //     Vector3 direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;

    //     if (direction.magnitude >= 0.1f)
    //     {
    //         float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
    //         float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
    //         transform.rotation = Quaternion.Euler(0f, angle, 0f);

    //         Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

    //         float moveSpeed = playerStats.isSprinting > 0f
    //             ? playerStats.sprintSpeed
    //             : playerStats.walkSpeed;

    //         // Move only if not punching
    //         if (!punchInProgress)
    //             cC.Move(moveDir.normalized * moveSpeed * Time.deltaTime);

    //         animator.SetBool("walk", !playerStats.isSprinting && !punchInProgress);
    //         animator.SetBool("running", playerStats.isSprinting && !punchInProgress);
    //     }
    //     else
    //     {
    //         animator.SetBool("walk", false);
    //         animator.SetBool("running", false);
    //     }
    // }

    public void JumpRequest()
    {
        jumpRequested = true;
    }

    public void HandleJump()
    {
        if (onSurface && jumpRequested && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        jumpRequested = false;
    }

    public void PunchRequest()
    {
        if (!punchInProgress)
        {
            StartCoroutine(PunchCoroutine());
        }
    }

    public float punchRange = 2f;
    public LayerMask zombieLayer;

    private IEnumerator PunchCoroutine()
    {
        punchInProgress = true;

        animator.SetTrigger("punch");

        // Get punch animation duration
        float punchDuration = 0.5f; // fallback
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name.ToLower().Contains("punch"))
            {
                punchDuration = clip.length;
                break;
            }
        }

        // Wait until mid-punch for impact
        yield return new WaitForSeconds(punchDuration * 0.4f); // adjust if needed

        // Apply damage
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, punchRange, zombieLayer);
        foreach (var hit in hits)
        {
            // ZombieHealth zombie = hit.GetComponent<ZombieHealth>();
            // if (zombie != null)
            // {
            //     zombie.TakeDamage(1);
            //     zombie.ShowHealthBar(true);
            // }
        }

        // Wait for rest of the animation
        yield return new WaitForSeconds(punchDuration * 0.6f);

        punchInProgress = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, punchRange);
    }
}
