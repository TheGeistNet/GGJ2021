using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]

public class SCR_PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField]
    LayerMask collisionMask;

    [Header("Bounding Box")]
    [SerializeField]
    BoxCollider2D boundingCollider;
    [SerializeField]
    const float skinWidth = 0.03f;
    Bounds boundingBox;
    Vector2 bottomRightLocal;
    Vector2 bottomLeftLocal;
    Vector2 topRightLocal;
    Vector2 topLeftLocal;
    Vector2 origin;
    float halfWidth;
    float halfHeight;

    [Header("Raycasting")]
    [SerializeField, Min(2)]
    int horizontalRayCount = 5;
    [SerializeField, Min(2)]
    int verticalRayCount = 5;
    List<Vector2> horizontalRayOrigins = new List<Vector2>();
    List<Vector2> verticalRayOrigins = new List<Vector2>();

    [Header("Gravity")]
    [SerializeField]
    float gravityMaxMagnitude = 20.0f;
    [SerializeField]
    float gravityDirection = -1.0f;
    [SerializeField, Min(1.0f)]
    float gravityFallingMultiplier = 2.0f;
    float gravityAccelerationDefault;
    float gravityAcceleration;
    bool isGrounded;
    bool isCollidingRight;
    bool isCollidingLeft;
    Vector2 velocity;
    Vector2 calculateMovement;

    [Header("Controls")]
    [Header("Jumping")]
    [SerializeField]
    float jumpTimeToApex = 0.6f;
    [SerializeField, Min(0.1f)]
    float jumpHeightMax = 3.0f;
    [SerializeField, Min (0.1f)]
    float jumpHeightMin = 1.0f;
    [SerializeField, Min(0.0f)]
    float jumpBufferTime = 0.1f;
    float jumpVelocityMax;
    float jumpVelocityMin;
    bool jumpQueued;
    float jumpQueueTime;

    // Debug variables
#if UNITY_EDITOR
    public bool debugPhysics;
    List<Vector2> hitPoints = new List<Vector2>();
