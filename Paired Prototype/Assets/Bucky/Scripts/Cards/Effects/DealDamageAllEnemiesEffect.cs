using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/DealDamageAllEnemies")]
public class DealDamageAllEnemiesEffect : CardEffect
{
    public int damage;

    private void OnEnable()
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
                h.TakeDamage(damage + bonus);
            }
        }
    }
}


