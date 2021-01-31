using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CollectibleController : MonoBehaviour, SCR_IDamageable
{
    public int m_PointValue = 1;
    SCR_HUDSwapCounter hudCounter;
    // Start is called before the first frame update
    void Start()
    {
        hudCounter = FindObjectOfType<SCR_HUDSwapCounter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SCR_PlayerController playerRef = other.gameObject.GetComponent<SCR_PlayerController>();

        if (playerRef)
        {
            hudCounter.AddToCollectible(m_PointValue);
            Destroy(gameObject, 0.1f);
        }
    }

    public void Damage(int amount, GameObject source, Vector3 contactPoint)
    {
        Destroy(gameObject, 0.1f);
    }
}
