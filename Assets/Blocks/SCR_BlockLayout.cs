using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class SCR_BlockLayout : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    Vector2 platformSize = new Vector2(1.0f, 0.5f);

    [ExecuteInEditMode]
    public void LayoutBlock()
    {
        GetComponent<SpriteRenderer>().size = platformSize;
        GetComponent<BoxCollider2D>().size = platformSize;

        return;
    }

#endif
}
