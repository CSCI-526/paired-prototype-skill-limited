using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("UI Transition")]
    public Animator transition;        // Animator with a Trigger "Start" that plays your wipe
    public float transitionTime = 1f;  // seconds

    [Header("Scene Names")]
    public string mainMenuScene   = "MainMenu";
    public string actionScene     = "ActionScene";
    public string rewardScene     = "CardRewardScene";
    public string gameOverScene   = "GameOver";

    bool isLoading;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMainMenu() => LoadScene(mainMenuScene);
    public void LoadAction()   => LoadScene(actionScene);
    public void LoadReward()   => LoadScene(rewardScene);
    public void LoadGameOver() => LoadScene(gameOverScene);

    public void LoadScene(string sceneName)
    {
        if (!isLoading) StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
{
    isLoading = true;

    if (transition) transition.SetTrigger("Start");           // fade to black
    yield return new WaitForSecondsRealtime(transitionTime);  // match Circle_Start length

    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    yield return null; // let one frame render under the overlay

    if (transition) transition.SetTrigger("End");             // fade back to clear
    // (optional) yield return new WaitForSecondsRealtime(transitionTime);

    isLoading = false;
}

}
