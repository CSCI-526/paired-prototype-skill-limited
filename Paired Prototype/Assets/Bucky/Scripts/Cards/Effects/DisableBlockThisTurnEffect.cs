using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/DisableBlockThisTurn")]
public class DisableBlockThisTurnEffect : CardEffect
{
    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = "Cannot gain Block this turn";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;
        player.blockDisabledThisTurn = true;
    }

    public override bool SupportsUpgrade => false;
}


