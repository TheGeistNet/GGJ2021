using System.Collections;
using System.Collections.Generic;
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
    GameObject middlePiece;
    GameObject rightEndPiece;
    GameObject leftEndPiece;


    public void LayoutBlock()
    {
        // Clear old pieces
        if (middlePiece)
        {
            Destroy(middlePiece);
        }
        if (leftEndPiece)
        {
            Destroy(leftEndPiece);
        }
        if (rightEndPiece)
        {
            Destroy(rightEndPiece);
        }


        if (middlePieceClass)
        {
            // Calculate total length
            float totalLength = middlePieceLength;
            float halfLength = 0.5f * totalLength;

            // Create middle piece
            middlePiece = Instantiate(middlePieceClass, transform);
            middlePiece.transform.localScale = new Vector2(totalLength, height);

            // Create ends
            // Left end
            if (leftEndClass)
            {
                leftEndPiece = Instantiate(leftEndClass, transform);
                leftEndPiece.transform.localPosition = new Vector2(-halfLength - (endLength * 0.5f), 0.0f);
                leftEndPiece.transform.localScale = new Vector2(endLength, height);
                totalLength += endLength;
            }
            if (rightEndClass)
            {
                rightEndPiece = Instantiate(leftEndClass, transform);
                rightEndPiece.transform.localPosition = new Vector2(halfLength + (endLength * 0.5f), 0.0f);
                rightEndPiece.transform.localScale = new Vector2(endLength, height);
                totalLength += endLength;
            }

            // Update collider
            GetComponent<BoxCollider2D>().size = new Vector2(totalLength, height);
        }


        return;
    }

#endif
}
