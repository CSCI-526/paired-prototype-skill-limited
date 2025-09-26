using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void OnPlayCardClick()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //player attacks an enemy
        //temporary
        //temp player movement
        PlayerAttack p_atk = player.GetComponent<PlayerAttack>();
        p_atk.Attack();

        //enemies attack player
        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;

            EnemyAttack atk = e.GetComponent<EnemyAttack>();
            if (atk != null)
                atk.Attack();
        } 

        //check if all enemies are dead
        StartCoroutine(CheckAllEnemiesDead());
    }

    private IEnumerator CheckAllEnemiesDead()
    {
        yield return null;

        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            //change level UI
            var lm = FindObjectOfType<LevelManager>();
            lm.NextLevel();

            //move onto next level
            FindObjectOfType<EnemyWaveManager>()?.ApplyUpgradeAndSpawn();
        }
    }
}
