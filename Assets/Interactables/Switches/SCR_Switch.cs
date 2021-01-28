using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Switch : MonoBehaviour
{
    public ContactFilter2D m_Filter;
    public bool m_Retriggerable = false;
    public float m_RetriggerDuration = 2.0f;
    public GameObject[] m_ObjectsToTrigger;

    private float m_RetriggerTimer = 0.0f;
    private bool m_HasTriggered = false;
    private Collider2D m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanTrigger())
        {
            return;
        }

        SCR_ICanTrigger triggerObject = collision?.gameObject?.GetComponent<SCR_ICanTrigger>();
        if(triggerObject == null)
        {
            return;
        }
        Trigger();
    }

    private void Trigger()
    {
        foreach (GameObject g in m_ObjectsToTrigger)
        {
            SCR_ITriggerable trigger = g.GetComponent<SCR_ITriggerable>();
            if (trigger != null)
            {
                //make sure we actually did something with the switch if we have dynamic trigger lists
                TriggerFeedback();
                trigger.Trigger();
            }
        }
    }

    private void FixedUpdate()
    {
        if(m_RetriggerTimer > 0.0f)
        {
            m_RetriggerTimer -= Time.fixedDeltaTime;
            if(m_RetriggerTimer <= 0.0f)
            {
                RevertTriggerFeedback();
            }
        }
    }

    private bool CanTrigger()
    {
        return !m_HasTriggered || (m_Retriggerable && m_RetriggerTimer == 0.0f);
    }

    private void RevertTriggerFeedback()
    {
        m_RetriggerTimer = 0.0f;
        m_HasTriggered = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        int num = m_Collider.OverlapCollider(m_Filter, overlappingColliders);
        if(num != 0)
        {
            foreach (Collider2D g in overlappingColliders)
            {
                SCR_ICanTrigger trigger = g.gameObject.GetComponent<SCR_ICanTrigger>();
                if (trigger != null)
                {
                    Trigger();
                }
            }
        }
    }

    private void TriggerFeedback()
    {
        if (!CanTrigger())
        {
            return;
        }
        m_HasTriggered = true;
        m_RetriggerTimer = m_RetriggerDuration;
        GetComponent<SpriteRenderer>().color = Color.green;
    }
}
