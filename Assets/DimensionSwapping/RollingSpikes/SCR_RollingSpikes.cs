using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_RollingSpikes : SCR_DimensionSwapObserverBase, SCR_ICanTrigger
{
    public int m_DamageAmount;

    private Rigidbody2D m_RigidBody;
    private ConstantForce2D m_GravitySimulator;

    protected override void Awake()
    {
        base.Awake();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_RigidBody.gravityScale = 0.0f;
        m_GravitySimulator = GetComponent<ConstantForce2D>();
        m_GravitySimulator.force = Physics2D.gravity;
    }
    public override void DoSwap(eSwapType type)
    {
        if(type != eSwapType.SWAP_TYPE_GRAVITY)
        {
            return;
        }
        m_GravitySimulator.force = new Vector2(m_GravitySimulator.force.x, -m_GravitySimulator.force.y);
    }

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

}
