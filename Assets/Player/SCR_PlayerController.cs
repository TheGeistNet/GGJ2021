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
    const float skinWidth = 0.015f;
    Bounds boundingBox;
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
    bool startWithInversedGravity;
    float gravitySign = -1.0f;
    float gravityAccelerationDefault;
    float gravityAcceleration;
    bool isGrounded;
    bool isCollidingRight;
    bool isCollidingLeft;
    Vector2 velocity;

    [Header("Walking")]
    [SerializeField]
    float walkMaxSpeed = 6.0f;
    [SerializeField]
    float walkAccelerationTime = 0.3f;
    [SerializeField]
    float walkDecelerationTime = 0.1f;
    [SerializeField]
    float walkAccelerationAirborneMultiplier = 0.5f;
    [SerializeField]
    float walkDeceleratioAirbornenMultiplier = 0.5f;
    [SerializeField]
    AnimationCurve walkAccelerationCurve;
    [SerializeField]
    AnimationCurve walkDecelerationCurve;
    bool isWalking;
    float walkDirection;
    float lastWalkDirection;
    float walkAccelerationProgress;
    float walkDecelerationProgress = 1.0f;

    [Header("Jumping")]
    [SerializeField]
    float jumpTimeToApex = 0.6f;
    [SerializeField, Min(0.1f)]
    float jumpHeightMax = 3.0f;
    [SerializeField, Min (0.1f)]
    float jumpHeightMin = 1.0f;
    [SerializeField, Min(0.0f)]
    float jumpLandingBufferTime = 0.1f;
    [SerializeField]
    float jumpWalkOffLedgeBufferTime = 0.1f;
    [SerializeField, Min(1.0f)]
    float jumpDescentGravityMultiplier = 2.0f;
    float jumpVelocityMax;
    float jumpVelocityMin;
    bool jumpLandingQueued;
    float jumpLandingQueuedTime;
    float jumpWalkedOffLedgeTimeStamp;
    bool jumpedRecently;

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

        // Apply walking input
        ApplyWalking();

        // Calculate new gravity
        ApplyGravity();

        // Calculate movement
        UpdateHorizontalMovement();
        UpdateVerticalMovement();

        return;
    }

    void ApplyWalking()
    {
        // If walking
        if (isWalking)
        {
            // Cache the direction
            float walkDirection = Mathf.Sign(this.walkDirection);

            // If we changed direction, set accel progress to 0
            if (lastWalkDirection != walkDirection)
            {
                walkAccelerationProgress = 0.0f;
            }

            // Cap the max accel progresss based on the analog stick
            float maxAcelerationProgress = Mathf.Abs(this.walkDirection);
            if (walkAccelerationProgress < maxAcelerationProgress)
            {
                // If grounded, apply normal acceleration
                if (isGrounded)
                {
                    walkAccelerationProgress = Mathf.Min(walkAccelerationProgress + (Time.deltaTime / walkAccelerationTime), maxAcelerationProgress);
                }

                // Otherwise, apply airborne acceleratiob
                else
                {
                    walkAccelerationProgress = Mathf.Min(walkAccelerationProgress + ((Time.deltaTime / walkAccelerationTime) * walkAccelerationAirborneMultiplier), maxAcelerationProgress);
                }
            }

            //Set
            velocity.x = walkDirection * (Mathf.Max(walkAccelerationCurve.Evaluate(walkAccelerationProgress) * walkMaxSpeed, Mathf.Abs(velocity.x)));
        }

        // If not walking
        else
        {
            // If still moving
            if (Mathf.Abs(velocity.x) > 0.0f)
            {
                // If not finished decelerating
                if (walkDecelerationProgress < 1.0f)
                {
                    // If grounded, apply normal deceleration
                    if (isGrounded)
                    {
                        walkDecelerationProgress = Mathf.Min(walkDecelerationProgress + (Time.deltaTime / walkDecelerationTime), 1.0f);
                    }

                    // Otherwise, apply airborne deceleration
                    else
                    {
                        walkDecelerationProgress = Mathf.Min(walkDecelerationProgress + ((Time.deltaTime / walkDecelerationTime) * walkDeceleratioAirbornenMultiplier), 1.0f);
                    }
                }
                velocity.x = Mathf.Sign(velocity.x) * (Mathf.Min((1.0f - walkDecelerationCurve.Evaluate(walkDecelerationProgress)) * walkMaxSpeed, Mathf.Abs(velocity.x)));
            }
        }

        // Cache the last walk axis
        lastWalkDirection = walkDirection;
    }

    void ApplyGravity()
    {
        // If descending from a jump, apply descent gravity
        if (Mathf.Sign(velocity.y) == gravitySign && jumpedRecently)
        {
            velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravitySign * Time.deltaTime * jumpDescentGravityMultiplier), -gravityMaxMagnitude, gravityMaxMagnitude);
        }

        // Otherwise, apply normal gravity
        else
        {
            velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravitySign * Time.deltaTime), -gravityMaxMagnitude, gravityMaxMagnitude);
        }

        return;
    }

    void UpdateHorizontalMovement()
    {
        Vector2 calculatedMovement = new Vector2();

        // Cache the proposed horizontal movement
        float proposedHorizontalMovement = velocity.x * Time.deltaTime;

        // Trace to check if we will collide wit a wallk
        float hitDistance = 0.0f;
        float traceSign = Mathf.Sign(velocity.x);
        bool didCollideWithObject = TraceForCollisions(halfWidth + Mathf.Abs(proposedHorizontalMovement), new Vector2(traceSign, 0.0f), origin, ref horizontalRayOrigins, ref hitDistance);

        // If we collided with an object
        if (didCollideWithObject)
        {
            // Update velocity and move us out of any penetration
            velocity.x = 0.0f;
            calculatedMovement.x = (hitDistance - halfWidth) * traceSign;
        }
        else
        {
            // Apply normal velocity
            calculatedMovement.x = proposedHorizontalMovement;
        }

        // Apply the movement
        origin += calculatedMovement;
        transform.Translate(calculatedMovement);
    }

    void UpdateVerticalMovement()
    {
        Vector2 calculatedMovement = new Vector2();

        // Cache the proposed vertical movement
        float proposedVerticalMovement = velocity.y * Time.deltaTime;

        // Trace to check if we are grounded
        float hitDistance = 0.0f;
        float traceSign = Mathf.Sign(velocity.y);
        bool didCollideWithObject = TraceForCollisions(halfHeight + Mathf.Abs(proposedVerticalMovement), new Vector2(0.0f, traceSign), origin, ref verticalRayOrigins, ref hitDistance);

        // If we collided with an object
        if (didCollideWithObject)
        {
            // Update velocity and move us out of any penetration
            velocity.y = 0.0f;
            calculatedMovement.y = (hitDistance - halfHeight) * traceSign;
        }
        else
        {
            // Apply normal velocity
            calculatedMovement.y = proposedVerticalMovement;
        }

        // If we were moving in the direction of gravity
        if (Mathf.Sign(proposedVerticalMovement) == gravitySign)
        {
            // If we are grounded
            if (didCollideWithObject)
            {
                // If we just became grounded
                if (!isGrounded)
                {
                    jumpedRecently = false;

                    // If a jump is queued in the buffer recently
                    if (jumpLandingQueued)
                    {
                        // If the jump was queued recently
                        if (Time.time - jumpLandingQueuedTime <= jumpLandingBufferTime)
                        {
                            Jump();
                        }
                        jumpLandingQueued = false;
                    }
                }

                // Update state
                isGrounded = true;
            }

            // Otherwise, if we are not grounded
            else
            {
                // If we just stopped being grounded
                if (isGrounded)
                {
                    // We know it wasn't a jump because our velocity is < 0.0f
                    // Cache the walk off ledge time
                    jumpWalkedOffLedgeTimeStamp = Time.time;
                }

                // Update state
                isGrounded = false;
            }
        }

        // Otherwise, if we were moving away from gravity
        else
        {
            // If we were on the ground and moved away
            if (isGrounded && Mathf.Sign(calculatedMovement.y) != gravitySign)
            {
                isGrounded = false;
            }
        }

        // Apply the movement
        origin += calculatedMovement;
        transform.Translate(calculatedMovement);

        return;
    }

    bool TraceForCollisions(float traceDistance, Vector2 traceDirection, Vector2 traceOrigin, ref List<Vector2> rayOrigins, ref float outHitDistance)
    {
        // Initialize variables
        bool didTraceHitObject = false;
        RaycastHit2D hit;
        float rayLength = traceDistance;

        // Raycast in the direction of gravity
        foreach (Vector2 rayOrigin in rayOrigins)
        {
            hit = Physics2D.Raycast(rayOrigin + traceOrigin, traceDirection, rayLength, collisionMask);

            // If raycast hit a target
            if (hit.collider)
            {
                // Update new is grounded
                didTraceHitObject = true;

                // Update the ray length so we don't look for any hits further than the current hit
                rayLength = hit.distance;

#if UNITY_EDITOR
                if (debugPhysics)
                {
                    Debug.DrawLine(rayOrigin + origin, hit.point, Color.yellow);
                    hitPoints.Add(hit.point);
                }
#endif
            }

#if UNITY_EDITOR
            else if (debugPhysics)
            {
                Debug.DrawRay(rayOrigin + origin, traceDirection * rayLength, Color.cyan);
            }
#endif
        }

        outHitDistance = rayLength;
        return didTraceHitObject;
    }

    void CalculateGravity()
    {
        if (startWithInversedGravity)
        {
            gravitySign = 1.0f;
        }
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

        return;
    }

    void CalculateRayOrigins()
    {
        float horizontalRayCoverage = halfHeight - skinWidth;
        float horizontalRaySpacing = (horizontalRayCoverage * 2.0f) / (float)(horizontalRayCount - 1);
        float verticalRayCoverage = halfWidth - skinWidth;
        float verticalRaySpacing = (verticalRayCoverage * 2.0f) / (float)(verticalRayCount - 1);

        // Calculate horizontal ray origins
        horizontalRayOrigins.Clear();

        for (int idx = 0; idx < horizontalRayCount; idx++)
        {
            horizontalRayOrigins.Add(new Vector2(0.0f, ((float)idx * horizontalRaySpacing) - horizontalRayCoverage));
        }

        // Calculate vertical ray origins
        verticalRayOrigins.Clear();
        for (int idx = 0; idx < verticalRayCount; idx++)
        {
            verticalRayOrigins.Add(new Vector2(((float)idx * verticalRaySpacing) - verticalRayCoverage, 0.0f));
        }

        return;
    }

    // Jump function
    void Jump()
    {
        velocity.y = jumpVelocityMax * -1.0f * gravitySign;
        jumpedRecently = true;
        return;
    }

    // Walking
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isWalking = true;
            walkDirection = context.ReadValue<float>();

            // Get the walk deceleration scalar based on the walk acceleration progress
            float walkDecelScalar = walkDecelerationCurve.Evaluate(walkDecelerationProgress);

            // Lookup the opposite of the scalar on acceleration curve
            walkDecelScalar = 1.0f - walkDecelScalar;
            walkAccelerationProgress = GetCurveTimeForValue(walkAccelerationCurve, walkDecelScalar, 10);

            walkDecelerationProgress = 0.0f;
        }
        else if (context.canceled)
        {
            isWalking = false;
            // Get the walk deceleration scalar based on the walk deceleration progress;
            float walkAccelScalar = walkAccelerationCurve.Evaluate(walkAccelerationProgress);

            // Lookup the opposite of the scalar on deceleration
            walkAccelScalar = 1.0f - walkAccelScalar;
            walkDecelerationProgress = GetCurveTimeForValue(walkDecelerationCurve, walkAccelScalar, 10);

            walkAccelerationProgress = 0.0f;
        }
        else
        {
            walkDirection = context.ReadValue<float>();
        }
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

            // Otherwise, check if we just walked off the ledge
            else if (!jumpedRecently && Time.time - jumpWalkedOffLedgeTimeStamp <= jumpWalkOffLedgeBufferTime)
            {
                Jump();
            }

            // Otherwise, queue a jump in case we land soon
            else
            {
                jumpLandingQueued = true;
                jumpLandingQueuedTime = Time.time;
            }
        }

        // Set jump velocity
        else if (context.canceled && Mathf.Abs(velocity.y) > jumpVelocityMin && Mathf.Sign(velocity.y) != gravitySign)
        {
            velocity.y = jumpVelocityMin * -1.0f * gravitySign;
        }
        return;
    }

    float GetCurveTimeForValue(AnimationCurve curveToCheck, float value, int accuracy)
    {

        float startTime = curveToCheck.keys[0].time;
        float endTime = curveToCheck.keys[curveToCheck.length - 1].time;
        float nearestTime = startTime;
        float step = endTime - startTime;

        for (int i = 0; i < accuracy; i++)
        {

            float valueAtNearestTime = curveToCheck.Evaluate(nearestTime);
            float distanceToValueAtNearestTime = Mathf.Abs(value - valueAtNearestTime);

            float timeToCompare = nearestTime + step;
            float valueAtTimeToCompare = curveToCheck.Evaluate(timeToCompare);
            float distanceToValueAtTimeToCompare = Mathf.Abs(value - valueAtTimeToCompare);

            if (distanceToValueAtTimeToCompare < distanceToValueAtNearestTime)
            {
                nearestTime = timeToCompare;
                valueAtNearestTime = valueAtTimeToCompare;
            }
            step = Mathf.Abs(step * 0.5f) * Mathf.Sign(value - valueAtNearestTime);
        }

        return nearestTime;
    }

    public void InvertGravity()
    {
        gravitySign *= -1.0f;
    }

    public void SetGravityDown()
    {
        gravitySign = -1.0f;
    }

    public void SetGravityUp()
    {
        gravitySign = 1.0f;
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
