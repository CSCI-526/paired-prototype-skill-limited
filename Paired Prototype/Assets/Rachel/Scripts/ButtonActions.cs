using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void OnPlayCardClick()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Try to play a card from the deck instead of the temp attack
        var deck = DeckService.Instance != null ? DeckService.Instance.Deck : null;
        if (deck != null && deck.Count > 0)
        {
            var playerHealth = player != null ? player.GetComponent<Health>() : null;
            Health targetHealth = null;
            if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
                targetHealth = SelectManager.Instance.Current.GetComponent<Health>();

            // Play the first card for now
            var card = deck[0];
            card.Play(playerHealth, targetHealth);
        }
        else
        {
            // Fallback to temp player attack if no cards
            PlayerAttack p_atk = player.GetComponent<PlayerAttack>();
            p_atk.Attack();
        }

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
            //unimplemented
        }
    }
}
