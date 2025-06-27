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
    [SerializeField] private UIManager UIManager;

    public RoundPhase phase = RoundPhase.Building; 

    void Start()
    {
            
    }

    void Update()
    {

    }

    void NewTurn()
    {

    }

    public bool TryAdvancePhase(out RoundPhase newPhase)
    {
        if(phase == RoundPhase.Building) phase = RoundPhase.HeroMoving;
        if(phase == RoundPhase.HeroMoving) phase = RoundPhase.Building;
        newPhase = phase;
        return true;
    }
}
