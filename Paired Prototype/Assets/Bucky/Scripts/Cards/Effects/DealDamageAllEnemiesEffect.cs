using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/DealDamageAllEnemies")]
public class DealDamageAllEnemiesEffect : CardEffect
{
    public int damage;

    private void OnValidate()
    {
        effectType = EffectType.Positive;
        description = $"Deal {damage} damage to all enemies";
    }

    public override void Execute(Health player, Health target)
    {
        int bonus = player != null ? player.power : 0;
        int final = Mathf.Max(0, damage + bonus);

        var playerGO = UnityEngine.GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            var pa = playerGO.GetComponent<PlayerAttack>();
            if (pa != null)
            {
                pa.DealAoeWithAnimation(final);
                return;
            }
        }

        // Fallback
        var enemies = UnityEngine.GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;
            var h = e.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(final);
            }
        }
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.damage = Mathf.Max(0, damage + 2);
        clone.description = $"Deal {clone.damage} damage to all enemies";
        return clone;
    }
}


