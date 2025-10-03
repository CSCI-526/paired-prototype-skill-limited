using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject selectPanel;

    [Header("Selection UI")]
    public Transform scrollContent;     
    public TMP_Text counterText;        
    public Button startButton;           

    [Header("Card Sources & Prefab")]
    public List<CardData> cardDatas;     
    public GameObject cardPrefab;        

    [Header("Settings")]
    public int deckSize = 5;

    private readonly List<int> selected = new();
    private readonly List<GameObject> spawned = new();

    void Start()
    {
        titlePanel.SetActive(true);
        selectPanel.SetActive(false);

        BuildList();
        UpdateCounterAndButton();
    }

    public void OnClickTitleStart()
    {
        titlePanel.SetActive(false);
        selectPanel.SetActive(true);
    }

    public void OnClickStartGame()
    {
        if (selected.Count != deckSize) return;

        var chosen = new List<CardData>(deckSize);
        foreach (var idx in selected)
            chosen.Add(cardDatas[idx]);
        SelectedDeck.Set(chosen);
        // Reset run progress and rebuild the runtime deck immediately for a clean start
        RunProgress.Reset();
        try
        {
            if (DeckService.Instance != null)
            {
                DeckService.Instance.BuildStartingDeck(chosen);
            }
        }
        catch {}
        LevelLoader.Instance?.LoadAction();
    }

    void BuildList()
    {
        foreach (Transform c in scrollContent) Destroy(c.gameObject);
        spawned.Clear();

        if (cardDatas == null || cardDatas.Count == 0 || cardPrefab == null) return;

        for (int i = 0; i < cardDatas.Count; i++)
        {
            var data = cardDatas[i];
            var inst = new CardInstance(data);

            var item = Instantiate(cardPrefab, scrollContent);
            item.name = data != null ? $"Menu_{data.cardName}" : "Menu_Card";
            spawned.Add(item);

            // Use the real in-game binder so text & colors match gameplay
            var display = item.GetComponent<CardDisplay>();
            if (display != null) display.Init(inst);

            // Make it clickable for selection
            var selectable = item.GetComponent<CardSelectable>();
            if (selectable == null) selectable = item.AddComponent<CardSelectable>();
            selectable.Initialize(inst);

            var btn = item.GetComponent<Button>();
            if (btn == null) btn = item.AddComponent<Button>();

            int capturedIndex = i;
            btn.onClick.AddListener(() => ToggleSelect(capturedIndex, item));
            SetItemVisual(item, false);
        }
    }

    void ToggleSelect(int index, GameObject item)
    {
        if (selected.Contains(index))
        {
            selected.Remove(index);
            SetItemVisual(item, false);
        }
        else
        {
            if (selected.Count >= deckSize) return;
            selected.Add(index);
            SetItemVisual(item, true);
        }
        UpdateCounterAndButton();
    }

    void UpdateCounterAndButton()
    {
        if (counterText) counterText.text = $"Selected: {selected.Count}/{deckSize}";
        if (startButton) startButton.interactable = (selected.Count == deckSize);
    }

    void SetItemVisual(GameObject item, bool isSelected)
    {
        // Light, non-invasive highlight for the menu
        var t = item.transform;
        t.localScale = isSelected ? new Vector3(1.06f, 1.06f, 1f) : Vector3.one;

        var img = item.GetComponent<Image>();
        if (img != null)
        {
            var c = img.color;
            c.a = isSelected ? 1f : 0.85f;
            img.color = c;
        }
    }
}
