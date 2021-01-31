using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]

public class SCR_BlockLayout : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField]
    GameObject middlePieceClass;
    [SerializeField]
    GameObject leftEndClass;
    [SerializeField]
    GameObject rightEndClass;
    [SerializeField, Min(0.1f)]
    float middlePieceLength = 1.0f;
    [SerializeField, Min (0.1f)]
    float endLength = 1.0f;
    [SerializeField, Min(0.1f)]
    float height = 0.5f;
    [SerializeField]
    GameObject middlePiece;
    [SerializeField]
    GameObject rightEndPiece;
    [SerializeField]
    GameObject leftEndPiece;


    [ExecuteInEditMode]
    public void LayoutBlock()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }


        if (middlePieceClass)
        {
            // Calculate total length
            float totalLength = middlePieceLength;
            float halfLength = 0.5f * totalLength;

            // Create middle piece
            middlePiece = Instantiate(middlePieceClass, transform);
            middlePiece.transform.localScale = new Vector3(totalLength, height, 1.0f);

            // Create ends
            // Left end
            if (leftEndClass)
            {
                leftEndPiece = Instantiate(leftEndClass, transform);
                leftEndPiece.transform.localPosition = new Vector2(-halfLength - (endLength * 0.5f), 0.0f);
                leftEndPiece.transform.localScale = new Vector3(endLength, height, 1.0f);
                totalLength += endLength;
            }
            if (rightEndClass)
            {
                rightEndPiece = Instantiate(leftEndClass, transform);
                rightEndPiece.transform.localPosition = new Vector2(halfLength + (endLength * 0.5f), 0.0f);
                rightEndPiece.transform.localScale = new Vector3(endLength, height, 1.0f);
                totalLength += endLength;
            }

            // Update collider
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(totalLength, height);
            boxCollider.offset = new Vector2();
        }
        return;
    }

#endif
}