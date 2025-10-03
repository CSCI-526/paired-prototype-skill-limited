using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool player = false;

    // Temporary block and power mechanics to support cards
    public int currentBlock = 0;
    public int power = 0;
    public bool blockDisabledThisTurn = false;

    private HealthBar healthBar;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = GlobalVars.currPlayerHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        // Absorb with block first
        if (currentBlock > 0 && damage > 0)
        {
            int absorbed = Mathf.Min(currentBlock, damage);
            currentBlock -= absorbed;
            damage -= absorbed;
        }

        currentHealth -= damage;

        if(player)
            GlobalVars.currPlayerHealth = currentHealth;

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

        if(player)
            GlobalVars.currPlayerHealth = currentHealth;

        UpdateBar();
    }

    public void GainBlock(int block)
    {
        if (block <= 0) return;
        if (blockDisabledThisTurn) return;
        currentBlock += block;
    }

    public void GainPower(int amount)
    {
        power += amount;
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
            if (SelectManager.Instance.Current != null && SelectManager.Instance.Current.gameObject == this.gameObject)
            {
                SelectManager.Instance?.ClearSelection();
            }
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

            GlobalVars.currPlayerHealth = 100;
            PlayerPrefs.SetInt("LastRunLevelsBeaten", levelsBeaten);
            int best = PlayerPrefs.GetInt("BestLevelsBeaten", 0);
            if (levelsBeaten > best) PlayerPrefs.SetInt("BestLevelsBeaten", levelsBeaten);
            PlayerPrefs.Save();

           LevelLoader.Instance?.LoadGameOver();
        }
    }
}
