using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemySelectable : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer sprite;
    public Color highlightColor = Color.yellow;

    Color baseColor;

    void Awake()
    {
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();

        if (sprite) baseColor = sprite.color;
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left) return;

        SelectManager.Instance.Select(this);
    }

    public void SetSelected(bool selected)
    {
        if (!sprite) return;

        if (selected)
        {
            sprite.color = highlightColor;
        }
        else
        {
            sprite.color = baseColor;
        }
    }
}
