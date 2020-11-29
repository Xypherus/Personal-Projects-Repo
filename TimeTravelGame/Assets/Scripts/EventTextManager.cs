using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTextManager : MonoBehaviour
{
    GameObject eventPanel;
    Text eventText;

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

    IEnumerator delayEndEvent(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        EndEvent();
    }
}
