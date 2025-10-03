using System.Collections.Generic;

[System.Serializable]
public class CardInstance
{
    public CardData baseData;
    private List<CardEffect> effects = new List<CardEffect>();

    public enum CardStage
    {
        Cursed = 0,
        Purified = 1,
        Blessed = 2
    }

    public CardStage Stage { get; private set; } = CardStage.Cursed;

    public CardInstance(CardData data)
    {
        baseData = data;
        LoadStageEffects();
    }

    // Build from explicit effects list (used for preview/clone scenarios)
    public CardInstance(CardData data, IEnumerable<CardEffect> effectsOverride)
    {
        baseData = data;
        effects = new List<CardEffect>();
        if (effectsOverride != null) effects.AddRange(effectsOverride);
    }

    public void Play(Health player, Health target)
    {
        if (TurnManager.Instance != null && TurnManager.Instance.IsPlayLocked) return;
        foreach (var e in effects)
            e.Execute(player, target);
    }

    // Upgrade/curses = swap one effect
    public void ReplaceEffect(CardEffect oldEffect, CardEffect newEffect)
    {
        int idx = effects.IndexOf(oldEffect);
        if (idx >= 0) effects[idx] = newEffect;
    }

    public IEnumerable<CardEffect> Effects => effects;

    public void AddEffect(CardEffect newEffect)
    {
        if (newEffect != null) effects.Add(newEffect);
    }

    private void LoadStageEffects()
    {
        effects.Clear();
        if (baseData == null)
            return;

        // Prefer staged arrays; fallback to legacy baseEffects
        CardEffect[] chosen = null;
        switch (Stage)
        {
            case CardStage.Cursed:
                chosen = baseData.cursedEffects != null && baseData.cursedEffects.Length > 0 ? baseData.cursedEffects : baseData.baseEffects;
                break;
            case CardStage.Purified:
                chosen = baseData.purifiedEffects != null && baseData.purifiedEffects.Length > 0 ? baseData.purifiedEffects : baseData.baseEffects;
                break;
            case CardStage.Blessed:
                chosen = baseData.blessedEffects != null && baseData.blessedEffects.Length > 0 ? baseData.blessedEffects : baseData.baseEffects;
                break;
        }
        if (chosen != null) effects.AddRange(chosen);
    }

    public void SetStage(CardStage stage)
    {
        Stage = stage;
        LoadStageEffects();
    }

    public void UpgradeStage()
    {
        if (Stage == CardStage.Cursed) SetStage(CardStage.Purified);
        else if (Stage == CardStage.Purified) SetStage(CardStage.Blessed);
    }

    public bool IsMaxStage => Stage == CardStage.Blessed;

    public CardStage GetNextStage()
    {
        if (Stage == CardStage.Cursed) return CardStage.Purified;
        if (Stage == CardStage.Purified) return CardStage.Blessed;
        return CardStage.Blessed;
    }

    public string GetDisplayName()
    {
        return baseData != null ? baseData.cardName : "Card";
    }

    public CardRarity GetRarity()
    {
        return baseData != null ? baseData.rarity : CardRarity.Common;
    }
}
