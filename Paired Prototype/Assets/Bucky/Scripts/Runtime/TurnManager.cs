using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public bool IsPlayLocked { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartTurn()
    {
        IsPlayLocked = false;
        ResetAllBlocks();
    }

    public void EndTurn()
    {
        // Hook for end-turn effects if needed
    }

    public void NextTurn()
    {
        EndTurn();
        StartTurn();
    }

    public void LockPlayForThisTurn()
    {
        IsPlayLocked = true;
    }

    private void ResetAllBlocks()
    {
        var allHealth = FindObjectsOfType<Health>(includeInactive: false);
        foreach (var h in allHealth)
        {
            h.currentBlock = 0;
        }
    }
}


