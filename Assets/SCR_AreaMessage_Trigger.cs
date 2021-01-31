using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AreaMessage_Trigger : MonoBehaviour
{
    [SerializeField]
    public SCR_InGameMessageController messageController;

    [SerializeField]
    string message;

    [SerializeField]
    float displayTime;

    [SerializeField]
    bool canBeReTriggered;

    bool isDisplayed;
    bool hasBeenTriggered;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDisplayed && (!hasBeenTriggered || canBeReTriggered))
        {
            SCR_PlayerController player = collision.gameObject.GetComponent<SCR_PlayerController>();

            if (player)
            {
                isDisplayed = true;
                hasBeenTriggered = true;
                messageController.ShowMessage(message, this);

                if (displayTime > 0.0f)
                {
                    StartCoroutine(DelayedHideMessage());
                }
            }
        }
        return;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDisplayed && displayTime <= 0.0f)
        {
            SCR_PlayerController player = collision.gameObject.GetComponent<SCR_PlayerController>();

            if (player)
            {
                isDisplayed = false;
                messageController.HideMessage(this);
            }
        }

        return;
    }

    IEnumerator DelayedHideMessage()
    {
        yield return new WaitForSeconds(displayTime);
        messageController.HideMessage(this);
    }
}

