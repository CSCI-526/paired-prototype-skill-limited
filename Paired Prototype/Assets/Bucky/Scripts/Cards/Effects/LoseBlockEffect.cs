using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/LoseBlock")]
public class LoseBlockEffect : CardEffect
{
    public int amount = 1;

    private void OnValidate()
    {
        effectType = EffectType.Negative;
        description = $"Lose {amount} Block";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;
        int loss = Mathf.Min(amount, player.currentBlock);
        player.currentBlock -= loss;
    }

    public override bool SupportsUpgrade => false;
}


