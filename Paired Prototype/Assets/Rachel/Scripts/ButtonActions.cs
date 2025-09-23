using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void OnPlayCardClick()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;

            EnemyAttack atk = e.GetComponent<EnemyAttack>();
            if (atk != null)
                atk.Attack();
        }

        //temp player movement
        //PlayerAttack p_atk = player.GetComponent<PlayerAttack>();
        //p_atk.Attack();
    }
}
