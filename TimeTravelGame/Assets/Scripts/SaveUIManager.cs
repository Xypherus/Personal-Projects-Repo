using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveUIManager : MonoBehaviour
{
    public Text saveText;
    public GameObject savePanel;
    public InputField saveInput;
    public Button saveButton, loadButton;

    // Start is called before the first frame update
    void Start()
    {
        savePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            savePanel.SetActive(!savePanel.activeInHierarchy);
            if (savePanel.activeInHierarchy)
            {
                UpdateSaveText();
            }
        }
    }

    private void UpdateSaveText()
    {
        int value = 1;
        string saveTextToSet = "Saves:\n";
        foreach (var save in FlagManager.FlagManagerInstance.saves)
        {
            saveTextToSet = saveTextToSet + value + ": " + save.saveName + "\n";
            value++;
        }
        saveText.text = saveTextToSet;
    }

    public void Save()
    {
        var parseText = saveInput.text;
        if (parseText == "")
        {
            FlagManager.FlagManagerInstance.CreateSaveState();
        }
        else if (!FlagManager.FlagManagerInstance.CreateSaveState(parseText))
        {
            saveInput.text = "Name is Duplicate";
            return;
        }
        UpdateSaveText();
    }
    public void Load()
    {
        var parseText = saveInput.text;
        int saveNum;
        if (int.TryParse(parseText, out saveNum))
        {
            if (!FlagManager.FlagManagerInstance.LoadSaveState(saveNum - 1))
            {
                saveInput.text = "Inputted number out of range";
            }
        }
        else
        {
            saveInput.text = "Input only a number to load a state";
        }
    }
}
