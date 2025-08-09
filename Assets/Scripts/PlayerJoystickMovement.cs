using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoystickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float sprintSpeed = 4f;

    [Header("References")]
    public CharacterController controller;
    public Transform cam;
    public Animator animator;
    public LayerMask surfaceMask;
    [Header("Joystick Input")]
    public Joystick joystick; // Reference to your on-screen joystick
    public bool isSprinting = false;

    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Animation
            animator.SetBool("idle", false);
            animator.SetBool("walk", !isSprinting);
            animator.SetBool("running", isSprinting);
        }
        else
        {
            animator.SetBool("idle", true);
            animator.SetBool("walk", false);
            animator.SetBool("running", false);
        }
    }

    // These methods can be hooked to Sprint button UI events
    public void StartSprinting()
    {
        isSprinting = true;
    }

    public void StopSprinting()
    {
        isSprinting = false;
    }
}
