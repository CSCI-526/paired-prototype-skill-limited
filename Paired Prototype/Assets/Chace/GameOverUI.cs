using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  

public class GameOverUI : MonoBehaviour
{
    public TMP_Text titleText;  
    public TMP_Text infoText;
    public string mainMenuSceneName = "MainMenuScene";

    void Start()
    {
        if (titleText) titleText.text = "GAME OVER";
        int last = PlayerPrefs.GetInt("LastRunLevelsBeaten", 0);
        if (infoText) infoText.text = $"Max Level Beat: {last}";
    }

    public void ReturnToMenu()
    {
        LevelLoader.Instance?.LoadMainMenu();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            ReturnToMenu();
    }
}
