using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/DealDamage")]
public class DealDamageEffect : CardEffect
{
    public int damage;

    private void OnEnable()
    {
        effectType = EffectType.Positive;
        description = $"Deal {damage} damage to an enemy";
    }

    public override void Execute(Health player, Health target)
    {
        int bonus = player != null ? player.power : 0;
        target?.TakeDamage(damage + bonus);
    }
}
