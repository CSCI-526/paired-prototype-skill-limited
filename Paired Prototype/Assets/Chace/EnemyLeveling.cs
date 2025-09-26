using UnityEngine;

public class EnemyLeveler : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public int startEnemies = 1;
    public int maxEnemies = 10;

    public float hpStep = 0.2f;
    public float atkStep = 0.1f;
    public int countStep = 1;

    public int seed = 0;

    private int level = 0;
    private int currentCount;
    private float hpMult = 1f;
    private float atkMult = 1f;
    private int baseHP = 100;
    private int baseATK = 5;
    private System.Random rng;

    void Awake()
    {
        rng = seed == 0 ? new System.Random() : new System.Random(seed);
        currentCount = Mathf.Clamp(startEnemies, 1, maxEnemies);

        if (enemyPrefab)
        {
            var h = enemyPrefab.GetComponent<Health>();
            if (h) baseHP = Mathf.Max(1, h.maxHealth);
            var a = enemyPrefab.GetComponent<EnemyAttack>();
            if (a) baseATK = Mathf.Max(1, a.damage);
        }
    }

    public void NextRound()
    {
        level++;
        int pick = rng.Next(0, 3);
        if (pick == 0) hpMult += hpStep;
        else if (pick == 1) atkMult += atkStep;
        else currentCount = Mathf.Clamp(currentCount + countStep, 1, maxEnemies);

        DespawnAll();
        SpawnWave();
    }

    private void DespawnAll()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++) Destroy(enemies[i]);
    }

    private void SpawnWave()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        for (int i = 0; i < currentCount; i++)
        {
            var p = spawnPoints[i % spawnPoints.Length];
            var go = Instantiate(enemyPrefab, p.position, p.rotation);

            var h = go.GetComponent<Health>();
            if (h)
            {
                int hp = Mathf.Max(1, Mathf.RoundToInt(baseHP * hpMult));
                h.maxHealth = hp;
                h.currentHealth = hp;
                var bar = go.GetComponentInChildren<HealthBar>();
                if (bar) bar.SetHealth(h.currentHealth, h.maxHealth);
            }

            var atk = go.GetComponent<EnemyAttack>();
            if (atk)
            {
                int dmg = Mathf.Max(1, Mathf.RoundToInt(baseATK * atkMult));
                atk.damage = dmg;
            }
        }
    }
}
