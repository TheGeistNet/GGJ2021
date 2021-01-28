using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Door : MonoBehaviour, SCR_ITriggerable
{
    public Vector3 m_StartPosition;
    public Vector3 m_EndPosition;
    public float m_Time = 2.0f;

    private Vector3 direction;
    private float timer;
    private bool m_Closing = false;

    private void Awake()
    {
        m_Closing = false;
        timer = 0;
        direction = m_EndPosition - m_StartPosition;
        transform.position = m_StartPosition;
    }


    public void FixedUpdate()
    {
        if(m_Closing)
        {
            timer += Time.fixedDeltaTime;
            if(timer > m_Time)
            {
                timer = m_Time;
                m_Closing = false;
            }
            transform.position = m_StartPosition + direction * (timer / m_Time);
        }

    }

    public void Trigger()
    {
        m_Closing = true;
        timer = 0;
    }
}
