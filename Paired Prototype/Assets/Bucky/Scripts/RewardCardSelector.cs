using UnityEngine;
using UnityEngine.UI;

public class RewardCardSelector : MonoBehaviour
{
    public static RewardCardSelector Instance;

    private GameObject selectedCard;
    [Header("Scene Names")] public string actionSceneName = "ActionScene";

    private void Awake()
    {
        Instance = this;
    }

    public void SelectCard(GameObject card)
    {
        if (selectedCard != null)
        {
            // remove highlight from old card
            Outline oldOutline = selectedCard.GetComponent<Outline>();
            if (oldOutline != null) oldOutline.enabled = false;
        }

        selectedCard = card;

        // add highlight to new
        Outline outline = card.GetComponent<Outline>();
        if (outline == null) outline = card.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
        outline.effectDistance = new Vector2(5, 5);
        outline.enabled = true;
    }

    public void ConfirmSelection()
    {
        // Prefer global hand selection so any CardSelectable works
        GameObject chosen = selectedCard;
        if (HandSelectionManager.Instance != null && HandSelectionManager.Instance.Current != null)
            chosen = HandSelectionManager.Instance.Current.gameObject;

        if (chosen == null)
        {
            Debug.Log("No card selected!");
            return;
        }

        var opt = chosen.GetComponent<RewardOption>();
        if (opt == null)
        {
            Debug.LogWarning("[Reward] Selected object has no RewardOption; ignoring");
            return;
        }

        if (DeckService.Instance == null)
        {
            Debug.LogWarning("[Reward] DeckService missing; cannot apply reward");
            return;
        }

        switch (opt.type)
        {
            case RewardOption.RewardType.NewCard:
                if (opt.newCardData != null)
                {
                    DeckService.Instance.AddNewCard(opt.newCardData);
                    Debug.Log($"[Reward] Added new card: {opt.newCardData.cardName}");
                    // Detailed log
                    var deckAfterAdd = DeckService.Instance.Deck != null ? DeckService.Instance.Deck.Count : 0;
                    Debug.Log($"[Reward][Continue] New Card '{opt.newCardData.cardName}' added. Deck size now {deckAfterAdd}. Effects:");
                    if (opt.newCardData.baseEffects != null)
                    {
                        foreach (var e in opt.newCardData.baseEffects)
                        {
                            if (e == null) continue;
                            Debug.Log($"  - {e.description}");
                        }
                    }
                }
                break;
            case RewardOption.RewardType.Upgrade:
                if (opt.originalInstance != null)
                {
                    var beforeStage = opt.originalInstance.Stage;
                    DeckService.Instance.UpgradeCardStage(opt.originalInstance);
                    var afterStage = opt.originalInstance.Stage;
                    Debug.Log($"[Reward] Upgraded stage: {opt.originalInstance.GetDisplayName()} {beforeStage} -> {afterStage}");
                }
                break;
        }
        RunSignals.AfterReward = true;
        LevelLoader.Instance?.LoadAction();
    }
}
