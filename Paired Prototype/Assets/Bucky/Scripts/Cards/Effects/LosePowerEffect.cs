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
        if (player.power < 0) player.power = 0;
    }
}
