using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int currLevel = 1;
    public Text levelText;

    public void NextLevel()
    {
        currLevel++;

        levelText.text = $"Level {currLevel}";
    }
}
