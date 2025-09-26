using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;       // assign the Card prefab
    public CardData cardToShow;         // assign a CardData asset (e.g., Strike)
    public Transform parent;            // assign Canvas or a panel under it

    private void Start()
    {
        if (!cardPrefab || !cardToShow || !parent) {
            Debug.LogError("Spawner missing references.");
            return;
        }

        var go = Instantiate(cardPrefab, parent);
        var display = go.GetComponent<CardDisplay>();
        display.Init(cardToShow);
    }
}
