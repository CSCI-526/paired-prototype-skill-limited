using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/Heal")]
public class HealEffect : CardEffect
{
    public int amount;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Heal {amount} HP";
    }

    public override void Execute(Health player, Health target)
    {
        player?.Heal(amount);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.amount = Mathf.Max(1, amount + 3);
        clone.description = $"Heal {clone.amount} HP";
        return clone;
    }
}
