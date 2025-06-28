using DG.Tweening;
using EasyButtons;
using Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HeroType
{
    Normal
}

public class HeroManager : MonoBehaviour
{

    public Hero activeHero;
    public Pathfinder pathfinder;
    public GridManager gridManager;
    public GameManager gameManager;

    public bool isEnabled = false;

    public GameObject heroPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Started Hero Manager");
    }

    public void SpawnNewHero()
    {
        var position = gridManager.startPositions[Random.Range(0, gridManager.startPositions.Count)];
        if (activeHero != null)
        {
            Debug.LogWarning("A hero is already active. Cannot spawn a new one.");
            return;
        }
        activeHero = Hero.New(position, heroPrefab, gridManager, this);
    }

    // Update is called once per frame
    public void HeroKilled(Hero hero)
    {
        if (activeHero == hero)
        {
            activeHero.StopMoving();
            activeHero = null;
            isEnabled = false;

            Debug.Log("Hero has been killed.");
            gameManager.TryAdvancePhase(out var _);
        }
    }
    public void Enable()
    {
        if (isEnabled) return; // Already enabled
        SpawnNewHero();
        isEnabled = true;
        activeHero.StartMoving();
    }

    public void Disable()
    {
        isEnabled = false;
        if (activeHero != null)
        {
            activeHero.KillSelf(true); // Force kill the hero to stop any ongoing movement
            activeHero.StopMoving();
        }
    }

    public void HeroDoneMoving(Hero hero)
    {
        activeHero.StopMoving();
        gameManager.TryAdvancePhase(out var newPhase);

    }

    public void HeroGotTreasure(Hero hero)
    {
        print("Hero found the treasure!");
        gameManager.HeroGotTreasure(hero);
        return;
    }
}
