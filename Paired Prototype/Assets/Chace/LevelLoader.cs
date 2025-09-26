using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    // Update is called once per frame
    void Update()
    {
        //trigger level load after After player either upgrades or gained new cards or presses L key
        if (PlayerHasUpgraded() || PlayerHasGainedNewCards() || Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    private bool PlayerHasUpgraded()
    {
        return false;
    }

    private bool PlayerHasGainedNewCards()
    {
        return false;
    }

    private void LoadNextLevel()
    {
        StartCoroutine(LoadLevelAfterDelay(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevelAfterDelay(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
