using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardFlowController : MonoBehaviour
{
    [Header("Scene Refs")]
    public Transform newCardArea;      // container with one slot
    public Transform upgradeArea;      // container with three slots
    public GameObject cardPrefab;      // Assets/Bucky/Prefabs/Cards/Card.prefab
    public Button continueButton;      // wired to call RewardCardSelector.ConfirmSelection

    [Header("Reward Pools")] 
    public CardData[] newCardPool;     // assign in Inspector

    [Tooltip("Multiplier when enhancing positive effects (>=1). 1.5 = +50%.")]
    public float positiveMultiplier = 1.5f;
    [Tooltip("Multiplier when decreasing negative effects (0..1). 0.5 = -50%.")]
    public float negativeMultiplier = 0.5f;

    [Header("Card Visual Size")] 
    [Tooltip("Preferred width for reward cards in UI layout (pixels).")]
    public float preferredCardWidth = 240f;
    [Tooltip("Preferred height for reward cards in UI layout (pixels).")]
    public float preferredCardHeight = 360f;
    [Tooltip("Optional local scale applied to instantiated card prefabs.")]
    public Vector3 cardLocalScale = new Vector3(1.0f, 1.0f, 1.0f);

    [Header("Card Text Sizing")]
    [Tooltip("Base font size for the card name at 260x360.")]
    public float baseNameFontSize = 12f;
    [Tooltip("Base font size for the description at 260x360.")]
    public float baseDescFontSize = 8f;
    [Tooltip("Padding applied around text areas (pixels).")]
    public float textPadding = 20f;
    [Tooltip("Portion of card height reserved for the title area (0..1).")]
    [Range(0.05f, 0.4f)] public float nameSectionHeightRatio = 0.18f;
    [Tooltip("Portion of card height reserved for the description area (0..1), independent of name.")]
    [Range(0.1f, 0.9f)] public float descSectionHeightRatio = 0.5f;

    void Start()
    {
        // Defensive: clear any selection carried over
        if (HandSelectionManager.Instance != null)
            HandSelectionManager.Instance.Clear();

        GenerateNewCardOption();
        GenerateUpgradeOptions();

        // Ensure Continue starts disabled until selection
        if (continueButton != null)
            continueButton.interactable = false;

        RebuildArea(newCardArea);
        RebuildArea(upgradeArea);
    }

    private void GenerateNewCardOption()
    {
        if (newCardArea == null || cardPrefab == null) return;

        // Clean container
        for (int i = newCardArea.childCount - 1; i >= 0; i--)
            Destroy(newCardArea.GetChild(i).gameObject);

        if (newCardPool == null || newCardPool.Length == 0)
        {
            Debug.LogWarning("[Reward] newCardPool is empty; New Card option disabled");
            return;
        }

        var data = newCardPool[Random.Range(0, newCardPool.Length)];
        var inst = new CardInstance(data);

        var go = Instantiate(cardPrefab, newCardArea);
        go.name = data != null ? $"Reward_New_{data.cardName}" : "Reward_NewCard";
        ApplySizing(go);
        var display = go.GetComponent<CardDisplay>();
        display.Init(inst);

        var selectable = go.GetComponent<CardSelectable>();
        if (selectable == null) selectable = go.AddComponent<CardSelectable>();
        selectable.Initialize(inst);

        var opt = go.AddComponent<RewardOption>();
        opt.ConfigureAsNewCard(data);
    }

    private void GenerateUpgradeOptions()
    {
        if (upgradeArea == null || cardPrefab == null) return;

        // Clean container
        for (int i = upgradeArea.childCount - 1; i >= 0; i--)
            Destroy(upgradeArea.GetChild(i).gameObject);

        var deck = DeckService.Instance != null ? DeckService.Instance.Deck : null;
        if (deck == null || deck.Count == 0)
        {
            Debug.Log("[Reward] Deck empty, hiding upgrade row");
            upgradeArea.gameObject.SetActive(false);
            return;
        }

        var picks = DeckService.Instance.PickRandomFromDeck(3);

        for (int i = 0; i < picks.Count; i++)
        {
            var original = picks[i];
            if (original == null) continue;

            // 50% prefer enhancing positive, 50% prefer weakening negative
            bool preferPositive = Random.value < 0.5f;

            if (!TryBuildUpgradePreview(original, preferPositive, out var preview, out var oldEff, out var newEff))
            {
                // Retry with opposite type if first choice failed
                if (!TryBuildUpgradePreview(original, !preferPositive, out preview, out oldEff, out newEff))
                {
                    Debug.LogWarning($"[Reward] Could not build upgrade preview for {original.GetDisplayName()}");
                    continue;
                }
            }

            var go = Instantiate(cardPrefab, upgradeArea);
            go.name = $"Reward_Upgrade_{original.GetDisplayName()}_{i}";
            ApplySizing(go);
            var display = go.GetComponent<CardDisplay>();
            display.Init(preview);

            var selectable = go.GetComponent<CardSelectable>();
            if (selectable == null) selectable = go.AddComponent<CardSelectable>();
            selectable.Initialize(preview);

            var opt = go.AddComponent<RewardOption>();
            opt.ConfigureAsUpgrade(original, oldEff, newEff);
        }
    }

    private void ApplySizing(GameObject go)
    {
        // Ensure layout element exists and set preferred size
        var le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        le.preferredWidth = preferredCardWidth;
        le.preferredHeight = preferredCardHeight;

        // Normalize RectTransform scale, then apply optional local scale
        var rt = go.transform as RectTransform;
        if (rt != null)
        {
            rt.localScale = Vector3.one;
            // If no parent layout will size for us, also set size directly
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredCardWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredCardHeight);
        }
        go.transform.localScale = cardLocalScale;

        // If parent uses a GridLayoutGroup, update its cell size to fit our preference
        var parentRt = go.transform.parent as RectTransform;
        if (parentRt != null)
        {
            var grid = parentRt.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                grid.cellSize = new Vector2(preferredCardWidth, preferredCardHeight);
            }
        }

        // Scale text boxes and font sizes proportionally to card size
        const float baseWidth = 260f;
        const float baseHeight = 360f;
        float scale = Mathf.Min(
            preferredCardWidth > 0 ? preferredCardWidth / baseWidth : 1f,
            preferredCardHeight > 0 ? preferredCardHeight / baseHeight : 1f
        );

        var display = go.GetComponent<CardDisplay>();
        if (display != null)
        {
            if (display.nameText != null)
                display.nameText.fontSize = Mathf.Max(1f, Mathf.Round(baseNameFontSize * scale));
            if (display.descText != null)
                display.descText.fontSize = Mathf.Max(1f, Mathf.Round(baseDescFontSize * scale));

            float nameH = Mathf.Clamp(preferredCardHeight * nameSectionHeightRatio, 20f, preferredCardHeight);
            float descH = Mathf.Clamp(preferredCardHeight * descSectionHeightRatio, 20f, preferredCardHeight);

            // If combined exceeds available height, proportionally scale them down to fit
            float maxUsable = Mathf.Max(0f, preferredCardHeight - 3f * textPadding);
            float combined = nameH + descH;
            if (combined > maxUsable && combined > 0f)
            {
                float s = maxUsable / combined;
                nameH *= s;
                descH *= s;
            }

            float contentWidth = Mathf.Max(0f, preferredCardWidth - 2f * textPadding);

            if (display.nameText != null)
            {
                var nameRt = display.nameText.rectTransform;
                // Anchor to top stretch horizontally
                nameRt.anchorMin = new Vector2(0f, 1f);
                nameRt.anchorMax = new Vector2(1f, 1f);
                nameRt.pivot = new Vector2(0.5f, 1f);
                // Offsets from card edges (top area)
                nameRt.offsetMin = new Vector2(textPadding, -nameH);
                nameRt.offsetMax = new Vector2(-textPadding, -textPadding);
                nameRt.anchoredPosition = Vector2.zero;
            }
            if (display.descText != null)
            {
                var descRt = display.descText.rectTransform;
                // Anchor to bottom, stretch horizontally, fixed height
                descRt.anchorMin = new Vector2(0f, 0f);
                descRt.anchorMax = new Vector2(1f, 0f);
                descRt.pivot = new Vector2(0.5f, 0f);
                // Horizontal padding via offsets; vertical height via size
                descRt.offsetMin = new Vector2(textPadding, 0f);
                descRt.offsetMax = new Vector2(-textPadding, 0f);
                descRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, descH);
                descRt.anchoredPosition = new Vector2(0f, textPadding);
            }

            if (rt != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }

    private void RebuildArea(Transform area)
    {
        var rt = area as RectTransform;
        if (rt == null) return;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    private bool TryBuildUpgradePreview(
        CardInstance original,
        bool preferPositive,
        out CardInstance preview,
        out CardEffect oldEffect,
        out CardEffect upgradedEffect)
    {
        preview = null;
        oldEffect = null;
        upgradedEffect = null;

        var all = new List<CardEffect>(original.Effects);
        if (all.Count == 0) return false;

        List<CardEffect> positives = new List<CardEffect>();
        List<CardEffect> negatives = new List<CardEffect>();

        foreach (var e in all)
        {
            if (e == null) continue;
            if (e.effectType == EffectType.Positive) positives.Add(e);
            else if (e.effectType == EffectType.Negative) negatives.Add(e);
        }

        List<CardEffect> pool = preferPositive ? positives : negatives;
        if (pool.Count == 0) pool = preferPositive ? negatives : positives; // fallback
        if (pool.Count == 0) return false;

        // If decreasing negatives, avoid non-magnitude effects (like EndTurnLock)
        CardEffect chosen = null;
        for (int guard = 0; guard < 10 && chosen == null; guard++)
        {
            var candidate = pool[Random.Range(0, pool.Count)];
            if (!preferPositive && !IsMagnitudeNegativeEffect(candidate))
                continue;
            chosen = candidate;
        }
        if (chosen == null) return false;

        CardEffect upgraded = UpgradeEffectMagnitude(chosen, preferPositive);
        if (upgraded == null) return false;

        // Build preview list replacing the one effect
        var list = new List<CardEffect>(all);
        int idx = list.IndexOf(chosen);
        list[idx] = upgraded;
        preview = new CardInstance(original.baseData, list);

        oldEffect = chosen;
        upgradedEffect = upgraded;
        return true;
    }

    private bool IsMagnitudeNegativeEffect(CardEffect e)
    {
        return e is SelfDamageEffect || e is LosePowerEffect;
    }

    private CardEffect UpgradeEffectMagnitude(CardEffect original, bool increasePositive)
    {
        // New approach: delegate to the effect itself if supported.
        if (original != null && original.SupportsUpgrade)
        {
            var upgraded = original.CreateUpgradedCopy();
            if (upgraded != null) return upgraded;
        }

        // Fallback: keep previous behavior for any effect lacking implementation
        Debug.LogWarning($"[Reward] Effect {original?.GetType().Name} has no CreateUpgradedCopy; skipping");
        return null;
    }
}


