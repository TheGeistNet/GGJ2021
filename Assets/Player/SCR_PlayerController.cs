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
    bool isCollidingBelow;
    bool isCollidingAbove;
    bool isCollidingRight;
    bool isCollidingLeft;
    Vector2 velocity;

    [Header("Bounding Box")]
    [SerializeField]
    BoxCollider2D boundingCollider;
    [SerializeField]
    const float skinWidth = 0.015f;
    Bounds boundingBox;
    Vector2 origin;
    float halfWidth;
    float halfHeight;

    [Header("Animation")]
    [SerializeField]
    Animator animator;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    int velocityXAnimationHash;
    int onJumpAnimationHash;
    int onLandAnimationHash;
    int isAirborneAnimationHash;
    int isMovingAnimationHash;

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
    bool isWalking = false;
    float walkAxis;
    float walkDirection = 1.0f;
    public float walkAccelerationProgress = 0.0f;
    public float walkDecelerationProgress = 1.0f;

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
    float jumpLandingQueuedTimeStamp;
    float jumpWalkedOffLedgeTimeStamp;
    bool isJumping;

    [Header("Wall Sliding")]
    [SerializeField, Min(0.0f)]
    float wallSlideMaxSpeed = 2.5f;

    [Header("Wall Jumping")]
    [SerializeField, Min(0.0f)]
    float wallJumpBufferTime = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)]
    float wallJumpStartAccelerationScalar = 0.75f;
    [SerializeField]
    float wallJumpAccelerationMultiplier = 2.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    float wallJumpHeightScalar = 0.75f;
    [SerializeField, Min(0.0f)]
    float wallJumpMinPushOffTime = 0.5f;
    float wallJumpStartTimeStamp;
    bool isWallJumping;
    float wallJumpDirection = 1.0f;
    float wallJumpLastPushOffTimeTimeStamp;


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
        CalculateAnimationHashes();

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
        // If walljumped recently
        if (isWallJumping && Time.time - wallJumpStartTimeStamp <= wallJumpMinPushOffTime)
        {
            float maxAcelerationProgress = Mathf.Max(wallJumpStartAccelerationScalar, Mathf.Abs(walkAxis));

            if (walkAccelerationProgress < maxAcelerationProgress)
            {
                // Apply airborne acceleration
                walkAccelerationProgress = Mathf.Min(walkAccelerationProgress + ((Time.deltaTime / walkAccelerationTime) * wallJumpAccelerationMultiplier), maxAcelerationProgress);
            }

            //Set velocity
            velocity.x = walkDirection * (Mathf.Max(walkAccelerationCurve.Evaluate(walkAccelerationProgress) * walkMaxSpeed, Mathf.Abs(velocity.x)));
        }

        // If walking
        else if (isWalking)
        {
            // Cache the direction
            float newWalkDirection = Mathf.Sign(walkAxis);

            // If we changed direction, set accel progress to 0
            if (newWalkDirection != walkDirection)
            {
                walkAccelerationProgress = 0.0f;
            }
            walkDirection = newWalkDirection;

            // Cap the max accel progresss based on the analog stick
            float maxAcelerationProgress = Mathf.Abs(walkAxis);
            if (walkAccelerationProgress < maxAcelerationProgress)
            {
                // If grounded, apply normal acceleration
                if (isCollidingBelow)
                {
                    walkAccelerationProgress = Mathf.Min(walkAccelerationProgress + (Time.deltaTime / walkAccelerationTime), maxAcelerationProgress);
                }

                // Otherwise, apply airborne acceleratiob
                else
                {
                    walkAccelerationProgress = Mathf.Min(walkAccelerationProgress + ((Time.deltaTime / walkAccelerationTime) * walkAccelerationAirborneMultiplier), maxAcelerationProgress);
                }
            }

            //Set velocity
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
                    if (isCollidingBelow)
                    {
                        walkDecelerationProgress = Mathf.Min(walkDecelerationProgress + (Time.deltaTime / walkDecelerationTime), 1.0f);
                    }

                    // Otherwise, apply airborne deceleration
                    else
                    {
                        walkDecelerationProgress = Mathf.Min(walkDecelerationProgress + ((Time.deltaTime / walkDecelerationTime) * walkDeceleratioAirbornenMultiplier), 1.0f);
                    }
                }
                //Set velocity
                velocity.x = Mathf.Sign(velocity.x) * (Mathf.Min((1.0f - walkDecelerationCurve.Evaluate(walkDecelerationProgress)) * walkMaxSpeed, Mathf.Abs(velocity.x)));
            }
        }
        if (Mathf.Abs(velocity.x) > float.Epsilon)
        {
            spriteRenderer.flipX = velocity.x < 0;
            animator.SetBool(isMovingAnimationHash, true);
        }
        else
        {
            animator.SetBool(isMovingAnimationHash, false);
        }
        animator.SetFloat(velocityXAnimationHash, velocity.x);
    }

    void ApplyGravity()
    {
        // If descending...
        if (Mathf.Sign(velocity.y) == gravitySign)
        {
            // If wall sliding, clamp the max descent speed
            if (isCollidingLeft && walkAxis < 0.0f || isCollidingRight && walkAxis > 0.0f)
            {
                velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravitySign * Time.deltaTime), -wallSlideMaxSpeed, wallSlideMaxSpeed);
            }

            // Otherwise, if jumping, apply the jump descent gravity
            else if (isJumping)
            {
                velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravitySign * Time.deltaTime * jumpDescentGravityMultiplier), -gravityMaxMagnitude, gravityMaxMagnitude);
            }

            // Otherwise, apply normal gravity
            else
            {
                velocity.y = Mathf.Clamp(velocity.y + (gravityAcceleration * gravitySign * Time.deltaTime), -gravityMaxMagnitude, gravityMaxMagnitude);
            }
        }

        // Otherwise, apply normal velocity
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

            // Update collision state
            if (traceSign > 0.0f)
            {
                isCollidingRight = true;
                isCollidingLeft = false;
            }
            else
            {
                isCollidingLeft = true;
                isCollidingRight = false;
            }

            wallJumpDirection = -traceSign;

            isWallJumping = false;
            isJumping = false;
        }
        else
        {
            // Apply normal velocity
            calculatedMovement.x = proposedHorizontalMovement;

            // If we just stopped colliding with a wall
            if (isCollidingRight || isCollidingLeft)
            {
                // Cache the time we stopped touching the wall
                wallJumpLastPushOffTimeTimeStamp = Time.time;

                // Update collision state
                isCollidingLeft = false;
                isCollidingRight = false;
            }
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

            // If we were moving towards gravity
            if (traceSign == gravitySign)
            {
                // If we just became grounded
                if (!isCollidingBelow)
                {
                    isJumping = false;
                    isWallJumping = false;

                    animator.SetBool(isAirborneAnimationHash, false);
                    animator.SetTrigger(onLandAnimationHash);

                    // If a jump is queued in the buffer recently
                    if (jumpLandingQueued)
                    {
                        // If the jump was queued recently then jump
                        if (Time.time - jumpLandingQueuedTimeStamp <= jumpLandingBufferTime)
                        {
                            Jump();
                        }
                        jumpLandingQueued = false;
                    }
                }

                // Update collision state
                isCollidingBelow = true;
                isCollidingAbove = false;
            }

            // If we were moving away from gravity
            else
            {
                isCollidingAbove = true;
                isCollidingBelow = false;
            }
        }

        // If we did not collide with an object
        else
        {
            // Apply normal velocity
            calculatedMovement.y = proposedVerticalMovement;

            // If we are moving towards gravity and just stopped being grounded
            if (isCollidingBelow && traceSign == gravitySign)
            {
                // Cache the walk off ledge time
                jumpWalkedOffLedgeTimeStamp = Time.time;
            }

            // Update collision state
            isCollidingBelow = false;
            isCollidingAbove = false;
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
            spriteRenderer.flipY = !spriteRenderer.flipY;
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

    void CalculateAnimationHashes()
    {
        velocityXAnimationHash = Animator.StringToHash("velocityX");
        onJumpAnimationHash = Animator.StringToHash("onJump");
        onLandAnimationHash = Animator.StringToHash("onLand");
        isAirborneAnimationHash = Animator.StringToHash("isAirborne");
        isMovingAnimationHash = Animator.StringToHash("isMoving");
    }

    // Jump function
    void Jump()
    {
        velocity.y = jumpVelocityMax * -1.0f * gravitySign;
        isJumping = true;
        animator.SetTrigger(onJumpAnimationHash);
        animator.SetBool(isAirborneAnimationHash, true);
        return;
    }

    void WallJump()
    {
        velocity.y = jumpVelocityMax * -1.0f * gravitySign * wallJumpHeightScalar;
        velocity.x = walkAccelerationCurve.Evaluate(wallJumpStartAccelerationScalar) * wallJumpDirection;
        isJumping = true;
        isWallJumping = true;
        walkDirection = wallJumpDirection;
        wallJumpStartTimeStamp = Time.time;

        return;
    }

    // Walking
    public void OnWalk(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isWalking = true;
            walkAxis = context.ReadValue<float>();

            // Perform wall jump is queued in the buffer
            if (!isCollidingBelow 
                && !isWallJumping
                && Mathf.Sign(walkAxis) == wallJumpDirection
                && jumpLandingQueued 
                && Time.time - jumpLandingQueuedTimeStamp <= wallJumpBufferTime)
            {
                jumpLandingQueued = false;
                WallJump();
            }

            // Otherwise, hand;e walking
            else
            {
                // Get the walk deceleration scalar based on the walk acceleration progress
                float walkDecelScalar = walkDecelerationCurve.Evaluate(walkDecelerationProgress);

                // Lookup the opposite of the scalar on acceleration curve
                walkDecelScalar = 1.0f - walkDecelScalar;
                walkAccelerationProgress = GetCurveTimeForValue(walkAccelerationCurve, walkDecelScalar, 10);

                walkDecelerationProgress = 0.0f;
            }
        }
        else if (context.canceled)
        {
            isWalking = false;
            walkAxis = 0.0f;

            // Perform wall jump is queued in the buffer
            if (!isCollidingBelow
                && !isWallJumping
                && jumpLandingQueued
                && Time.time - jumpLandingQueuedTimeStamp <= wallJumpBufferTime)
            {
                jumpLandingQueued = false;
                WallJump();
            }

            // Get the walk deceleration scalar based on the walk deceleration progress;
            float walkAccelScalar = walkAccelerationCurve.Evaluate(walkAccelerationProgress);

            // Lookup the opposite of the scalar on deceleration
            walkAccelScalar = 1.0f - walkAccelScalar;
            walkDecelerationProgress = GetCurveTimeForValue(walkDecelerationCurve, walkAccelScalar, 10);

            walkAccelerationProgress = 0.0f;
        }
        else
        {
            walkAxis = context.ReadValue<float>();

            // Perform wall jump is queued in the buffer
            if (Mathf.Sign(walkAxis) != walkDirection
                && !isCollidingBelow
                && !isWallJumping
                && Mathf.Sign(walkAxis) == wallJumpDirection
                && jumpLandingQueued
                && Time.time - jumpLandingQueuedTimeStamp <= wallJumpBufferTime)
            {
                jumpLandingQueued = false;
                WallJump();
            }
        }
    }

    // Controls
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If grounded, jump
            if (isCollidingBelow)
            {
                Jump();
            }

            // Otherwise, check if we just walked off the ledge
            else if (!isJumping && Time.time - jumpWalkedOffLedgeTimeStamp <= jumpWalkOffLedgeBufferTime)
            {
                Jump();
            }

            // Otherwise, check to see if we are jumping off a wall
            else if ((isCollidingLeft || isCollidingRight || (!isWallJumping && Time.time - wallJumpLastPushOffTimeTimeStamp <= wallJumpBufferTime)) 
                && (walkAxis == 0.0f || Mathf.Sign(walkAxis) == wallJumpDirection))
            {
                WallJump();
            }

            // Otherwise, queue a jump in case we land soon
            else
            {
                jumpLandingQueued = true;
                jumpLandingQueuedTimeStamp = Time.time;
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
        spriteRenderer.flipY = !spriteRenderer.flipY;
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
