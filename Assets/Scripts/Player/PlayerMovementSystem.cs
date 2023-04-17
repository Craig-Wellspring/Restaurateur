
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSystem : MonoBehaviour
{
    [Tooltip("Maximum slope the character can jump on")]
    [Range(5f, 60f)]
    public float slopeLimit = 45f;

    [Tooltip("Move speed in meters/second")]
    public float moveSpeed = 6f;

    [Tooltip("Turn speed in degrees/second, left (+) or right (-)")]     
    public float turnSpeed = 450;

    [Tooltip("Upward speed to apply when jumping in meters/second")]
    public float jumpForce = 6f;

    [Tooltip("Camera rotation speed multiplier")]
    public float camRotationSpeed = 2f;

    [Tooltip("Camera zoom speed multiplier")]
    public float camZoomSpeed = 0.8f;

    [Range(6, 12)]
    public float camMaxZoom = 11f;
    
    [Range(1, 6)]
    public float camMinZoom = 3f;


    private bool isJumping;
    public bool isGrounded { get; private set; }
    public bool canMove { get; private set; } = true;
    private PlayerManager player;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    // Rotate character toward movement vector
    private void Update()
    {        
        if (canMove && !Mathf.Approximately(player.kb.movementInput.magnitude, 0f)){
            Quaternion targetRotation = Quaternion.LookRotation(player.kb.movementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    // Apply physics on physics frames
    private void FixedUpdate()
    {
        CheckGrounded();
        ProcessCamera();
        ProcessMovement();
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        float capsuleHeight = Mathf.Max(player.bodyCollider.radius * 2f, player.bodyCollider.height);
        Vector3 capsuleBottom = transform.TransformPoint(player.bodyCollider.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(player.bodyCollider.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < slopeLimit)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    isGrounded = true;
            }
        }
    }

    public void SetCanMove(bool _can) {
        canMove = _can;
    }

    public void Jump() {
        isJumping = true;
    }


    // Character movement
    private void ProcessMovement()
    {
        // Reset the velocity
        player.rb.velocity = new Vector3(0, player.rb.velocity.y, 0);

        if (canMove) {
            // Add Movement/Jumping velocity while on the ground
            if (isGrounded)
            {
                // Apply a velocity based on player input
                player.rb.velocity += player.kb.movementInput;

                // Apply an upward velocity to jump
                if (isJumping) {
                    player.rb.velocity += Vector3.up * jumpForce;
                    isJumping = false;
                }
            }
            // Add movement velocity while jumping/falling
            else
            {
                if (!Mathf.Approximately(player.kb.movementInput.magnitude, 0f))
                {
                    // Apply a velocity based on player input at half speed
                    Vector3 verticalVelocity = Vector3.Project(player.rb.velocity, Vector3.up);
                    player.rb.velocity += player.kb.movementInput / 2f;
                }
            }
        }
    }

    // Camera movement
    private void ProcessCamera() {
        player.vCamTransposer.m_XAxis.Value += player.kb.cameraRotationInput * camRotationSpeed;
    }

    public void ZoomCamera(float _input) {
        player.vCam.m_Lens.OrthographicSize -= _input * camZoomSpeed;
        player.vCam.m_Lens.OrthographicSize = Mathf.Clamp(player.vCam.m_Lens.OrthographicSize, camMinZoom, camMaxZoom);
    }
}
