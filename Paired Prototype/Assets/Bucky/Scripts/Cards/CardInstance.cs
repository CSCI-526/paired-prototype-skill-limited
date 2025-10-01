using System.Collections.Generic;

[System.Serializable]
public class CardInstance
{
    public CardData baseData;
    private List<CardEffect> effects = new List<CardEffect>();

    public CardInstance(CardData data)
    {
        baseData = data;
        effects.AddRange(data.baseEffects);
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

    public string GetDisplayName()
    {
        return baseData != null ? baseData.cardName : "Card";
    }
}
