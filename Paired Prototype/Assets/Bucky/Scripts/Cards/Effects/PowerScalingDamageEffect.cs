using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/PowerScalingDamage")]
public class PowerScalingDamageEffect : CardEffect
{
    public int baseDamage = 0;
    public int perPower = 2;

    public override bool RequiresTarget => true;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Deal {baseDamage} + {perPower}xPower";
    }

    public override void Execute(Health player, Health target)
    {
        if (target == null) return;
        int pow = player != null ? player.power : 0;
        int final = Mathf.Max(0, baseDamage + (pow * perPower));
        target.TakeDamage(final);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.perPower = Mathf.Max(0, perPower + 1);
        clone.description = $"Deal {clone.baseDamage} + {clone.perPower}xPower";
        return clone;
    }
}


