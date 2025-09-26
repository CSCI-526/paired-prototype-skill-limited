using System.Text;
using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text descText;  // long multi-line
    private CardInstance instance;

    public void Init(CardInstance card)
    {
        instance = card;
        nameText.text = card.GetDisplayName();
        if (powerText != null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var ph = player != null ? player.GetComponent<Health>() : null;
            powerText.text = ph != null ? ph.power.ToString() : "0";
        }
        descText.text = BuildEffectText(card);
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
