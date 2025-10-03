using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/MultiHitDamage")]
public class MultiHitDamageEffect : CardEffect
{
    public int damagePerHit = 1;
    public int hits = 2;

    public override bool RequiresTarget => true;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Deal {damagePerHit} x {hits} to an enemy";
    }

    public override void Execute(Health player, Health target)
    {
        if (target == null) return;
        int bonus = player != null ? player.power : 0;
        int perHit = Mathf.Max(0, damagePerHit + bonus);

        for (int i = 0; i < hits; i++)
        {
            target.TakeDamage(perHit);
        }
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.damagePerHit = Mathf.Max(0, damagePerHit + 2);
        clone.description = $"Deal {clone.damagePerHit} x {hits} to an enemy";
        return clone;
    }
}


