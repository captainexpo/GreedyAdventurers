using Grid;
using UnityEngine;


public enum RoundPhase
{
    Building,
    HeroMoving,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private HeroManager heroManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BuilderManager builderManager;


    public RoundPhase phase = RoundPhase.Building;

    void Start()
    {
        NewPhase(phase); // Initialize the game with the starting phase
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (phase == RoundPhase.Building && TryAdvancePhase(out var newPhase))
                NewPhase(newPhase);
        }
    }

    void NewPhase(RoundPhase phase)
    {
        print("New phase: " + phase);
        switch (phase)
        {
            case RoundPhase.Building:
                builderManager.Enable();
                heroManager.Disable();
                break;
            case RoundPhase.HeroMoving:
                builderManager.Disable();
                heroManager.Enable();
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }
    public void RevertPhase()
    {
        switch (phase)
        {
            case RoundPhase.Building:
                phase = RoundPhase.HeroMoving;
                break;
            case RoundPhase.HeroMoving:
                phase = RoundPhase.Building;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }
    public bool TryAdvancePhase(out RoundPhase newPhase)
    {
        print("Trying to advance phase from: " + phase);
        switch (phase)
        {
            case RoundPhase.Building:
                if (!builderManager.CheckValidity()) // Check if the dungeon is valid before moving to the next phase
                {
                    newPhase = phase;
                    return false;
                }
                phase = RoundPhase.HeroMoving;
                break;
            case RoundPhase.HeroMoving:
                phase = RoundPhase.Building; // Automatically revert to Building phase after moving
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
        newPhase = phase;
        NewPhase(phase);
        return true;
    }
    public void HeroGotTreasure(Hero hero)
    {
        // TODO: Handle treasure collection logic here (probably take a life away, update UI, etc.)
        if(phase == RoundPhase.HeroMoving) 
            TryAdvancePhase(out var newPhase);
    }
}
