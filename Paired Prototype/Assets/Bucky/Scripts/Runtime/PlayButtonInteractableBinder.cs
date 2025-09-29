using UnityEngine;
using UnityEngine.UI;

public class PlayButtonInteractableBinder : MonoBehaviour
{
    public Button playButton;
    public bool alwaysEnable = false; // Set true if this is used on End Turn or should never be disabled

    void Reset()
    {
        if (playButton == null) playButton = GetComponent<Button>();
    }

    void Update()
    {
        if (playButton == null) return;
        if (alwaysEnable)
        {
            playButton.interactable = true;
            return;
        }
        var selected = HandSelectionManager.Instance != null ? HandSelectionManager.Instance.Selected : null;
        bool hasSelection = selected != null;

        bool needsTarget = false;
        bool hasTarget = true;
        if (hasSelection)
        {
            foreach (var eff in selected.Effects)
            {
                if (eff != null && eff.RequiresTarget)
                {
                    needsTarget = true;
                    break;
                }
            }
            if (needsTarget)
            {
                hasTarget = SelectManager.Instance != null && SelectManager.Instance.Current != null;
            }
        }

        playButton.interactable = hasSelection && (!needsTarget || hasTarget);
    }
}


