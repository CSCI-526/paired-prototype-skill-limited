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
    public Transform scrollContent;      // ScrollView/Viewport/Content
    public TMP_Text counterText;         // "Selected: N/5"
    public Button startButton;           // disabled until N==5

    [Header("Card Sources & Prefab")]
    public List<CardData> cardDatas;     // Drag CardData assets here (e.g., AOE6, Defend-LosePower...)
    public GameObject cardPrefab;        // Use the SAME prefab as Reward scene (has CardDisplay)

    [Header("Settings")]
    public int deckSize = 5;

    private readonly List<int> selected = new();
    private readonly List<GameObject> spawned = new();

    void Start()
    {
        // Title first
        titlePanel.SetActive(true);
        selectPanel.SetActive(false);

        // Prebuild list so selection panel shows immediately after click
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

        // Pass the chosen CardData list to your run (use the static holder you already added)
        SelectedDeck.Set(chosen);

        // Go to gameplay via your transition
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
            // Build a runtime CardInstance from CardData (matches how Reward scene does previews)
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

            // Basic selection visuals using a Button (for your existing select logic)
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
