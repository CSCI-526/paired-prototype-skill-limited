using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardFlowController : MonoBehaviour
{
    [Header("Scene Refs")]
    public Transform newCardArea;      // container with one slot
    public Transform upgradeArea;      // container with three slots
    public GameObject cardPrefab;      // Assets/Bucky/Prefabs/Cards/Card.prefab
    public Button continueButton;      // wired to call RewardCardSelector.ConfirmSelection

    [Header("Reward Pools")] 
    public CardData[] newCardPool;     // assign in Inspector

    [Tooltip("Multiplier when enhancing positive effects (>=1). 1.5 = +50%.")]
    public float positiveMultiplier = 1.5f;
    [Tooltip("Multiplier when decreasing negative effects (0..1). 0.5 = -50%.")]
    public float negativeMultiplier = 0.5f;

    void Start()
    {
        // Defensive: clear any selection carried over
        if (HandSelectionManager.Instance != null)
            HandSelectionManager.Instance.Clear();

        GenerateNewCardOption();
        GenerateUpgradeOptions();

        // Ensure Continue starts disabled until selection
        if (continueButton != null)
            continueButton.interactable = false;
    }

    private void GenerateNewCardOption()
    {
        if (newCardArea == null || cardPrefab == null) return;

        // Clean container
        for (int i = newCardArea.childCount - 1; i >= 0; i--)
            Destroy(newCardArea.GetChild(i).gameObject);

        if (newCardPool == null || newCardPool.Length == 0)
        {
            Debug.LogWarning("[Reward] newCardPool is empty; New Card option disabled");
            return;
        }

        var data = newCardPool[Random.Range(0, newCardPool.Length)];
        var inst = new CardInstance(data);

        var go = Instantiate(cardPrefab, newCardArea);
        go.name = data != null ? $"Reward_New_{data.cardName}" : "Reward_NewCard";
        var display = go.GetComponent<CardDisplay>();
        display.Init(inst);

        var selectable = go.GetComponent<CardSelectable>();
        if (selectable == null) selectable = go.AddComponent<CardSelectable>();
        selectable.Initialize(inst);

        var opt = go.AddComponent<RewardOption>();
        opt.ConfigureAsNewCard(data);
    }

    private void GenerateUpgradeOptions()
    {
        if (upgradeArea == null || cardPrefab == null) return;

        // Clean container
        for (int i = upgradeArea.childCount - 1; i >= 0; i--)
            Destroy(upgradeArea.GetChild(i).gameObject);

        var deck = DeckService.Instance != null ? DeckService.Instance.Deck : null;
        if (deck == null || deck.Count == 0)
        {
            Debug.Log("[Reward] Deck empty, hiding upgrade row");
            upgradeArea.gameObject.SetActive(false);
            return;
        }

        var picks = DeckService.Instance.PickRandomFromDeck(3);

        for (int i = 0; i < picks.Count; i++)
        {
            var original = picks[i];
            if (original == null) continue;

            // 50% prefer enhancing positive, 50% prefer weakening negative
            bool preferPositive = Random.value < 0.5f;

            if (!TryBuildUpgradePreview(original, preferPositive, out var preview, out var oldEff, out var newEff))
            {
                // Retry with opposite type if first choice failed
                if (!TryBuildUpgradePreview(original, !preferPositive, out preview, out oldEff, out newEff))
                {
                    Debug.LogWarning($"[Reward] Could not build upgrade preview for {original.GetDisplayName()}");
                    continue;
                }
            }

            var go = Instantiate(cardPrefab, upgradeArea);
            go.name = $"Reward_Upgrade_{original.GetDisplayName()}_{i}";
            var display = go.GetComponent<CardDisplay>();
            display.Init(preview);

            var selectable = go.GetComponent<CardSelectable>();
            if (selectable == null) selectable = go.AddComponent<CardSelectable>();
            selectable.Initialize(preview);

            var opt = go.AddComponent<RewardOption>();
            opt.ConfigureAsUpgrade(original, oldEff, newEff);
        }
    }

    private bool TryBuildUpgradePreview(
        CardInstance original,
        bool preferPositive,
        out CardInstance preview,
        out CardEffect oldEffect,
        out CardEffect upgradedEffect)
    {
        preview = null;
        oldEffect = null;
        upgradedEffect = null;

        var all = new List<CardEffect>(original.Effects);
        if (all.Count == 0) return false;

        List<CardEffect> positives = new List<CardEffect>();
        List<CardEffect> negatives = new List<CardEffect>();

        foreach (var e in all)
        {
            if (e == null) continue;
            if (e.effectType == EffectType.Positive) positives.Add(e);
            else if (e.effectType == EffectType.Negative) negatives.Add(e);
        }

        List<CardEffect> pool = preferPositive ? positives : negatives;
        if (pool.Count == 0) pool = preferPositive ? negatives : positives; // fallback
        if (pool.Count == 0) return false;

        // If decreasing negatives, avoid non-magnitude effects (like EndTurnLock)
        CardEffect chosen = null;
        for (int guard = 0; guard < 10 && chosen == null; guard++)
        {
            var candidate = pool[Random.Range(0, pool.Count)];
            if (!preferPositive && !IsMagnitudeNegativeEffect(candidate))
                continue;
            chosen = candidate;
        }
        if (chosen == null) return false;

        CardEffect upgraded = UpgradeEffectMagnitude(chosen, preferPositive);
        if (upgraded == null) return false;

        // Build preview list replacing the one effect
        var list = new List<CardEffect>(all);
        int idx = list.IndexOf(chosen);
        list[idx] = upgraded;
        preview = new CardInstance(original.baseData, list);

        oldEffect = chosen;
        upgradedEffect = upgraded;
        return true;
    }

    private bool IsMagnitudeNegativeEffect(CardEffect e)
    {
        return e is SelfDamageEffect || e is LosePowerEffect;
    }

    private CardEffect UpgradeEffectMagnitude(CardEffect original, bool increasePositive)
    {
        // Clone and adjust known types
        if (original is DealDamageEffect dmg)
        {
            var clone = ScriptableObject.Instantiate(dmg);
            clone.damage = Mathf.Max(0, Mathf.CeilToInt(dmg.damage * positiveMultiplier));
            clone.description = $"Deal {clone.damage} damage to an enemy";
            return clone;
        }
        if (original is DealDamageAllEnemiesEffect aoe)
        {
            var clone = ScriptableObject.Instantiate(aoe);
            clone.damage = Mathf.Max(0, Mathf.CeilToInt(aoe.damage * positiveMultiplier));
            clone.description = $"Deal {clone.damage} damage to all enemies";
            return clone;
        }
        if (original is GainBlockEffect blk)
        {
            var clone = ScriptableObject.Instantiate(blk);
            clone.block = Mathf.Max(1, Mathf.CeilToInt(blk.block * positiveMultiplier));
            clone.description = $"Gain {clone.block} block";
            return clone;
        }
        if (original is GainPowerEffect pow)
        {
            var clone = ScriptableObject.Instantiate(pow);
            clone.amount = Mathf.Max(1, Mathf.CeilToInt(pow.amount * positiveMultiplier));
            clone.description = $"Gain {clone.amount} power";
            return clone;
        }
        if (original is HealEffect heal)
        {
            var clone = ScriptableObject.Instantiate(heal);
            clone.amount = Mathf.Max(1, Mathf.CeilToInt(heal.amount * positiveMultiplier));
            clone.description = $"Heal {clone.amount} HP";
            return clone;
        }

        // Negative magnitude reductions
        if (original is SelfDamageEffect selfNeg)
        {
            var clone = ScriptableObject.Instantiate(selfNeg);
            clone.damage = Mathf.Max(0, Mathf.FloorToInt(selfNeg.damage * negativeMultiplier));
            clone.description = $"Deal {clone.damage} damage to yourself";
            return clone;
        }
        if (original is LosePowerEffect lose)
        {
            var clone = ScriptableObject.Instantiate(lose);
            clone.amount = Mathf.Max(0, Mathf.FloorToInt(lose.amount * negativeMultiplier));
            clone.description = $"Lose {clone.amount} power";
            return clone;
        }

        // If we got here, either a neutral effect or unsupported type
        Debug.LogWarning($"[Reward] Unsupported effect type for upgrade: {original.GetType().Name}");
        return null;
    }
}


