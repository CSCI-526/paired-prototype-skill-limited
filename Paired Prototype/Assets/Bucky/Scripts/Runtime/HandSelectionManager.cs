using UnityEngine;

public class HandSelectionManager : MonoBehaviour
{
    public static HandSelectionManager Instance { get; private set; }

    private CardSelectable current;

    public CardInstance Selected => current != null ? current.GetInstance() : null;
    public CardSelectable Current => current;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Select(CardSelectable selectable)
    {
        if (selectable == current)
        {
            // toggle off
            if (current != null) current.SetSelected(false);
            current = null;
            return;
        }

        if (current != null) current.SetSelected(false);
        current = selectable;
        if (current != null) current.SetSelected(true);
    }

    public void Clear()
    {
        if (current != null) current.SetSelected(false);
        current = null;
    }
}


