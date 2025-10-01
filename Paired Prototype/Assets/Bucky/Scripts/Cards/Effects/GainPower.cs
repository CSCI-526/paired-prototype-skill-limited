using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/GainPower")]
public class GainPowerEffect : CardEffect
{
    public int amount;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Gain {amount} power";
    }

    public override void Execute(Health player, Health target)
    {
        player?.GainPower(amount);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.amount = Mathf.Max(1, amount + 1);
        clone.description = $"Gain {clone.amount} power";
        return clone;
    }
}
