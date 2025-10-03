using System.Collections.Generic;
using UnityEngine;

public class DeckService : MonoBehaviour
{
    public static DeckService Instance { get; private set; }

    // The player’s current deck (runtime copies)
    public List<CardInstance> Deck { get; private set; } = new();

    [Header("Config")]
    public float curseChancePerRound = 0.20f; // 20%

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Build a deck at run start from starting blueprints
    public void BuildStartingDeck(IEnumerable<CardData> startingCards)
    {
        Deck.Clear();
        foreach (var data in startingCards)
            Deck.Add(new CardInstance(data));
    }

    // Reward: add new card (wrap the SO in a CardInstance)
    public void AddNewCard(CardData data) => Deck.Add(new CardInstance(data));

    // Reward: apply an upgrade by adding a positive effect to a chosen instance
    public void ApplyUpgrade(CardInstance target, CardEffect positiveEffect)
    {
        if (target != null && positiveEffect != null && positiveEffect.effectType != EffectType.Negative)
            target.AddEffect(positiveEffect);
    }

    // Reward: replace an existing effect with an upgraded one
    public void ApplyUpgradeReplace(CardInstance target, CardEffect oldEffect, CardEffect upgradedEffect)
    {
        if (target == null || upgradedEffect == null) return;
        target.ReplaceEffect(oldEffect, upgradedEffect);
    }

    // Stage-based upgrades
    public void UpgradeCardStage(CardInstance target)
    {
        if (target == null) return;
        target.UpgradeStage();
    }

    public void SetCardStage(CardInstance target, CardInstance.CardStage stage)
    {
        if (target == null) return;
        target.SetStage(stage);
    }

    // Round end: maybe curse a random card by adding a negative effect
    public void MaybeApplyRandomCurse(CardEffect negativeEffect)
    {
        if (negativeEffect == null || negativeEffect.effectType != EffectType.Negative) return;
        if (Deck.Count == 0) return;
        if (Random.value > curseChancePerRound) return;

        int i = Random.Range(0, Deck.Count);
        Deck[i].AddEffect(negativeEffect);
        Debug.Log($"Cursed: {Deck[i].GetDisplayName()} with {negativeEffect.name}");
    }

    // Helper: pick N random cards from deck (for upgrade offers)
    public List<CardInstance> PickRandomFromDeck(int count)
    {
        var list = new List<CardInstance>(Deck);
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
        if (count < list.Count) list.RemoveRange(count, list.Count - count);
        return list;
    }
}
