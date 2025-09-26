using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool player = false;

    // Temporary block and power mechanics to support cards
    public int currentBlock = 0;
    public int power = 0;

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
        // Absorb with block first
        if (currentBlock > 0 && damage > 0)
        {
            int absorbed = Mathf.Min(currentBlock, damage);
            currentBlock -= absorbed;
            damage -= absorbed;
        }

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

    public void GainBlock(int block)
    {
        if (block <= 0) return;
        currentBlock += block;
    }

    public void GainPower(int amount)
    {
        power += amount;
        if (power < 0) power = 0;
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

        }
    }
}
