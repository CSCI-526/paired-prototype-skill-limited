using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
    private bool isTurnRunning = false;
    public bool testMode = false; // Toggle in Inspector to skip enemy attacks during testing

    public int cardsPerHand = 3;
    [Header("Scenes")] public string rewardSceneName = "CardRewardScene";

    public void OnPlayCardClick()
    {
        if (isTurnRunning) return;
        StartCoroutine(PlayAttackSeq());
    }

    public void OnReshuffleClick()
    {
        if (isTurnRunning) return;

        Debug.Log("[Reshuffle] Clicked");

        // Deal 10 self-damage to the player
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerHealth = player != null ? player.GetComponent<Health>() : null;
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);
            Debug.Log($"[Reshuffle] Player took 10 damage. HP={playerHealth.currentHealth} Block={playerHealth.currentBlock}");
        }
        else
        {
            Debug.LogWarning("[Reshuffle] Player Health not found.");
        }

        // Clear existing hand visuals
        var handContainer = FindObjectOfType<CardHandSpawner>();
        if (handContainer == null)
        {
            Debug.LogWarning("[Reshuffle] No CardHandSpawner found in scene.");
        }
        if (handContainer != null && handContainer.container != null)
        {
            for (int i = handContainer.container.childCount - 1; i >= 0; i--)
            {
                var child = handContainer.container.GetChild(i);
                if (child == null) continue;
                if (child.GetComponent<CardDisplay>() != null || child.GetComponent<CardSelectable>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Optionally clear selection so no stale selection remains
        if (HandSelectionManager.Instance != null)
        {
            HandSelectionManager.Instance.Clear();
        }

        // Draw a fresh hand from the current deck
        if (DeckService.Instance != null)
        {
            var picks = DeckService.Instance.PickRandomFromDeck(cardsPerHand);
            Debug.Log($"[Reshuffle] New hand ({picks.Count})");
            for (int i = 0; i < picks.Count; i++)
            {
                var c = picks[i];
                if (c == null) continue;
                Debug.Log($"  [{i}] {c.GetDisplayName()}");
            }
            if (handContainer != null)
            {
                handContainer.SpawnSpecific(picks);
            }
        }
        else
        {
            Debug.LogWarning("[Reshuffle] DeckService not available.");
        }
    }

    private IEnumerator PlayAttackSeq()
    {
        isTurnRunning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerAttack p_atk = player.GetComponent<PlayerAttack>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Prefer selected hand card if available
        var selected = HandSelectionManager.Instance != null ? HandSelectionManager.Instance.Selected : null;
        if (selected != null)
        {
            var playerHealth = player != null ? player.GetComponent<Health>() : null;
            Health targetHealth = null;
            if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
                targetHealth = SelectManager.Instance.Current.GetComponent<Health>();

            Debug.Log($"[Play] {selected.GetDisplayName()} targeting {(targetHealth != null ? targetHealth.name : "none")}");

            selected.Play(playerHealth, targetHealth);

            // Post-state logs for validation
            if (playerHealth != null)
                Debug.Log($"[Post] Player HP={playerHealth.currentHealth} Block={playerHealth.currentBlock} Power={playerHealth.power}");
            if (targetHealth != null)
                Debug.Log($"[Post] Target {targetHealth.name} HP={targetHealth.currentHealth}");
            else
            {
                // If AOE, print each enemy
                foreach (var e in enemies)
                {
                    if (!e.activeInHierarchy) continue;
                    var eh = e.GetComponent<Health>();
                    if (eh != null) Debug.Log($"[Post] Enemy {e.name} HP={eh.currentHealth}");
                }
            }

            // Remove played card from hand (destroy its GO) and clear selection
            if (HandSelectionManager.Instance != null)
            {
                var currentSelectable = HandSelectionManager.Instance.Current;
                if (currentSelectable != null)
                {
                    GameObject toDestroy = currentSelectable.gameObject;
                    HandSelectionManager.Instance.Clear();
                    if (toDestroy != null) Destroy(toDestroy);
                }
                else
                {
                    HandSelectionManager.Instance.Clear();
                }
            }
        }
        else
        {
            // Try to play a card from deck as fallback
            var deck = DeckService.Instance != null ? DeckService.Instance.Deck : null;
            if (deck != null && deck.Count > 0)
            {
                var playerHealth = player != null ? player.GetComponent<Health>() : null;
                Health targetHealth = null;
                if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
                    targetHealth = SelectManager.Instance.Current.GetComponent<Health>();

                var card = deck[0];
                Debug.Log($"[Play] fallback {card.GetDisplayName()}");
                foreach (var eff in card.Effects)
                {
                    if (eff == null) continue;
                    Debug.Log($"  - {eff.description}");
                }
                card.Play(playerHealth, targetHealth);
            }
            else
            {
                // No selected card and deck empty: no action
                Debug.LogWarning("[Play] No cards available to play.");
                yield return new WaitForSeconds(0.2f);
            }
        }

        if (!testMode)
        {
            //enemies attack player
            foreach (var e in enemies)
            {
                if (!e.activeInHierarchy) continue;

                EnemyAttack atk = e.GetComponent<EnemyAttack>();
                if (atk != null)
                    atk.Attack();
            }
        }

        //check if all enemies are dead
        StartCoroutine(CheckAllEnemiesDead());

        // mark turn sequence finished so Play button can be used again
        isTurnRunning = false;
    }

    public void OnEndTurnClick()
    {
        Debug.Log("[EndTurn] Clicked");
        // Enemies attack now (end of player turn)
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerHealth = player != null ? player.GetComponent<Health>() : null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        StartCoroutine(AllEnemiesAttack());
        StartCoroutine(CheckAllEnemiesDead());

        // Start next turn: reset blocks etc.
        if (TurnManager.Instance != null) TurnManager.Instance.NextTurn();

        // Clear any remaining hand cards in UI
        var handContainer = FindObjectOfType<CardHandSpawner>();
        if (handContainer == null) Debug.LogWarning("[EndTurn] No CardHandSpawner found in scene.");
        if (handContainer != null && handContainer.container != null)
        {
            // Only destroy spawned card visuals, keep other UI/spawner objects intact
            for (int i = handContainer.container.childCount - 1; i >= 0; i--)
            {
                var child = handContainer.container.GetChild(i);
                if (child == null) continue;
                if (child.GetComponent<CardDisplay>() != null || child.GetComponent<CardSelectable>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Draw a new hand (random 3) from current deck
        if (DeckService.Instance != null)
        {
            var picks = DeckService.Instance.PickRandomFromDeck(cardsPerHand);
            // Log the new hand contents
            Debug.Log($"[Hand] New hand ({picks.Count})");
            for (int i = 0; i < picks.Count; i++)
            {
                var c = picks[i];
                if (c == null) continue;
                Debug.Log($"  [{i}] {c.GetDisplayName()}");
            }
            if (handContainer != null)
            {
                // Use spawner to render these specific cards
                handContainer.SpawnSpecific(picks);
            }
        }
    }

    private IEnumerator AllEnemiesAttack()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var e in enemies)
        {
            //if (!e.activeInHierarchy) continue;
            EnemyAttack atk = e.GetComponent<EnemyAttack>();
            if (atk != null)
            {
                Debug.Log("[Enemy] Atacked");
                atk.Attack();
                yield return new WaitForSeconds(0.5f);
            }
        }
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
            if (!string.IsNullOrEmpty(rewardSceneName))
            {
                // Only load if the scene is in Build Settings; otherwise log an error once
                if (IsSceneInBuildSettings(rewardSceneName))
                {
                    SceneManager.LoadScene(rewardSceneName);
                }
                else
                {
                    Debug.LogError($"[Scene] '{rewardSceneName}' not in Build Settings. Add it via File -> Build Settings -> Scenes In Build.");
                }
            }
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            if (!string.IsNullOrEmpty(path))
            {
                var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                if (string.Equals(filename, sceneName)) return true;
            }
        }
        return false;
    }
}
