using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("UI Transition")]
    public Animator transition;       // trigger name must be "Start"
    public float transitionTime = 1f; // seconds

    [Header("Scene Names")]
    public string actionScene;
    public string rewardScene;
    public string gameOverScene;
    public string mainMenuScene;

    bool isLoading;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // keep the wipe across scenes
    }

    public void LoadAction()     => LoadScene(actionScene);
    public void LoadReward()     => LoadScene(rewardScene);
    public void LoadGameOver()   => LoadScene(gameOverScene);
    public void LoadMainMenu()   => LoadScene(mainMenuScene);

    public void LoadScene(string sceneName)
    {
        if (!isLoading) StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        isLoading = true;
        if (transition) transition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene(sceneName);
        isLoading = false;
    }
}
