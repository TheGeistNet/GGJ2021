using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_ScoreboardHUD : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;
    public Text m_LabelText;
    public Text m_GradeText;

    private float m_Timer;
    AudioSource thisAudioSource;

    private void Start()
    {
        thisAudioSource = GetComponent<AudioSource>();
    }

    public void StartScoreboard(SCR_ScoreboardEntry entry)
    {
        background1.SetActive(true);
        background2.SetActive(true);
        thisAudioSource.Play();
        //m_LabelText.text = string.Format(m_LabelText.text, entry.usedSwaps, entry.maxSwaps, entry.amountCollected, entry.maxCollected);
        m_LabelText.text = string.Format(m_LabelText.text, entry.usedSwaps, entry.amountCollected, entry.maxCollected);
        int index = -1;
        for(int x = 0; x < entry.rankingValues.Length; ++x)
        {
            if(entry.rankingValues[x] >= entry.usedSwaps)
            {
                index = x;
                break;
            }
        }
        if(index == -1)
        {
            m_GradeText.text = string.Format(m_GradeText.text, "D");
        }
        else
        {
            m_GradeText.text = string.Format(m_GradeText.text, entry.rankingStrings[index]);
        }
        m_Timer = 5.0f;

    }

    private void Update()
    {
        if(m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
        }
    }

    public bool Finished()
    {
        return m_Timer < 0.0f;
    }
}
