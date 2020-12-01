using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStartupEvent : MonoBehaviour
{
    public string variableToCheck;
    [TextArea]
    public string EventText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoEventCheck());
    }

    IEnumerator DoEventCheck()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log(variableToCheck + " = " + FlagManager.FlagManagerInstance.InTimeVariables[variableToCheck].ToString());
        if (!GameObject.FindObjectOfType<CustomYarnVariableStorage>().GetValue(variableToCheck).AsBool)
        {
            GameObject.FindObjectOfType<EventTextManager>().StartEvent(EventText, 3f);
            GameObject.FindObjectOfType<CustomYarnVariableStorage>().SetValue(variableToCheck, true);
        }
    }
}
