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
        enemy.TakeDamage(damage);
    }
}
