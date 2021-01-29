using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_StaticSpikes : MonoBehaviour
{
    public int m_DamageAmount = 1;

    public int m_AmountInStrip = 1;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other == null)
        {
            return;
        }
        SCR_IDamageable damagable = other.GetComponent<SCR_IDamageable>();
        if (damagable == null)
        {
            return;
        }
        damagable.Damage(m_DamageAmount);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other == null)
        {
            return;
        }
        SCR_IDamageable damagable = other.GetComponent<SCR_IDamageable>();
        if (damagable == null)
        {
            return;
        }
        damagable.Damage(m_DamageAmount);
    }

    public void LayoutSingleSpikes()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x, 0.5f * (m_AmountInStrip - 1));
        sprite.size = new Vector2(sprite.size.x, 0.5f * m_AmountInStrip);
        GameObject leftCollider = transform.GetChild(0).gameObject;
        GameObject rightCollider = transform.GetChild(1).gameObject;
        leftCollider.transform.localPosition = new Vector3(0, -0.5f + 0.25f * (m_AmountInStrip - 1), 0);
        rightCollider.transform.localPosition = new Vector3(0, -0.5f - 0.25f * (m_AmountInStrip - 1), 0);
    }
}
