using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SCR_PlayerDeath : MonoBehaviour, SCR_IDamageable, SCR_ICanTrigger
{
    public VisualEffect m_VFX;
    public SpriteRenderer m_SpriteRenderer;
    public SCR_PlayerController m_PlayerController;
    public float m_TimeToWait = 2.0f;

    private float m_TimeOfDeath = -1;

    SCR_PlayerAudio playerAudio;

    private void Awake()
    {
        m_VFX.Stop();
        playerAudio = GetComponent<SCR_PlayerAudio>();
    }

    public void Damage(int amount, GameObject source, Vector3 contactPoint)
    {
        Kill(contactPoint);
    }

    public void OnQuickRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Kill(Vector3.zero);
            m_TimeOfDeath = Time.time + m_TimeToWait;
        }
    }

    public void Kill(Vector3 contactPoint)
    {
        if(m_TimeOfDeath != -1)
        {
            return;
        }
        m_TimeOfDeath = Time.time;
        m_VFX.SetVector3("SprayDirection", -(transform.position - contactPoint).normalized);
        m_VFX.Play();
        m_SpriteRenderer.enabled = false;
        m_PlayerController.physicsDisabled = true;
        playerAudio.AudioDeath();
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
