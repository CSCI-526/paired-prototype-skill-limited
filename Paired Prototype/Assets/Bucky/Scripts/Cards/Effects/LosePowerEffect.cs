using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/LosePower")]
public class LosePowerEffect : CardEffect
{
    public int amount;

    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = $"Lose {amount} power";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;
        player.power -= amount;
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.amount = Mathf.Max(0, amount - 1);
        clone.description = $"Lose {clone.amount} power";
        return clone;
    }
}
