using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyWaveManager : MonoBehaviour
{
    [Header("Content")]
    public List<GameObject> enemyPrefabs;
    public string resourcesFolder = "Enemies";
    public Transform[] spawnPoints;

    [Header("Tuning")]
    public int startEnemies = 1;
    public int maxEnemies = 3;
    public float hpStep = 0.2f;
    public float atkStep = 0.1f;
    public int countStep = 1;

    [Header("Overlap Handling")]
    [Tooltip("Extra enemies beyond available spawn points get this max random offset (units).")]
    public float overlapJitterRadius = 0.5f;

    [Header("RNG")]
    public int seed = 0;
    private System.Random rng;

    void Awake()
    {
        rng = seed == 0 ? new System.Random() : new System.Random(seed);

        // Ensure sane baseline if a fresh run hasn't set this yet
        if (RunProgress.EnemyCount < 1)
            RunProgress.EnemyCount = Mathf.Clamp(startEnemies, 1, maxEnemies);

        // Lazy-load prefabs if not assigned in Inspector
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            var loaded = Resources.LoadAll<GameObject>(resourcesFolder);
            if (loaded != null && loaded.Length > 0)
                enemyPrefabs = new List<GameObject>(loaded);
        }
    }

    // Called after Reward when we want to upgrade AND spawn
    public void ApplyUpgradeAndSpawn()
    {
        Debug.Log("[EnemyWaveManager] ApplyUpgradeAndSpawn called.");
        int pick = rng.Next(0, 3); // 0=HP, 1=ATK, 2=Count
        switch (pick)
        {
            case 0:
                RunProgress.HpMult += hpStep;
                Debug.Log($"[EnemyWaveManager] Increased HpMult to {RunProgress.HpMult:F2}");
                break;
            case 1:
                RunProgress.AtkMult += atkStep;
                Debug.Log($"[EnemyWaveManager] Increased AtkMult to {RunProgress.AtkMult:F2}");
                break;
            case 2:
                RunProgress.EnemyCount = Mathf.Clamp(RunProgress.EnemyCount + countStep, 1, maxEnemies);
                Debug.Log($"[EnemyWaveManager] Increased EnemyCount to {RunProgress.EnemyCount}");
                break;
        }

        SpawnWaveFromProgress();
    }

    public void SpawnWaveFromProgress()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("[EnemyWaveManager] No enemyPrefabs assigned or found in Resources/Enemies.");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemyWaveManager] No spawnPoints assigned.");
            return;
        }

        int count = (RunProgress.Level <= 1)
            ? 1
            : Mathf.Clamp(RunProgress.EnemyCount, 1, maxEnemies);

        for (int i = 0; i < count; i++)
        {
            // choose base spawn point
            var p = spawnPoints[i % spawnPoints.Length];
            Vector3 pos = p.position;

            if (i >= spawnPoints.Length && overlapJitterRadius > 0f)
            {
                // Small 2D jitter; adjust axis if your game uses XZ for 2D
                Vector2 jitter2D = Random.insideUnitCircle * overlapJitterRadius;
                pos += new Vector3(jitter2D.x, jitter2D.y, 0f);
            }

            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            var go = Instantiate(prefab, pos, p.rotation);

            // Scale HP from prefab baseline
            var hPrefab = prefab.GetComponent<Health>();
            var h = go.GetComponent<Health>();
            if (h && hPrefab)
            {
                int hp = Mathf.Max(1, Mathf.RoundToInt(hPrefab.maxHealth * RunProgress.HpMult));
                h.maxHealth = hp;
                h.currentHealth = hp;
                var bar = go.GetComponentInChildren<HealthBar>();
                if (bar) bar.SetHealth(h.currentHealth, h.maxHealth);
            }

            // Scale ATK from prefab baseline
            var aPrefab = prefab.GetComponent<EnemyAttack>();
            var atk = go.GetComponent<EnemyAttack>();
            if (atk && aPrefab)
            {
                int dmg = Mathf.Max(1, Mathf.RoundToInt(aPrefab.damage * RunProgress.AtkMult));
                atk.damage = dmg;
            }
        }

        Debug.Log($"[EnemyWaveManager] Spawned {count} enemies.");
    }

    void Start()
    {
        // Only act in the combat scene
        var active = SceneManager.GetActiveScene().name;
        if (active != "ActionScene") return;

        if (RunSignals.AfterReward)
        {
            RunSignals.AfterReward = false;

            // advance level & refresh HUD
            var lvl = FindObjectOfType<LevelManager>();
            lvl?.NextLevel();

            Debug.Log("[EnemyWaveManager] Start(): AfterReward -> upgrade + spawn");
            ApplyUpgradeAndSpawn();
            return;
        }

        // Ensure at least one enemy exists after scene load. We wait one frame to
        // allow other Start() methods (e.g., ActionSceneLoad) to spawn first.
        StartCoroutine(EnsureSpawnedAfterStart());
    }

    private IEnumerator EnsureSpawnedAfterStart()
    {
        yield return null;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0)
        {
            Debug.Log("[EnemyWaveManager] No enemies found after scene load; spawning from progress.");
            SpawnWaveFromProgress();

            var lvl = FindObjectOfType<LevelManager>();
            lvl?.RefreshLabel();
        }
    }
}
