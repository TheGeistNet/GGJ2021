using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int horizontalRayCount = 4;
    [SerializeField, Min(2)]
    int verticalRayCount = 4;
    [SerializeField]
    const float rayLength = 1.0f;
    List<Vector2> rightEdgeRayOrigins;
    List<Vector2> leftEdgeRayOrigins;
    List<Vector2> topEdgeRayOrigins;
    List<Vector2> bottomEdgeRayOrigins;

    // Debug variables
#if UNITY_EDITOR
    public bool debugDrawRays;
#endif

    // Use awake to initialize values
    void Awake()
    {
        rightEdgeRayOrigins = new List<Vector2>();
        leftEdgeRayOrigins = new List<Vector2>();
        topEdgeRayOrigins = new List<Vector2>();
        bottomEdgeRayOrigins = new List<Vector2>();
        CalculateBoundingBox();
        CalculateRayOrigins();
    }

    private void FixedUpdate()
    {
        PhysicsRaycast();
    }

    void PhysicsRaycast()
    {
        origin = boundingCollider.bounds.center;


        // Debug draw
#if UNITY_EDITOR

        if (debugDrawRays)
        {
            foreach (Vector2 rayOrigin in rightEdgeRayOrigins)
            {
                Debug.DrawRay(origin + rayOrigin, Vector2.right * rayLength, Color.red);
            }

            foreach (Vector2 rayOrigin in leftEdgeRayOrigins)
            {
                Debug.DrawRay(origin + rayOrigin, Vector2.left * rayLength, Color.cyan);
            }

            foreach (Vector2 rayOrigin in topEdgeRayOrigins)
            {
                Debug.DrawRay(origin + rayOrigin, Vector2.up * rayLength, Color.magenta);
            }

            foreach (Vector2 rayOrigin in bottomEdgeRayOrigins)
            {
                Debug.DrawRay(origin + rayOrigin, Vector2.down * rayLength, Color.yellow);
            }
        }
#endif
    }

    void CalculateBoundingBox()
    {
        // Setup collision
        boundingBox = boundingCollider.bounds;
        boundingBox.Expand(skinWidth * -1.0f);
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

        // Calculate right and left edge origins
        rightEdgeRayOrigins.Clear();
        leftEdgeRayOrigins.Clear();
        for (int idx = 0; idx < horizontalRayCount; idx++)
        {
            rightEdgeRayOrigins.Add(new Vector2(halfWidth, ((float)idx * horizontalRaySpacing) - halfHeight));
            leftEdgeRayOrigins.Add(new Vector2(-halfWidth, ((float)idx * horizontalRaySpacing) - halfHeight));
        }

        // Calculate top and bottom edge origins
        topEdgeRayOrigins.Clear();
        bottomEdgeRayOrigins.Clear();
        for (int idx = 0; idx < verticalRayCount; idx++)
        {
            topEdgeRayOrigins.Add(new Vector2(((float)idx * verticalRaySpacing) - halfWidth, halfHeight));
            bottomEdgeRayOrigins.Add(new Vector2(((float)idx * verticalRaySpacing) - halfWidth, -halfHeight));
        }

        return;
    }
}
