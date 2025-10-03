using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/SelfDamage")]
public class SelfDamageEffect : CardEffect
{
    public int damage;

    private void OnValidate()
    {
        effectType = EffectType.Negative;
        description = $"Deal {damage} damage to yourself";
    }

    public override void Execute(Health player, Health target)
    {
        if (player == null)
        {
            return;
        }

        var playerGO = UnityEngine.GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            var pa = playerGO.GetComponent<PlayerAttack>();
            if (pa != null)
            {
                pa.DealSelfDamageWithAnimation(damage);
                return;
            }
        }

        // Fallback
        player.TakeDamage(damage);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        // Upgrading a negative effect makes it less punishing
        var clone = ScriptableObject.Instantiate(this);
        clone.damage = Mathf.Max(0, damage - 2);
        clone.description = $"Deal {clone.damage} damage to yourself";
        return clone;
    }
}
