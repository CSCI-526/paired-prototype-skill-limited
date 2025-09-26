using UnityEngine;
using System.Collections.Generic;

public class EnemyWaveManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public string resourcesFolder = "Enemies";
    public Transform[] spawnPoints;

    public int startEnemies = 1;
    public int maxEnemies = 10;

    public float hpStep = 0.2f;
    public float atkStep = 0.1f;
    public int countStep = 1;

    public int seed = 0;

    private int currentCount;
    private float hpMult = 1f;
    private float atkMult = 1f;
    private System.Random rng;

    void Awake()
    {
        rng = seed == 0 ? new System.Random() : new System.Random(seed);
        currentCount = Mathf.Clamp(startEnemies, 1, maxEnemies);
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            var loaded = Resources.LoadAll<GameObject>(resourcesFolder);
            if (loaded != null && loaded.Length > 0)
                enemyPrefabs = new List<GameObject>(loaded);
        }
    }

    public void ApplyUpgradeAndSpawn()
    {
        int pick = rng.Next(0, 3);
        if (pick == 0) hpMult += hpStep;
        else if (pick == 1) atkMult += atkStep;
        else currentCount = Mathf.Clamp(currentCount + countStep, 1, maxEnemies);

        SpawnWave();
    }

    private void SpawnWave()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < currentCount; i++)
        {
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            var p = spawnPoints[i % spawnPoints.Length];
            var go = Instantiate(prefab, p.position, p.rotation);

            var hPrefab = prefab.GetComponent<Health>();
            var aPrefab = prefab.GetComponent<EnemyAttack>();

            var h = go.GetComponent<Health>();
            if (h && hPrefab)
            {
                int hp = Mathf.Max(1, Mathf.RoundToInt(hPrefab.maxHealth * hpMult));
                h.maxHealth = hp;
                h.currentHealth = hp;
                var bar = go.GetComponentInChildren<HealthBar>();
                if (bar) bar.SetHealth(h.currentHealth, h.maxHealth);
            }

            var atk = go.GetComponent<EnemyAttack>();
            if (atk && aPrefab)
            {
                int dmg = Mathf.Max(1, Mathf.RoundToInt(aPrefab.damage * atkMult));
                atk.damage = dmg;
            }
        }
    }
}
