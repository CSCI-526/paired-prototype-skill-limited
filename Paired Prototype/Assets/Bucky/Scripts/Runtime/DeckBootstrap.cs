using UnityEngine;
[DefaultExecutionOrder(-50)]
public class DeckBootstrap : MonoBehaviour
{
    public CardData[] startingCards;

    void Start()
    {
        if (TurnManager.Instance != null) TurnManager.Instance.StartTurn();
        if (TurnManager.Instance != null)
            Debug.Log($"[TurnManager] IsPlayLocked={TurnManager.Instance.IsPlayLocked}");

        if (DeckService.Instance == null)
        {
            Debug.LogWarning("[DeckBootstrap] DeckService missing in scene.");
            return;
        }

        // Determine if we need to (re)build the deck
        bool hasDeck = DeckService.Instance.Deck != null && DeckService.Instance.Deck.Count > 0;
        bool hasNullEntries = false;
        if (hasDeck)
        {
            // If any CardInstance or its baseData is null, treat deck as invalid
            for (int i = 0; i < DeckService.Instance.Deck.Count; i++)
            {
                var ci = DeckService.Instance.Deck[i];
                if (ci == null || ci.baseData == null)
                {
                    hasNullEntries = true;
                    break;
                }
            }
        }

        bool needsBuild = !hasDeck || hasNullEntries;

        // Choose the source of starting cards (prefer Main Menu selection if available)
        CardData[] source = null;
        try
        {
            // SelectedDeck is defined in Chace folder; safe to access if present
            if (SelectedDeck.SelectedCards != null && SelectedDeck.SelectedCards.Count > 0)
                source = SelectedDeck.SelectedCards.ToArray();
        }
        catch { /* SelectedDeck class might not exist in some contexts */ }

        // Fallback to inspector-assigned startingCards (filter out nulls)
        if (source == null || source.Length == 0)
        {
            if (startingCards != null && startingCards.Length > 0)
            {
                // Filter out any null elements to avoid null CardInstances
                var tmp = new System.Collections.Generic.List<CardData>(startingCards.Length);
                foreach (var c in startingCards)
                {
                    if (c != null) tmp.Add(c);
                }
                source = tmp.ToArray();
            }
        }

        if (needsBuild)
        {
            if (source == null || source.Length == 0)
            {
                Debug.LogWarning("[DeckBootstrap] No valid starting cards found. Deck will remain empty.");
                return;
            }

            // Rebuild deck from the chosen source
            DeckService.Instance.BuildStartingDeck(source);
            Debug.Log($"[Deck] Built deck. startingCards={source.Length}, deckCount={DeckService.Instance?.Deck.Count}");
        }
        else
        {
            if (hasNullEntries)
            {
                Debug.LogWarning("[Deck] Deck had null entries; attempted rebuild skipped due to missing sources.");
            }
            else
            {
                Debug.Log($"[Deck] Skipped build (already has {DeckService.Instance.Deck.Count} valid cards)");
            }
        }
    }
}