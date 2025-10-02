// DeckStartingCardsInjector.cs
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class DeckStartingCardsInjector : MonoBehaviour
{
    void Awake()
    {
        if (SelectedDeck.SelectedCards.Count == 0) return;

        var bootstrap = FindObjectOfType<DeckBootstrap>();
        if (!bootstrap) return;

        bootstrap.startingCards = SelectedDeck.SelectedCards.ToArray();
    }
}
