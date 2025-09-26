using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/GainBlock")]
public class GainBlockEffect : CardEffect
{
    public int block;

    private void OnEnable()
    {
        effectType = EffectType.Positive;
        description = $"Gain {block} block";
    }

    public override void Execute(Health player, Health target)
    {
        player?.GainBlock(block);
    }
}
