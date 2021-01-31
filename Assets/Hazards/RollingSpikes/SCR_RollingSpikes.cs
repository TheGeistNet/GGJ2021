using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_RollingSpikes : SCR_DimensionSwapObserverBase, SCR_ICanTrigger
{
    public int m_DamageAmount = 1;
    public bool m_UseCustomForce = false;
    public Vector2 m_CustomForceNormal = new Vector2(0.0f, -9.8f);
    public Vector2 m_CustomForceInverted = new Vector2(0.0f, 9.8f);

    private Rigidbody2D m_RigidBody;
    private ConstantForce2D m_GravitySimulator;
    private bool m_IsForceInverted = false;

    protected override void Awake()
    {
        base.Awake();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_RigidBody.gravityScale = 0.0f;
        m_GravitySimulator = GetComponent<ConstantForce2D>();
        if (!m_UseCustomForce)
        {
            if (m_IsForceInverted)
            {
                m_GravitySimulator.force = Physics2D.gravity * -1.0f;
            }
            else
            {
                m_GravitySimulator.force = Physics2D.gravity;
            }
        }
        else
        {
            if (m_IsForceInverted)
            {
                m_GravitySimulator.force = m_CustomForceInverted;
            }
            else
            {
                m_GravitySimulator.force = m_CustomForceNormal;
            }
        }

    }
    public override void DoSwap(eSwapType type)
    {
        if(type != eSwapType.SWAP_TYPE_GRAVITY)
        {
            return;
        }
        m_IsForceInverted = !m_IsForceInverted;
        if (!m_UseCustomForce)
        {
            m_GravitySimulator.force = new Vector2(m_GravitySimulator.force.x, -m_GravitySimulator.force.y);
        }
        else
        {
            if (m_IsForceInverted)
            {
                m_GravitySimulator.force = m_CustomForceInverted;
            }
            else
            {
                m_GravitySimulator.force = m_CustomForceNormal;
            }
        }

        return;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDamage(collision.gameObject, transform.position);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        CheckDamage(collision.gameObject, new Vector3(contactPoint.x, contactPoint.y, 0));
    }

    private void CheckDamage(GameObject other, Vector3 collisionPoint)
    {
        if (other == null)
        {
            return;
        }
        SCR_IDamageable damagable = other.GetComponent<SCR_IDamageable>();
        if (damagable == null)
        {
            return;
        }
        damagable.Damage(m_DamageAmount, gameObject, collisionPoint);
    }

}
