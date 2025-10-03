using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public bool IsPlayLocked { get; private set; }
    private int skipTurnsRemaining = 0; // player skips

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
        ResetBlockDisables();

        if (skipTurnsRemaining > 0)
        {
            // Automatically end the turn without allowing play
            IsPlayLocked = true;
            skipTurnsRemaining--;
        }
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

    public void AddSkipTurns(int turns)
    {
        if (turns <= 0) return;
        skipTurnsRemaining += turns;
    }

    private void ResetAllBlocks()
    {
        var allHealth = FindObjectsOfType<Health>(includeInactive: false);
        foreach (var h in allHealth)
        {
            h.currentBlock = 0;
        }
    }

    private void ResetBlockDisables()
    {
        var allHealth = FindObjectsOfType<Health>(includeInactive: false);
        foreach (var h in allHealth)
        {
            h.blockDisabledThisTurn = false;
        }
    }
}


