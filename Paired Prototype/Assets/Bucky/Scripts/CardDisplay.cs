using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [Header("Data")]
    public CardData cardData;

    [Header("UI Refs")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI descText;

    public void Init(CardData data)
    {
        cardData = data;
        if (nameText)  nameText.text  = data.cardName;
        if (powerText) powerText.text = data.power.ToString();
        if (descText)  descText.text  = data.description;
    }

    // Handy for previewing in Editor if you assign cardData in Inspector
    private void OnValidate()
    {
        if (cardData != null) Init(cardData);
    }
}
