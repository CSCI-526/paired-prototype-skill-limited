using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (SelectManager.Instance.Current != null && SelectManager.Instance.Current.gameObject == this.gameObject)
            {
                SelectManager.Instance?.ClearSelection();
            }
            Destroy(gameObject);
        }

        //player death
        else
        {

        }
    }
}
