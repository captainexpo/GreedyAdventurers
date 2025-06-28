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
        activeHero = Hero.New(new Vector2Int(0, 0), heroPrefab, gridManager, this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Enable()
    {
        if (isEnabled) return; // Already enabled
        isEnabled = true;
        activeHero.StartMoving();
    }

    public void Disable()
    {
        isEnabled = false;
        activeHero.StopMoving();
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
