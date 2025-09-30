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

        // If explicit cards provided, render those
        var sourceCards = cardsToShow;
        if (sourceCards != null && sourceCards.Length > 0)
        {
            foreach (var cd in sourceCards)
            {
                var go = Instantiate(cardPrefab, container);
                go.name = cd != null ? $"Card_{cd.cardName}" : "Card";
                var le = go.GetComponent<LayoutElement>();
                if (le == null) le = go.AddComponent<LayoutElement>();
                le.preferredWidth = 260f;
                le.preferredHeight = 360f;
                var rt = go.transform as RectTransform;
                if (rt != null) rt.localScale = Vector3.one;
                var display = go.GetComponent<CardDisplay>();
                var instance = new CardInstance(cd);
                display.Init(instance);
                var selectable = go.GetComponent<CardSelectable>();
                if (selectable == null) selectable = go.AddComponent<CardSelectable>();
                selectable.Initialize(instance);
            }
            return;
        }

        // Otherwise, draw from the actual deck instances so upgrades are reflected immediately
        if (DeckService.Instance != null)
        {
            var deck = DeckService.Instance.Deck;
            if (deck == null || deck.Count == 0)
            {
                Debug.LogWarning("CardHandSpawner: Deck empty; nothing to show.");
                return;
            }
            Debug.Log($"[Deck] Initial hand draw from runtime deck. Deck count: {deck.Count}");
            var picks = DeckService.Instance.PickRandomFromDeck(3);
            SpawnSpecific(picks);
            return;
        }

        Debug.LogWarning("CardHandSpawner: No cards to show. Assign Cards To Show or ensure Deck has cards.");
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