using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/SelfDamage")]
public class SelfDamageEffect : CardEffect
{
    public int damage;

    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = $"Deal {damage} damage to yourself";
    }

    public override void Execute(Health player, Health target)
    {
        player?.TakeDamage(damage);
    }
}
