using UnityEngine;
using UnityEngine.UI;

public class CardSelectable : MonoBehaviour
{
    private Outline outline;
    private Button button;
    private CardInstance cardInstance;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.effectColor = Color.yellow;
        outline.effectDistance = new Vector2(4f, 4f);
        outline.enabled = false;

        button = GetComponent<Button>();
        if (button == null) button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Initialize(CardInstance instance)
    {
        cardInstance = instance;
    }

    public CardInstance GetInstance()
    {
        return cardInstance;
    }

    public void SetSelected(bool selected)
    {
        if (outline != null) outline.enabled = selected;
    }

    private void OnClick()
    {
        if (HandSelectionManager.Instance != null)
        {
            HandSelectionManager.Instance.Select(this);
        }
    }
}


