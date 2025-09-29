using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool player = false;

    private HealthBar healthBar;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        UpdateBar();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        //enemy death
        if (!player)
        {
            Destroy(gameObject);
        }

        //player death
        else
        {
            var lm = FindObjectOfType<LevelManager>();
            int levelsBeaten = 0;
            if (lm != null)
            {
                // levels beaten = last fully cleared level = currLevel - 1
                levelsBeaten = Mathf.Max(0, lm.currLevel - 1);
            }

            PlayerPrefs.SetInt("LastRunLevelsBeaten", levelsBeaten);
            int best = PlayerPrefs.GetInt("BestLevelsBeaten", 0);
            if (levelsBeaten > best) PlayerPrefs.SetInt("BestLevelsBeaten", levelsBeaten);
            PlayerPrefs.Save();

           LevelLoader.Instance?.LoadGameOver();
        }
    }
}
