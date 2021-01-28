using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SCR_PlayerDeath : MonoBehaviour, SCR_IDamageable, SCR_ICanTrigger
{
    public GameObject[] m_OnDeathFeedbackObjects;
    public float m_TimeToWait = 2.0f;

    private float m_TimeOfDeath = -1;

    private void Awake()
    {
        foreach(GameObject g in m_OnDeathFeedbackObjects)
        {
            g.SetActive(false);
        }
    }

    public void Damage(int amount)
    {
        Kill();
    }

    public void OnQuickRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Kill();
            m_TimeOfDeath = Time.time + m_TimeToWait;
        }
    }

    public void Kill()
    {
        m_TimeOfDeath = Time.time;
        foreach (GameObject g in m_OnDeathFeedbackObjects)
        {
            g.SetActive(true);
        }
    }

    private void Update()
    {
        if(m_TimeOfDeath != -1)
        {
            if (Time.time > m_TimeToWait + m_TimeOfDeath)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
