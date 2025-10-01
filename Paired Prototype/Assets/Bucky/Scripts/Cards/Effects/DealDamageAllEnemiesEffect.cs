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
        var enemies = UnityEngine.GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            if (!e.activeInHierarchy) continue;
            var h = e.GetComponent<Health>();
            if (h != null)
            {
                int final = Mathf.Max(0, damage + bonus);
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


