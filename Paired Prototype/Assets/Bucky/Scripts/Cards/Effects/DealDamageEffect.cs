using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/DealDamage")]
public class DealDamageEffect : CardEffect
{
    public int damage;

    public override bool RequiresTarget => true;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Deal {damage} damage to an enemy";
    }

    public override void Execute(Health player, Health target)
    {
        int bonus = player != null ? player.power : 0;
        int final = Mathf.Max(0, damage + bonus);
        target?.TakeDamage(final);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.damage = Mathf.Max(0, damage + 2);
        clone.description = $"Deal {clone.damage} damage to an enemy";
        return clone;
    }
}
