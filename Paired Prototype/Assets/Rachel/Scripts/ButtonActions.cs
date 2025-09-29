using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void OnPlayCardClick()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack p_atk = player.GetComponent<PlayerAttack>();

        StartCoroutine(PlayAttackSeq(enemies, p_atk));
    }

    private IEnumerator PlayAttackSeq(GameObject[] enemies, PlayerAttack player)
    {
        //temp player movement
        player.Attack();
        yield return new WaitForSeconds(0.5f);

        //enemies attack player
        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;

            EnemyAttack atk = e.GetComponent<EnemyAttack>();
            if (atk != null)
            {
                atk.Attack();
                yield return new WaitForSeconds(0.5f);
            }

        }

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
            //unimplemented
        }
    }
}
