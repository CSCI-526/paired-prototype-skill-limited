using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/EndTurnLock")]
public class EndTurnLockEffect : CardEffect
{
    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = $"Cannot play other cards this round";
    }

    public override void Execute(Health player, Health target)
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.LockPlayForThisTurn();
    }

    // Not upgradable: qualitative effect
    public override bool SupportsUpgrade => false;
}
