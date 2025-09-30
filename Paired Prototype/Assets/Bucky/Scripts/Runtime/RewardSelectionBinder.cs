using UnityEngine;
using UnityEngine.UI;

// Enables Continue button when any RewardOption is selected via HandSelectionManager
public class RewardSelectionBinder : MonoBehaviour
{
    public Button continueButton;

    void Update()
    {
        if (continueButton == null) return;
        var current = HandSelectionManager.Instance != null ? HandSelectionManager.Instance.Current : null;
        continueButton.interactable = current != null && current.GetComponent<RewardOption>() != null;
    }
}


