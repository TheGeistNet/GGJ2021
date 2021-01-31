using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SCR_InGameMessageController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI messageText;

    [SerializeField]
    Animator messageAnimator;

    string currentMessageText;

    SCR_AreaMessage_Trigger currentMessageSource;

    public void ShowMessage(string message, SCR_AreaMessage_Trigger messageSource)
    {
        currentMessageSource = messageSource;
        currentMessageText = message;
        {
            if (messageAnimator.GetBool("isOpen"))
            {
                messageText.SetText(currentMessageText);
            }
            else
            {
                messageAnimator.SetBool("isOpen", true);
            }
        }

        return;
    }

    public void HideMessage(SCR_AreaMessage_Trigger messageSource)
    {
        if (currentMessageSource == messageSource)
        {
            messageAnimator.SetBool("isOpen", false);
            currentMessageSource = null;
        }

        return;
    }

    public void SwapText()
    {
        messageText.SetText(currentMessageText);
        return;
    }
}