#endif

    // Use awake to initialize values
    void Awake()
    {
        CalculateGravity();
        CalculateJumpVelocities();
        CalculateBoundingBox();
        CalculateRayOrigins();

        return;
    }

    private void FixedUpdate()
    {
        UpdateMovement();

        return;
    }

    void UpdateMovement()
    {
        // Update origin
        origin = boundingCollider.bounds.center;

        // Calculate new gravity
        ApplyGravity();

        // Handle collisions
        UpdateVerticalCollisions();

        // Move the character
        transform.Translate(calculateMovement);

        return;
    }

    void ApplyGravity()
    {
        // If not falling, apply normal gravity
        if (velocity.y >= 0.0f)
        {
            velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravityDirection * Time.deltaTime), -gravityMaxMagnitude, gravityMaxMagnitude);
        }

        // Otherwise, apply falling gravity
        else
        {
            velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravityDirection * Time.deltaTime * gravityFallingMultiplier), -gravityMaxMagnitude, gravityMaxMagnitude);
        }
    }

    void UpdateVerticalCollisions()
    {
        // Cache the proposed vertical movement
        float proposedVerticalMovement = velocity.y * Time.deltaTime;

        // If we are moving in the direction of gravity
        if (Mathf.Sign(proposedVerticalMovement) == Mathf.Sign(gravityDirection))
        {
            // Check if we are grounded
            bool newIsGrounded = false;
            RaycastHit2D hit;
            float rayLength = halfHeight + Mathf.Abs(proposedVerticalMovement);
            float groundDistance = 0.0f;

            // Raycast in the direction of gravity
            foreach (Vector2 rayOrigin in verticalRayOrigins)
            {
                hit = Physics2D.Raycast(rayOrigin + origin, Vector2.up * gravityDirection, rayLength, collisionMask);

                // If raycast hit a target
                if (hit.collider)
                {

#if UNITY_EDITOR
                    if (debugPhysics)
                    {
                        Debug.DrawLine(rayOrigin + origin, hit.point, Color.yellow);
                        hitPoints.Add(hit.point);
                    }
#endif
                    // Update new is grounded
                    newIsGrounded = true;

                    // Update the ray length so we don't look for any hits further than the current hit
                    groundDistance = hit.distance;
                    rayLength = hit.distance;
                }

#if UNITY_EDITOR
                else if (debugPhysics)
                {
                    Debug.DrawRay(rayOrigin + origin, Vector2.up * gravityDirection * rayLength, Color.cyan);
                }
#endif
            }

            // If are grounded
            if (newIsGrounded)
            {
                // Update velocity to move us out of any penetration
                velocity.y = 0.0f;
                calculateMovement.y = (groundDistance - halfHeight) * gravityDirection;

                // If we just became grounded
                if (!isGrounded)
                {
                    // If a jump is queued in the buffer recently
                    if (jumpQueued)
                    {
                        // If the jump was queued recently
                        if (Time.time - jumpQueueTime <= jumpBufferTime)
                        {
                            Jump();
                        }
                        jumpQueued = false;
                    }
                }
            }

            // If we are not grounded, then apply the normal velocity
            else
            {
                calculateMovement.y = velocity.y * Time.deltaTime;
            }

            // Update state
            isGrounded = newIsGrounded;
        }

        // Otherwise we are not grounded
        // Apply normal velocity
        else
        {
            isGrounded = false;
            calculateMovement.y = velocity.y * Time.deltaTime;
        }

        return;
    }

    void CalculateGravity()
    {
        gravityAccelerationDefault = (2.0f * jumpHeightMax) / Mathf.Pow(jumpTimeToApex, 2.0f);
        gravityAcceleration = gravityAccelerationDefault;
        return;
    }

    void CalculateJumpVelocities()
    {
        jumpVelocityMin = Mathf.Sqrt(2.0f * gravityAccelerationDefault * jumpHeightMin);
        jumpVelocityMax = Mathf.Max(jumpVelocityMin, gravityAccelerationDefault * jumpTimeToApex);
        return;
    }

    void CalculateBoundingBox()
    {
        // Setup collision
        boundingBox = boundingCollider.bounds;
        halfWidth = boundingBox.size.x * 0.5f;
        halfHeight = boundingBox.size.y * 0.5f;
        bottomRightLocal = new Vector2(halfWidth, -halfHeight);
        bottomLeftLocal = new Vector2(-halfWidth, -halfHeight);
        topRightLocal = new Vector2(halfWidth, halfHeight);
        topLeftLocal = new Vector2(-halfWidth, halfHeight);

        return;
    }

    void CalculateRayOrigins()
    {
        float horizontalRaySpacing = boundingBox.size.y / (float)(horizontalRayCount - 1);
        float verticalRaySpacing = boundingBox.size.x / (float)(verticalRayCount - 1);

        // Calculate horizontal ray origins
        horizontalRayOrigins.Clear();

        for (int idx = 0; idx < horizontalRayCount; idx++)
        {
            horizontalRayOrigins.Add(new Vector2(0.0f, ((float)idx * horizontalRaySpacing) - halfHeight));
        }

        // Calculate vertical ray origins
        verticalRayOrigins.Clear();
        for (int idx = 0; idx < verticalRayCount; idx++)
        {
            verticalRayOrigins.Add(new Vector2(((float)idx * verticalRaySpacing) - halfWidth, 0.0f));
        }

        return;
    }

    // Controls
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If grounded, jump
            if (isGrounded)
            {
                Jump();
            }

            // Otherwise, queue a jump
            else
            {
                jumpQueued = true;
                jumpQueueTime = Time.time;
            }
        }
        else if (context.canceled && velocity.y > jumpVelocityMin)
        {
            velocity.y = jumpVelocityMin * -1.0f * gravityDirection;
        }
        return;
    }

    void Jump()
    {
        velocity.y = jumpVelocityMax * -1.0f * gravityDirection;
        return;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debugPhysics)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector2 hitPoint in hitPoints)
            {
                Gizmos.DrawWireSphere(hitPoint, 0.2f);
            }
            hitPoints.Clear();
        }
        return;
    }
#endif
}
