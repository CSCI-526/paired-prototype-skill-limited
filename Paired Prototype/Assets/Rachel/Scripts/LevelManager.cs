using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int currLevel = 1;     // kept for inspector/debug
    public Text levelText;

    void Awake()
    {
        currLevel = RunProgress.Level;
        RefreshLabel();
    }

    public void NextLevel()
    {
        RunProgress.Level++;
        currLevel = RunProgress.Level;
        RefreshLabel();
    }

    public void RefreshLabel()
    {
        if (levelText != null)
            levelText.text = $"Level {RunProgress.Level}";
    }
}
