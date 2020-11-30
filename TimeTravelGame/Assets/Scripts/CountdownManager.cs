using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class CountdownManager : MonoBehaviour
{
    public Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        countdownText.text = "Actions Remaining: " + FlagManager.FlagManagerInstance.startingActions;
        countdownText.gameObject.SetActive(FlagManager.FlagManagerInstance.intimeCountdownActive);
    }

    public void SetCountdownActive(bool isActive)
    {
        FlagManager.FlagManagerInstance.intimeCountdownActive = isActive;
        countdownText.gameObject.SetActive(FlagManager.FlagManagerInstance.intimeCountdownActive);
    }

    [YarnCommand("setCountdownActive")]
    public void SetCountdownActiveYarn(string isActive)
    {
        bool setActValue;
        if (bool.TryParse(isActive, out setActValue))
        {
            SetCountdownActive(setActValue);
        }
        else
        {
            Debug.LogError("Countdown activity was not set. Could not parse: " + isActive);
        }
    }

    public void actionUsed()
    {
        countdownText.text = "Actions Remaining: " + FlagManager.FlagManagerInstance.remainingActions;
        if (FlagManager.FlagManagerInstance.remainingActions <= 0)
        {
            SetCountdownActive(false);
            StartCoroutine(DelayEndingEvent());
        }
    }

    IEnumerator DelayEndingEvent()
    {
        yield return new WaitForSecondsRealtime(3.5f);
        StartEndingEvent();
    }

    private void StartEndingEvent()
    {
        if (!FlagManager.FlagManagerInstance.InTimeVariables["$butcher_alive"])
        {
            GameObject.FindObjectOfType<EventTextManager>().StartEvent("With the infiltrator dead, the town easily repels the raiders\n\n<Press Tab to load a Save>", 999);
        }
        else
        {
            GameObject.FindObjectOfType<EventTextManager>().StartEvent("The raiders attack, and the town burns to the ground within hours\n\n<Press Tab to load a Save>", 999);
        }
    }
}
