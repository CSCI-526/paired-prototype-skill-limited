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
    public Transform scrollContent;        // Content of a Scroll View
    public TMP_Text counterText;           // "Selected: N/5"
    public Button startButton;             // Disabled until N==5

    [Header("Available Cards")]
    public List<GameObject> cardPrefabs;   // Drag all your Card prefabs here

    [Header("Settings")]
    public int deckSize = 5;

    private readonly List<int> selected = new();
    private readonly List<GameObject> spawned = new();

    void Start()
    {
        ShowTitle();
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

        var names = new List<string>(deckSize);
        foreach (var idx in selected)
            names.Add(cardPrefabs[idx].name);
        SelectedDeck.Set(names);

        LevelLoader.Instance?.LoadAction();
    }

    void ShowTitle()
    {
        titlePanel.SetActive(true);
        selectPanel.SetActive(false);
    }

    void BuildList()
    {
        foreach (Transform c in scrollContent) Destroy(c.gameObject);
        spawned.Clear();

        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            var item = Instantiate(cardPrefabs[i], scrollContent);
            spawned.Add(item);

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
        var t = item.transform;
        t.localScale = isSelected ? new Vector3(1.08f, 1.08f, 1f) : Vector3.one;

        var img = item.GetComponent<Image>();
        if (img != null)
        {
            var c = img.color;
            c.a = isSelected ? 1f : 0.8f;
            img.color = c;
        }
    }
}
