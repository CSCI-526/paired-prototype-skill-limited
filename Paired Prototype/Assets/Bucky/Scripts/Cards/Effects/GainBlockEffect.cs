using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/GainBlock")]
public class GainBlockEffect : CardEffect
{
    public int block;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Gain {block} block";
    }

    public override void Execute(Health player, Health target)
    {
        player?.GainBlock(block);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.block = Mathf.Max(1, block + 2);
        clone.description = $"Gain {clone.block} block";
        return clone;
    }
}
