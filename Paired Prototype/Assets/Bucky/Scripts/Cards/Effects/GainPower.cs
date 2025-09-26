using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/GainPower")]
public class GainPowerEffect : CardEffect
{
    public int amount;

    private void OnEnable()
    {
        effectType = EffectType.Positive;
        description = $"Gain {amount} power";
    }

    public override void Execute(Health player, Health target)
    {
        player?.GainPower(amount);
    }
}
