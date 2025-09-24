using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //temporary for testing
    public int damage = 5;
    private Health enemy;

    void Awake()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Health>();
    }

    public void Attack()
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
