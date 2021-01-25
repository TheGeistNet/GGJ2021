using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerController : MonoBehaviour
{
    // Collision
    [SerializeField]
    private BoxCollider2D boundingCollider;
    [SerializeField]
    private float skinWidth = 0.015f;
    private Bounds boundingBox;
    Vector2 bottomRight;
    Vector2 bottomLeft;
    Vector2 topRight;
    Vector2 topLeft;

    // Use awake to initialize values
    void Awake()
    {
        // Setup collision
        boundingBox = boundingCollider.bounds;
        boundingBox.Expand(skinWidth);
        bottomRight = new Vector2(boundingBox.max.x, boundingBox.min.y);
        bottomLeft = new Vector2(boundingBox.min.x, boundingBox.min.y);
        topRight = new Vector2(boundingBox.max.y, boundingBox.max.x);
        topLeft = new Vector2(boundingBox.max.y, boundingBox.min.x);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
