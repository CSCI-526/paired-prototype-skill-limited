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
        if (selectedCard == null)
        {
            Debug.Log("No card selected!");
            return;
        }

        var opt = selectedCard.GetComponent<RewardOption>();
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
                }
                break;
            case RewardOption.RewardType.Upgrade:
                if (opt.originalInstance != null && opt.upgradedEffectPreview != null)
                {
                    // Apply upgrade by adding the upgraded positive/less-negative effect
                    DeckService.Instance.ApplyUpgrade(opt.originalInstance, opt.upgradedEffectPreview);
                    Debug.Log($"[Reward] Upgraded {opt.originalInstance.GetDisplayName()} with {opt.upgradedEffectPreview.name}");
                }
                break;
        }

        // Return to main action scene
        if (!string.IsNullOrEmpty(actionSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(actionSceneName);
    }
}
