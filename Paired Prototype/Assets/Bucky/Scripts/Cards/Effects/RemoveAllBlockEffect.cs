using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/RemoveAllBlock")]
public class RemoveAllBlockEffect : CardEffect
{
    public bool affectOnlyPlayer = true;

    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = affectOnlyPlayer ? "Lose all Block" : "Target loses all Block";
    }

    public override void Execute(Health player, Health target)
    {
        var who = affectOnlyPlayer ? player : target;
        if (who == null) return;
        who.currentBlock = 0;
    }

    public override bool SupportsUpgrade => false;
}


