using UnityEngine;

public class TestSave : MonoBehaviour
{
    //local data
    string randomText = "";
    bool showVideo;
    float musicVolume;
    int totalCoins;

    //the object you want to serialize
    GameValues gameValues = new GameValues();

    string fullPath;

    //if true data will be encripted with an XOR function
    bool encrypt = false;

    //for debug purpose only
    static string logText = "";

    void Awake()
    {
        //the filename where saved data will be stored
        fullPath = Application.persistentDataPath + "/" + "LocalFileName";
    }

    //called from scene using built in unity events
    public void LoadGameValues()
    {
#if JSONSerializationFileSave || BinarySerializationFileSave
        logText += "\nLoad Started (File): " + fullPath;
#else
        logText += "\nLoad Started (PlayerPrefs): " + fullPath;
#endif
        SaveManager.Instance.Load<GameValues>(fullPath, DataWasLoaded, encrypt);
    }

    private void DataWasLoaded(GameValues data, SaveResult result, string message)
    {
        logText += "\nData Was Loaded";
        logText += "\nresult: " + result + ", message: " + message;

        if (result == SaveResult.EmptyData || result == SaveResult.Error)
        {
            logText += "\nNo Data File Found -> Creating new data...";
            gameValues = new GameValues();
        }

        if (result == SaveResult.Success)
        {
            gameValues = data;
        }
        randomText = gameValues.randomText;
        showVideo = gameValues.showVideo;
        musicVolume = gameValues.musicVolume;
        totalCoins = gameValues.totalCoins;
    }

    public void SaveGameValues()
    {
        logText += "\nSave Started";
        gameValues.randomText = randomText;
        gameValues.showVideo = showVideo;
        gameValues.musicVolume = musicVolume;
        gameValues.totalCoins = totalCoins;
        SaveManager.Instance.Save(gameValues, fullPath, DataWasSaved, encrypt);
    }

    private void DataWasSaved(SaveResult result, string message)
    {
        logText += "\nData Was Saved";
        logText += "\nresult: " + result + ", message: " + message;
        if (result == SaveResult.Error)
        {
            logText += "\nError saving data";
        }
    }

    public void Clear()
    {
        logText += "\nClear";
        SaveManager.Instance.ClearFIle(fullPath);
    }

    public void ClearLog()
    {
        logText = "";
    }

    public void AddCoins()
    {
        totalCoins++;
    }

    public void RemoveCoins()
    {
        totalCoins--;
    }

    private void OnGUI()
    {      
        float margin = Screen.width / 20;
        float half = Screen.width / 2-2*margin;
        float buttonHeight = Screen.height / 20;
        GUI.skin.textField.fontSize = 20;
        GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
        GUI.skin.label.fontSize = 20;
        GUI.skin.toggle.fontSize = 20;
        GUI.skin.button.fontSize = 20;
        GUI.Label(new Rect(margin, margin + 0 * buttonHeight, half, buttonHeight), "Random Text");
        randomText = GUI.TextField(new Rect(margin, margin + 1 * buttonHeight, half, buttonHeight), randomText, 20);
        showVideo = GUI.Toggle(new Rect(margin, margin + 2*buttonHeight, half, buttonHeight), showVideo, "Show video");
        GUI.Label(new Rect(margin, margin + 3 * buttonHeight, half, buttonHeight), "Music Volume");
        musicVolume = GUI.HorizontalSlider(new Rect(margin, margin + 4 * buttonHeight, half, buttonHeight), musicVolume, 0.0F, 1.0F);
        GUI.Label(new Rect(margin, margin + 5 * buttonHeight, half, buttonHeight), "Total Coins: "+totalCoins);
        if(GUI.Button(new Rect(margin, margin + 6 * buttonHeight, half, buttonHeight), "Add Coins"))
        {
            AddCoins();
        }
        if (GUI.Button(new Rect(margin, 2*margin + 7 * buttonHeight, half, buttonHeight), "Remove Coins"))
        {
            AddCoins();
        }
        if (GUI.Button(new Rect(margin, 3 * margin + 8 * buttonHeight, half, buttonHeight), "Load"))
        {
            LoadGameValues();
        }
        if (GUI.Button(new Rect(margin, 4 * margin + 9 * buttonHeight, half, buttonHeight), "Save"))
        {
            SaveGameValues();
        }
        if (GUI.Button(new Rect(margin, 5 * margin + 10 * buttonHeight, half, buttonHeight), "Clear Save"))
        {
            Clear();
        }
        if (GUI.Button(new Rect(margin, 6 * margin + 11 * buttonHeight, half, buttonHeight), "Clear Log"))
        {
            ClearLog();
        }
        GUI.Label(new Rect(half+2*margin, margin, half, Screen.height - 2 * margin), logText);
    }
}
