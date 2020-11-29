using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

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
        [TextArea]
        public string journalEntry;
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
            inTimeFlags = new Dictionary<string, bool>(flags);
        }
        public override string ToString()
        {
            string toReturn = "Save Name: " + saveName + "\nSaved in Scene " + activeSceneIndex
                            + " with player at position " + position + "\nNumber of Variables: " + inTimeFlags.Count;
            return toReturn;
        }
    }
    public List<SaveState> saves;

    public int startSceneNumber = 1;
    public Vector2 playerPositionToSet;
    private bool firstLoad;

    public bool LoadingOffSave;

    [TextArea]
    public string journalText;

    private GameObject playerReference;
    public void SetPlayer(GameObject player)
    {
        playerReference = player;
        if (!firstLoad)
        {
            if (LoadingOffSave)
            {
                player.transform.position = playerPositionToSet;
                LoadingOffSave = false;
            }
        }
        firstLoad = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        firstLoad = true;
        LoadingOffSave = false;
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

    public void updateJournalText()
    {
        string newText = "";
        foreach (var item in GlobalVariables)
        {
            if (item.Value == true)
            {
                foreach (var dgv in defaultGlobalVariables)
                {
                    if (item.Key.Contains(dgv.key))
                    {
                        if (dgv.journalEntry == "") { continue; }
                        newText += "- " + dgv.journalEntry + "\n";
                    }
                }
            }
        }
        journalText = newText;
    }

    IEnumerator WaitSceneLoad(float delay, int sceneNum)
    {
        yield return new WaitForSeconds(delay);
        loadIntoScene(sceneNum);
    }

    public bool CreateSaveState()
    {
        //SWITCH LOCATION FOR ACTUAL PLAYER LOCATION WHEN AVAILABLE
        SaveState toAdd = new SaveState("Save " + (int)(saves.Count + 1), SceneManager.GetActiveScene().buildIndex, playerReference.transform.position, InTimeVariables);
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

    public bool LoadSaveState(int index)
    {
        Debug.Log(saves.Count);
        if (saves.Count >= index && saves.Count != 0)
        {
            Time.timeScale = 1f;
            LoadingOffSave = true;
            var state = saves[index];
            Debug.Log("Loading " + state.ToString());

            InTimeVariables = new Dictionary<string, bool>(state.inTimeFlags);
            playerPositionToSet = state.position;
            StartCoroutine(WaitSceneLoad(1f, state.activeSceneIndex));
            //loadIntoScene(state.activeSceneIndex);
            return true;
        }
        else { return false; }
    }

    [YarnCommand("loadScene")]
    public void LoadScene(string sceneNum)
    {
        int sceneToloadNum;
        if (int.TryParse(sceneNum, out sceneToloadNum))
        {
            StartCoroutine(WaitSceneLoad(1f, sceneToloadNum));
        }
        else
        {
            Debug.LogError("sceneNum: " + sceneNum + " is unparsable");
        }
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
            LoadSaveState(0);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateSaveState();
        }
    }
}
