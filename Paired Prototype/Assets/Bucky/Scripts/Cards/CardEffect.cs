using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public EffectType effectType = EffectType.Neutral;

    // Short, player-facing line, e.g. "Deal 6 damage" or "Lose 2 HP"
    [TextArea] public string description;

    // If true, the effect needs a specific target (e.g., single-target damage)
    public virtual bool RequiresTarget => false;

    // Execute in order when the card is played (wired to Rachel's systems)
    public abstract void Execute(Health player, Health target);
}
