using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(50)]
public class CardHandSpawner : MonoBehaviour
{
    public Transform container;       // HandContainer
    public GameObject cardPrefab;     // Assets/Bucky/Prefabs/Cards/Card.prefab
    public CardData[] cardsToShow;    // Optional: show specific cards; else shows deck

    void Start()
    {
        if (container == null)
        {
            Debug.LogError("CardHandSpawner: Container is not assigned.");
            return;
        }
        if (cardPrefab == null)
        {
            Debug.LogError("CardHandSpawner: Card Prefab is not assigned.");
            return;
        }

        var sourceCards = cardsToShow;
        if ((sourceCards == null || sourceCards.Length == 0) && DeckService.Instance != null)
        {
            // Show first few from deck for demo
            // Convert to CardData for display only
            var deck = DeckService.Instance.Deck;
            Debug.Log($"[Deck] Deck count: {deck.Count}");
            int n = Mathf.Min(3, deck.Count);
            sourceCards = new CardData[n];
            for (int i = 0; i < n; i++) sourceCards[i] = deck[i].baseData;
        }

        if (sourceCards == null || sourceCards.Length == 0)
        {
            Debug.LogWarning("CardHandSpawner: No cards to show. Assign Cards To Show or ensure Deck has cards.");
            return;
        }

        foreach (var cd in sourceCards)
        {
            var go = Instantiate(cardPrefab, container);
            go.name = cd != null ? $"Card_{cd.cardName}" : "Card";
            // Ensure layout sizing works with HorizontalLayoutGroup/Grid
            var le = go.GetComponent<LayoutElement>();
            if (le == null) le = go.AddComponent<LayoutElement>();
            le.preferredWidth = 260f;
            le.preferredHeight = 360f;

            // Defensive: ensure RectTransform scale is sane
            var rt = go.transform as RectTransform;
            if (rt != null)
            {
                rt.localScale = Vector3.one;
            }
            var display = go.GetComponent<CardDisplay>();
            // Build a temporary instance purely for display
            var instance = new CardInstance(cd);
            display.Init(instance);

            // Make this card selectable in hand
            var selectable = go.GetComponent<CardSelectable>();
            if (selectable == null) selectable = go.AddComponent<CardSelectable>();
            selectable.Initialize(instance);
        }
    }

    // Spawn a provided set of CardInstances (e.g., from DeckService.PickRandomFromDeck)
    public void SpawnSpecific(System.Collections.Generic.List<CardInstance> instances)
    {
        if (container == null || cardPrefab == null || instances == null) return;
        foreach (var inst in instances)
        {
            var go = Instantiate(cardPrefab, container);
            go.name = inst != null && inst.baseData != null ? $"Card_{inst.baseData.cardName}" : "Card";
            var le = go.GetComponent<LayoutElement>();
            if (le == null) le = go.AddComponent<LayoutElement>();
            le.preferredWidth = 260f;
            le.preferredHeight = 360f;
            var rt = go.transform as RectTransform;
            if (rt != null) rt.localScale = Vector3.one;
            var display = go.GetComponent<CardDisplay>();
            display.Init(inst);
            var selectable = go.GetComponent<CardSelectable>();
            if (selectable == null) selectable = go.AddComponent<CardSelectable>();
            selectable.Initialize(inst);
        }
    }
}