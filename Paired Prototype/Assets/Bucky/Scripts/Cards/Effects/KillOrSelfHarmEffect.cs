using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/KillOrSelfHarmChance")]
public class KillOrSelfHarmEffect : CardEffect
{
    [Range(0f, 1f)] public float killChance = 0.5f;
    public bool targetRequired = true;

    private void OnValidate()
    {
        effectType = EffectType.Neutral;
        description = $"{Mathf.RoundToInt(killChance * 100)}% kill target, else take half HP";
    }

    public override bool RequiresTarget => true;

    public override void Execute(Health player, Health target)
    {
        if (player == null) return;

        if (Random.value <= killChance && target != null)
        {
            target.TakeDamage(99999); // kill
        }
        else
        {
            int half = Mathf.Max(0, player.currentHealth / 2);
            player.TakeDamage(half);
        }
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.killChance = Mathf.Clamp01(killChance + 0.1f);
        clone.description = $"{Mathf.RoundToInt(clone.killChance * 100)}% kill target, else take half HP";
        return clone;
    }
}


