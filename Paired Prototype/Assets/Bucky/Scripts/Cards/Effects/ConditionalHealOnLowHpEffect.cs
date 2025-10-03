using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/ConditionalHealOnLowHp")]
public class ConditionalHealOnLowHpEffect : CardEffect
{
    public int damageToDeal = 10;
    public int healAmount = 10;
    [Range(0f,1f)] public float hpThreshold = 0.5f; // 0.5 = 50%

    public override bool RequiresTarget => true;

    private void OnValidate()
    {
        effectType = EffectType.Neutral;
        int pct = Mathf.RoundToInt(hpThreshold * 100);
        description = $"Deal {damageToDeal}. If HP < {pct}%, Heal {healAmount}";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;
        if (target != null) target.TakeDamage(damageToDeal);
        float ratio = player.maxHealth > 0 ? (float)player.currentHealth / player.maxHealth : 0f;
        if (ratio < hpThreshold)
        {
            player.Heal(healAmount);
        }
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.healAmount = healAmount + 2;
        return clone;
    }
}


