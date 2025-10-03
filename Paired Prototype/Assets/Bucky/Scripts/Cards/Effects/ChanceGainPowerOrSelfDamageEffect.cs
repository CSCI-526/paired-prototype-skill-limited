using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/ChanceGainPowerOrSelfDamage")]
public class ChanceGainPowerOrSelfDamageEffect : CardEffect
{
    [Range(0f,1f)] public float gainChance = 0.5f;
    public int powerAmount = 2;
    public int selfDamage = 16;

    private void OnValidate()
    {
        effectType = EffectType.Neutral;
        description = $"{Mathf.RoundToInt(gainChance*100)}% gain {powerAmount} Power, else take {selfDamage}";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;
        if (Random.value <= gainChance)
        {
            player.GainPower(powerAmount);
        }
        else
        {
            player.TakeDamage(selfDamage);
        }
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.gainChance = Mathf.Clamp01(gainChance + 0.1f);
        clone.description = $"{Mathf.RoundToInt(clone.gainChance*100)}% gain {clone.powerAmount} Power, else take {clone.selfDamage}";
        return clone;
    }
}


