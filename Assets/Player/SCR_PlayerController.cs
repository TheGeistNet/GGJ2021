using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]

public class SCR_PlayerController : MonoBehaviour
{
    // Bounding box
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

    // Raycast settings
    [SerializeField, Min(2)]
    int horizontalRayCount = 5;
    [SerializeField, Min(2)]
    int verticalRayCount = 5;
    List<Vector2> horizontalRayOrigins = new List<Vector2>();
    List<Vector2> verticalRayOrigins = new List<Vector2>();

    // Physics
    [SerializeField]
    float gravityAcceleration = 9.8f;
    [SerializeField]
    float gravityMaxMagnitude = 20.0f;
    [SerializeField]
    float gravityDirection = -1.0f;
    [SerializeField]
    LayerMask collisionMask;
    bool isGrounded;
    bool isCollidingRight;
    bool isCollidingLeft;
    Vector2 velocity;
    Vector2 calculateMovement;
    float jumpForce = 10.0f;

    // Debug variables
#if UNITY_EDITOR
    public bool debugPhysics;
    List<Vector2> hitPoints = new List<Vector2>();
#endif

    // Use awake to initialize values
    void Awake()
    {
        CalculateBoundingBox();
        CalculateRayOrigins();
        //playerInput.actions.FindAction<"Jump">
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        // Update origin
        origin = boundingCollider.bounds.center;

        // Calculate new gravity
        velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravityDirection * Time.deltaTime), -gravityMaxMagnitude, gravityMaxMagnitude);

        // Handle collisions
        UpdateVerticalCollisions();

        // Move the character
        transform.Translate(calculateMovement);
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

                    newIsGrounded = true;

                    // Update velocity to move us out of any penetration
                    velocity.y = 0.0f;
                    calculateMovement.y = (hit.distance - halfHeight) * gravityDirection;
                    // Update the ray length so we don't look for any hits further than the current hit
                    rayLength = hit.distance;
                }

#if UNITY_EDITOR
                else if (debugPhysics)
                {
                    Debug.DrawRay(rayOrigin + origin, Vector2.up * gravityDirection * rayLength, Color.cyan);
                }
#endif
            }
            isGrounded = newIsGrounded;
            if (!isGrounded)
            {
                calculateMovement.y = velocity.y * Time.deltaTime;
            }
        }

        // Otherwise we are not grounded
        else
        {
            isGrounded = false;
            calculateMovement.y = velocity.y * Time.deltaTime;
        }
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
        if (context.performed && isGrounded)
        {
            velocity += new Vector2(0.0f, jumpForce);
        }
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
