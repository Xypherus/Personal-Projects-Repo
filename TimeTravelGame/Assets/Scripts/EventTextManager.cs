using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class EventTextManager : MonoBehaviour
{
    GameObject eventPanel;
    Text eventText;
    public float defaultEventDuration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        eventPanel = transform.Find("Panel").gameObject;
        eventText = transform.Find("EventText").gameObject.GetComponent<Text>();

        EndEvent();
    }

    private void EndEvent()
    {
        Time.timeScale = 1f;
        eventPanel.SetActive(false);
        eventText.text = "";
        eventText.gameObject.SetActive(false);
    }

    public void StartEvent(string text, float duration)
    {
        Time.timeScale = 0;
        eventText.gameObject.SetActive(true);
        eventText.text = text;
        eventPanel.SetActive(true);
        StartCoroutine(delayEndEvent(duration));
    }

    [YarnCommand("startScreenEvent")]
    public void StartEventYarnVer(string[] text)
    {
        string textToPass = "";
        foreach (var word in text)
        {
            if (word == ":break:") { textToPass += "\n"; }
            else { textToPass += (word + " "); }
        }
        textToPass = textToPass.Trim(new char[] {' ', '"'});
        StartEvent(textToPass, defaultEventDuration);
    }
    [YarnCommand("startScreenEventLong")]
    public void StartEventLongYarnVer(string[] text)
    {
        string textToPass = "";
        foreach (var word in text)
        {
            if (word == ":break:") { textToPass += "\n"; }
            else { textToPass += (word + " "); }
        }
        textToPass = textToPass.Trim(new char[] {' ', '"'});
        textToPass += "\n\n<Press Tab to open Save Menu>";
        StartEvent(textToPass, 999f);
    }

    IEnumerator delayEndEvent(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        EndEvent();
    }
}
