using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CollectibleController : MonoBehaviour, SCR_IDamageable
{
    public int m_PointValue = 1;

    public AudioSource m_Source;
    public SpriteRenderer m_SpriteRenderer;

    private bool m_Collected = false;

    SCR_HUDSwapCounter hudCounter;

    void Start()
    {
        hudCounter = FindObjectOfType<SCR_HUDSwapCounter>();
    }

    void Update()
    {
        if(m_Collected && !m_Source.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SCR_PlayerController playerRef = other.gameObject.GetComponent<SCR_PlayerController>();

        if (!m_Collected && playerRef != null && hudCounter != null)
        {
            hudCounter.AddToCollectible(m_PointValue);
            m_Source.Play();
            m_Collected = true;
            m_SpriteRenderer.enabled = false;
        }
    }

    public void Damage(int amount, GameObject source, Vector3 contactPoint)
    {
        if(m_Collected)
        {
            return;
        }
        Destroy(gameObject, 0.1f);
    }
}
