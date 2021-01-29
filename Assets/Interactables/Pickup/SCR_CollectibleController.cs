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

        if(playerRef)
        {
            for (int x = 0; x < m_PointValue; ++x)
            {
                hudCounter.AddToCollectible();
            }
            Destroy(gameObject, 0.1f);
        }
    }

    public void Damage(int amount)
    {
        Destroy(gameObject, 0.1f);
    }
}
