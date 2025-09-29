using UnityEngine;
[DefaultExecutionOrder(-50)]
public class DeckBootstrap : MonoBehaviour
{
    public CardData[] startingCards;

    void Start()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.StartTurn();
        Debug.Log($"[TurnManager] IsPlayLocked={TurnManager.Instance.IsPlayLocked}");
        if (DeckService.Instance != null && startingCards != null)
        {
            bool needsBuild = DeckService.Instance.Deck == null || DeckService.Instance.Deck.Count == 0;
            if (needsBuild)
            {
                DeckService.Instance.BuildStartingDeck(startingCards);
                Debug.Log($"[Deck] Built deck. startingCards={startingCards?.Length ?? 0}, deckCount={DeckService.Instance?.Deck.Count}");
            }
            else
            {
                Debug.Log($"[Deck] Skipped build (already has {DeckService.Instance.Deck.Count} cards)");
            }
        }
    }
}