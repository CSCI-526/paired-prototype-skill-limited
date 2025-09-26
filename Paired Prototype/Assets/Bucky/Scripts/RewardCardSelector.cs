using UnityEngine;
using UnityEngine.UI;

public class RewardCardSelector : MonoBehaviour
{
    public static RewardCardSelector Instance;

    private GameObject selectedCard;

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

        Debug.Log("Chosen card: " + selectedCard.name);
        // TODO: Apply the chosen card to deck/upgrade
        // Then load next scene / continue game
    }
}
