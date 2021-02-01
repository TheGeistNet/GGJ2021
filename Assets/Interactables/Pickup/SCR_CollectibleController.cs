using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SCR_CollectibleController : MonoBehaviour, SCR_IDamageable
{
    public int m_PointValue = 1;

    public AudioSource m_Source;
    public SpriteRenderer m_SpriteRenderer;
    public VisualEffect m_VFX;

    private bool m_Collected = false;
    private float m_StartPositionY;

    SCR_HUDSwapCounter hudCounter;

    void Start()
    {
        hudCounter = FindObjectOfType<SCR_HUDSwapCounter>();
        m_StartPositionY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, m_StartPositionY + Mathf.Sin(Time.time) * 0.15f, transform.position.z);
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
            m_VFX.Stop();
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
