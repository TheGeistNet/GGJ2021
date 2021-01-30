using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Door : MonoBehaviour, SCR_ITriggerable
{
    public Vector3 m_StartPosition;
    public Vector3 m_EndPosition;
    public float m_Time = 2.0f;
    public float m_RumbleAmountX = 0;
    public float m_RumbleAmountY = 0;

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
            transform.position += new Vector3(Random.Range(0, m_RumbleAmountX), Random.Range(0, m_RumbleAmountY), 0);
        }
    }

    public void Trigger()
    {
        m_Closing = true;
        timer = 0;
    }
}
