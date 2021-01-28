using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CollectibleController : MonoBehaviour
{
    SCR_HUDSwapCounter hudCounter;
    // Start is called before the first frame update
    void Start()
    {
        hudCounter = FindObjectOfType<SCR_HUDSwapCounter>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SCR_PlayerController playerRef = other.gameObject.GetComponent<SCR_PlayerController>();

        if(playerRef)
        {
            hudCounter.AddToCollectible();
            Destroy(gameObject, 0.1f);
        }
    }
}
