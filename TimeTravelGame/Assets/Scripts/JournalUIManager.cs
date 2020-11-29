using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUIManager : MonoBehaviour
{
    public Text journalText;
    public GameObject journalPanel;

    // Start is called before the first frame update
    void Start()
    {
        journalPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            journalPanel.SetActive(true);
            journalText.text = FlagManager.FlagManagerInstance.journalText;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            journalPanel.SetActive(false);
        }
    }
}
