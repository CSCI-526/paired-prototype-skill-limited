using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName = "Unnamed";

    [Header("Default effects at start of run")]
    public CardEffect[] baseEffects;

    [TextArea] public string description; // Optional static flavor
}
