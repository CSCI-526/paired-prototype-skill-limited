using UnityEngine;

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName = "Unnamed";

    [Header("Rarity")]
    public CardRarity rarity = CardRarity.Common;

    [Header("Default effects at start of run")]
    public CardEffect[] baseEffects;

    [TextArea] public string description; // Optional static flavor
}
