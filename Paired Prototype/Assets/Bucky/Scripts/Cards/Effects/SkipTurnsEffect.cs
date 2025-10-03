using UnityEngine;

[CreateAssetMenu(menuName = "CardEffects/SkipTurns")]
public class SkipTurnsEffect : CardEffect
{
    public int turns = 1;

    private void OnEnable()
    {
        effectType = EffectType.Negative;
        description = turns == 1 ? "Skip next turn" : $"Skip next {turns} turns";
    }

    public override void Execute(Health player, Health target)
    {
		// Prefer to immediately end the current and subsequent turns via UI flow
		if (turns > 0)
		{
			var ui = GameObject.FindObjectOfType<ButtonActions>();
			if (ui != null)
			{
				for (int i = 0; i < turns; i++)
				{
					ui.OnEndTurnClick();
				}
				return;
			}
		}

		// Fallback: mark turns to be skipped by the turn manager
		if (TurnManager.Instance != null && turns > 0)
			TurnManager.Instance.AddSkipTurns(turns);
    }

    public override bool SupportsUpgrade => true;

    public override CardEffect CreateUpgradedCopy()
    {
        var clone = ScriptableObject.Instantiate(this);
        clone.turns = Mathf.Max(0, turns - 1);
        clone.description = clone.turns == 1 ? "Skip next turn" : (clone.turns == 0 ? "No skip" : $"Skip next {clone.turns} turns");
        return clone;
    }
}


