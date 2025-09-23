using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }
    public EnemySelectable Current { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Select(EnemySelectable e)
    {
        if (Current == e)
        {
            Current.SetSelected(false);
            Current = null;
            return;
        }
        if (Current != null) Current.SetSelected(false);

        Current = e;
        if (Current != null) Current.SetSelected(true);
    }

    public void ClearSelection()
    {
        if (Current != null) Current.SetSelected(false);

        Current = null;
    }
}
