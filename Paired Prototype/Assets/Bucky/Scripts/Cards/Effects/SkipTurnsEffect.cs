using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/SkipTurns")]
public class SkipTurnsEffect : CardEffect
{
    public int turns = 1;

    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = turns == 1 ? "Skip next turn" : $"Skip next {turns} turns";
    }

    public override void Execute(Health player, Health target)
    {
        if (TurnManager.Instance != null && turns > 0)
            TurnManager.Instance.AddSkipTurns(turns);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.turns = Mathf.Max(0, turns - 1);
        clone.description = clone.turns == 1 ? "Skip next turn" : (clone.turns == 0 ? "No skip" : $"Skip next {clone.turns} turns");
        return clone;
    }
}


