// SelectedDeck.cs
using System.Collections.Generic;

public static class SelectedDeck
{
    public static readonly List<CardData> SelectedCards = new List<CardData>(5);

    public static void Set(IEnumerable<CardData> cards)
    {
        SelectedCards.Clear();
        SelectedCards.AddRange(cards);
    }

    public static void Clear()
    {
        SelectedCards.Clear();
    }
}
