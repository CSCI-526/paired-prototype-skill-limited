using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Base damage only used for fallback/manual tests; cards drive real damage
    public int damage = 5;

    [Header("Animation")]
    public float lungeDistance = 40.0f;
    public float lungeSpeed = 5.0f;
    public Vector3 damagePopupOffset = new Vector3(30.0f, 90.0f, 0f);
    private Health playerHealth;
    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    private IEnumerator AttackLungeHit(Transform target, int displayDamage, Action onHit)
    {
        if (target == null)
        {
            onHit?.Invoke();
            yield break;
        }

        Vector3 start = player.transform.localPosition;
        Vector3 forward = start + new Vector3(lungeDistance, 0, 0);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            player.transform.localPosition = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        onHit?.Invoke();
        if (displayDamage >= 0)
            DamageInit.I.Show(displayDamage, target.position + damagePopupOffset);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 5;
            player.transform.localPosition = Vector3.Lerp(forward, start, t);
            yield return null;
        }
    }

    public void Attack()
    {
        if (playerHealth.currentHealth <= 0) return;

        // Determine target (if any)
        Transform targetTransform = null;
        Health enemyHealth = null;
        if (SelectManager.Instance != null && SelectManager.Instance.Current != null)
        {
            targetTransform = SelectManager.Instance.Current.transform;
            enemyHealth = SelectManager.Instance.Current.GetComponent<Health>();
        }

        // Prefer selected hand card if available
        CardInstance selectedCard = HandSelectionManager.Instance != null ? HandSelectionManager.Instance.Selected : null;
        if (selectedCard != null)
        {
            // Execute card effects (handles damage, block, power, AOE, etc.)
            selectedCard.Play(playerHealth, enemyHealth);

            // Remove played card from hand (destroy its GO) and clear selection
            if (HandSelectionManager.Instance != null)
            {
                var currentSelectable = HandSelectionManager.Instance.Current;
                GameObject toDestroy = currentSelectable != null ? currentSelectable.gameObject : null;
                HandSelectionManager.Instance.Clear();
                if (toDestroy != null) Destroy(toDestroy);
            }
            return;
        }

        // Fallback: basic attack using configured damage, applying player's power
        if (targetTransform != null)
        {
            int finalDamage = Mathf.Max(0, damage + (playerHealth != null ? playerHealth.power : 0));
            StartCoroutine(AttackLungeHit(targetTransform, finalDamage, () => enemyHealth?.TakeDamage(finalDamage)));
        }
    }

    public void DealDamageWithAnimation(Health target, int amount)
    {
        if (target == null) return;
        int final = Mathf.Max(0, amount);
        StartCoroutine(AttackLungeHit(target.transform, final, () => target.TakeDamage(final)));
    }

    public void DealAoeWithAnimation(int amount)
    {
        int final = Mathf.Max(0, amount);
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform firstTarget = null;
        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;
            firstTarget = e.transform;
            break;
        }
        StartCoroutine(AttackLungeHit(firstTarget, -1, () =>
        {
            foreach (var e in enemies)
            {
                if (!e.activeInHierarchy) continue;
                var h = e.GetComponent<Health>();
                if (h != null)
                {
                    h.TakeDamage(final);
                    DamageInit.I.Show(final, e.transform.position + damagePopupOffset);
                }
            }
        }));
    }

    public void DealSelfDamageWithAnimation(int amount)
    {
        int final = Mathf.Max(0, amount);
        // No lunge; simply show popup and apply
        DamageInit.I.Show(final, player.transform.position + damagePopupOffset);
        playerHealth?.TakeDamage(final);
    }
}