using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Switch : MonoBehaviour
{
    public bool m_Retriggerable = false;
    public GameObject[] m_ObjectsToTrigger;

    private bool m_HasTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_HasTriggered && !m_Retriggerable)
        {
            return;
        }

        SCR_ICanTrigger canTrigger = collision?.gameObject?.GetComponent<SCR_ICanTrigger>();
        if(canTrigger == null)
        {
            return;
        }
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

    private void TriggerFeedback()
    {
        if (m_HasTriggered && !m_Retriggerable)
        {
            return;
        }
        m_HasTriggered = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }
}
