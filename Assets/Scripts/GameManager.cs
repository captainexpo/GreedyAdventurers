using Grid;
using UnityEngine;


public enum RoundPhase
{
    Prelude = -1,
    Building,
    HeroMoving,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private HeroManager heroManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BuilderManager builderManager;

    public int round = 0;

    public RoundPhase phase = RoundPhase.Building;

    void Start()
    {
        Application.targetFrameRate = 60;
        Invoke(nameof(StartPhases), 0.1f);
        Camera.main.GetComponent<CameraManager>().ResetRotation();
    }

    void StartPhases()
    {
        NewPhase(phase); // Initialize the game with the starting phase
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (phase == RoundPhase.Building && TryAdvancePhase())
                NewPhase(phase);
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
                foreach (var row in gridManager.grid)
                foreach (var node in row)
                {
                    var beh = node.nodeBehavior;
                    if (beh == null) Debug.LogWarning(node.name + " doesn't have a node behavior set!");
                    else beh.OnStartBuild();
                }

                round++;
                uiManager.UpdateRoundText(round);
                break;
            case RoundPhase.HeroMoving:
                builderManager.Disable();
                heroManager.Enable();
                foreach (var row in gridManager.grid)
                    foreach (var node in row)
                        node.nodeBehavior.OnStartHeroMove(heroManager.activeHero);
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }
    public bool TryAdvancePhase()
    {
        print("Trying to advance phase from: " + phase);
        switch (phase)
        {
            case RoundPhase.Prelude:
                phase = RoundPhase.Building;
                break;
            case RoundPhase.Building:
                if (!builderManager.CheckValidity().Item1) // Check if the dungeon is valid before moving to the next phase
                {
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
        NewPhase(phase);
        return true;
    }
    public void HeroGotTreasure(Hero hero)
    {
        // TODO: Handle treasure collection logic here (probably take a life away, update UI, etc.)
        if(phase == RoundPhase.HeroMoving) 
            TryAdvancePhase();
        // Now in building phase
        
        // Delete a floor tile for extra difficulty
        var rpos = gridManager.GetRandomPosition(false);
        gridManager.TryReplaceAtPosition(rpos,
            gridManager.CreateGridNodeAtPos(gridManager.typeToPrefab[GridNodeType.Void], rpos, GridNodeType.Void, Orientation.North));
    }
    public void HeroDied(Hero hero)
    {
        // Optionally handle UI, stats, etc.
        if (phase == RoundPhase.HeroMoving)
            TryAdvancePhase();
    }
}
