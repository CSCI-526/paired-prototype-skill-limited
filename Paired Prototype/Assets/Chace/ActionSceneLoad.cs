using UnityEngine;

public class ActionSceneLoad : MonoBehaviour
{
    void Start()
    {
        var levelMgr = FindObjectOfType<LevelManager>();
        var waves = FindObjectOfType<EnemyWaveManager>(); // expected type

        if (waves == null)
        {
            Debug.LogError("[ActionSceneLoad] No EnemyWaveManager found in scene. " +
                           "Attach EnemyWaveManager to a GameObject in ActionScene and set spawnPoints/prefabs.");
            return;
        }

        if (RunSignals.AfterReward)
        {
            RunSignals.AfterReward = false;
            levelMgr?.NextLevel();
            waves.ApplyUpgradeAndSpawn();
        }
        else
        {
            waves.SpawnWaveFromProgress();
            levelMgr?.RefreshLabel();
        }
    }
}
