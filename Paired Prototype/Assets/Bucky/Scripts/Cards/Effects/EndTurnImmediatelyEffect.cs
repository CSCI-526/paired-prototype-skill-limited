using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/EndTurnImmediately")]
public class EndTurnImmediatelyEffect : CardEffect
{
    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = "End turn immediately";
    }

    public override void Execute(Health player, Health target)
    {
        // Try to route through ButtonActions to trigger enemy attack & hand refresh
        var ba = Object.FindObjectOfType<ButtonActions>();
        if (ba != null)
        {
            ba.OnEndTurnClick();
            return;
        }

        // Fallback: just advance the turn
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.NextTurn();
        }
    }

    public override bool SupportsUpgrade => false;
}


