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

    //Save Information
    public class SaveState
    {
        public string saveName;
        public int activeSceneIndex;
        public Vector2 position;
        public Dictionary<string, bool> inTimeFlags;
        public SaveState(string name, int sceneIndex, Vector2 pos, Dictionary<string, bool> flags)
        {
            saveName = name;
            activeSceneIndex = sceneIndex;
            position = pos;
            inTimeFlags = flags;
        }
        public override string ToString()
        {
            string toReturn = "Save Name: " + saveName + "\nSaved in Scene " + activeSceneIndex + " with player at position " + position;
            return toReturn;
        }
    }
    public List<SaveState> saves;

    public int startSceneNumber = 1;

    private GameObject playerReference;
    public void SetPlayer(GameObject player)
    {
        playerReference = player;
    }

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

        saves = new List<SaveState>();
        loadIntoScene(startSceneNumber);
    }

    public bool CreateSaveState()
    {
        //SWITCH LOCATION FOR ACTUAL PLAYER LOCATION WHEN AVAILABLE
        SaveState toAdd = new SaveState("Save " + saves.Count, SceneManager.GetActiveScene().buildIndex, playerReference.transform.position, InTimeVariables);
        saves.Add(toAdd);
        Debug.Log("Saved " + toAdd.ToString());
        return true;
    }
    public bool CreateSaveState(string saveName)
    {
        bool match = false;
        foreach (var state in saves)
        {
            if (state.saveName == saveName) { match = true; }
        }
        if (!match)
        {
            //SWITCH LOCATION FOR ACTUAL PLAYER LOCATION WHEN AVAILABLE
            SaveState toAdd = new SaveState(saveName, SceneManager.GetActiveScene().buildIndex, playerReference.transform.position, InTimeVariables);
            saves.Add(toAdd);
            Debug.Log("Saved " + toAdd.ToString());
            return true;
        }
        else { return false; }
    }

    private void loadIntoScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    private void Update()
    {
        //Testing Stuff
        if (Input.GetKeyDown(KeyCode.K))
        {
            loadIntoScene(startSceneNumber);
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            CreateSaveState();
        }
    }
}
