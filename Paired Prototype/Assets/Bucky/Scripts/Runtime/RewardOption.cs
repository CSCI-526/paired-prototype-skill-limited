using UnityEngine;

// Component attached to each reward card object, describing what action it represents
public class RewardOption : MonoBehaviour
{
    public enum RewardType { NewCard, Upgrade }

    public RewardType type;

    // For NewCard
    public CardData newCardData;

    // For Upgrade
    public CardInstance originalInstance;     // reference into DeckService
    public CardEffect oldEffectInOriginal;    // to identify which one we previewed
    public CardEffect upgradedEffectPreview;  // cloned ScriptableObject applied on continue

    public void ConfigureAsNewCard(CardData data)
    {
        type = RewardType.NewCard;
        newCardData = data;
    }

    public void ConfigureAsUpgrade(CardInstance original, CardEffect oldEff, CardEffect newEff)
    {
        type = RewardType.Upgrade;
        originalInstance = original;
        oldEffectInOriginal = oldEff;
        upgradedEffectPreview = newEff;
    }

    public void Apply()
    {
        if (DeckService.Instance == null) return;
        switch (type)
        {
            case RewardType.NewCard:
                if (newCardData != null)
                    DeckService.Instance.AddNewCard(newCardData);
                break;
            case RewardType.Upgrade:
                if (originalInstance != null)
                {
                    // Stage-based upgrade: advance to next stage
                    DeckService.Instance.UpgradeCardStage(originalInstance);
                }
                break;
        }
    }
}


