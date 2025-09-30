using System.Text;
using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public TMP_Text nameText;
    public TMP_Text descText;  // long multi-line
    private CardInstance instance;

    void Awake()
    {
        // Allow prefab to work when dropped in the scene with only CardData set
        if (instance == null && cardData != null)
        {
            Init(cardData);
        }
    }

    public void Init(CardInstance card)
    {
        instance = card;
        nameText.text = card.GetDisplayName();
        descText.text = BuildEffectText(card);
    }

    public void Init(CardData data)
    {
        if (data == null) return;
        var temp = new CardInstance(data);
        Init(temp);
    }

    private string BuildEffectText(CardInstance card)
    {
        // Use TMP rich text for color (green for pos, red for neg)
        StringBuilder sb = new StringBuilder();

        foreach (var eff in card.Effects)
        {
            if (eff == null) continue;
            string line = eff.description;

            switch (eff.effectType)
            {
                case EffectType.Positive: line = $"<color=#00C853>{line}</color>"; break; // green
                case EffectType.Negative: line = $"<color=#D50000>{line}</color>"; break; // red
                default: break;
            }
            sb.AppendLine(line);
        }

        return sb.ToString().TrimEnd();
    }

    // If effects change after shown (upgrade/curse), re-render:
    public void Refresh() { if (instance != null) Init(instance); }
}
