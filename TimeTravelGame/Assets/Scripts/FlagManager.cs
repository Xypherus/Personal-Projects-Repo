using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagManager : MonoBehaviour
{

    #region Singleton
    public static FlagManager FlagManagerInstance = null;
    private void Awake()
    {
        if (FlagManagerInstance == null)
        {
            FlagManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    #endregion

    //Global Variable Information
    [System.Serializable]
    public class DefaultGlobalVariable
    {
        public string key;
        public bool value;
    }
    public DefaultGlobalVariable[] defaultGlobalVariables;
    public Dictionary<string, bool> GlobalVariables;

    //InTime Variable Information
    public Dictionary<string, bool> InTimeVariables;

    public int startSceneNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        GlobalVariables = new Dictionary<string, bool>();
        foreach (DefaultGlobalVariable dgv in defaultGlobalVariables)
        {
            if (dgv.key != "" && dgv.key != null)
            {
                GlobalVariables.Add("$" + dgv.key, dgv.value);
            }
        }

        InTimeVariables = new Dictionary<string, bool>();

        Debug.Log("Global Variables Length = " + GlobalVariables.Count);
        loadIntoScene(startSceneNumber);
    }

    private void loadIntoScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    private void Update() {
        //Testing Stuff
        if(Input.GetKeyDown(KeyCode.K))
        {
            loadIntoScene(startSceneNumber);
        }
    }
}
