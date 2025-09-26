using UnityEngine;

public enum CardType { Attack, Defense, PowerUp }

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string cardName = "Unnamed";
    public CardType type = CardType.Attack;

    [Header("Numbers")]
    public int power = 5;            // damage for Attack, block for Defense, magnitude for PowerUp
    public int cost = 1;             // energy/hp (future use)

    [TextArea]
    public string description = "Describe the effect here.";

    [Header("Upgrade (future)")]
    public int upgradePowerBonus = 2;
    [TextArea] public string upgradedDescription = "Stronger effect.";
}
