using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //temporary for testing
    public int damage = 5;
    private Health player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    public void Attack()
    {
        if (player.currentHealth > 0)
        {
            if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
            {
                Health enemyHealth = SelectManager.Instance.Current.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }
}
