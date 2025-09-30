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
}
