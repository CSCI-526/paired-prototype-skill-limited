using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{

    private bool isTurnRunning = false;

    public void OnPlayCardClick()
    {
        if (isTurnRunning) return;
        StartCoroutine(PlayAttackSeq());
    }

    private IEnumerator PlayAttackSeq()
    {
        isTurnRunning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack p_atk = player.GetComponent<PlayerAttack>();

        //temp player movement
        p_atk.Attack();
        yield return new WaitForSeconds(0.5f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //enemies attack player
        foreach (var e in enemies)
        {
            //if (!e.activeInHierarchy) continue;

            EnemyAttack atk = e.GetComponent<EnemyAttack>();
            if (atk != null)
            {
                atk.Attack();
                yield return new WaitForSeconds(0.5f);
            }

        }

        StartCoroutine(CheckAllEnemiesDead());

        isTurnRunning = false;
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
