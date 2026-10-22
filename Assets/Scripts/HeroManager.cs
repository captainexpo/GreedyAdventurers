using Grid;
using System.Collections;
using DG.Tweening;
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
    public BuilderManager builderManager;

    public bool heroIsDead
    {
        get => activeHero == null;
    }

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
        StartCoroutine(_HeroKilled(hero));
    }
    IEnumerator _HeroKilled(Hero hero)
    {
        yield return new WaitForSeconds(0.25f);
        if (activeHero == hero)
        {
            activeHero.StopMoving();
            activeHero = null;
            isEnabled = false;

            Debug.Log("Hero has been killed.");
            gameManager.TryAdvancePhase();
        }
        gameManager.HeroDied(hero);
        builderManager.UpdateFunds(-75);
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
        if(gameManager.phase == RoundPhase.HeroMoving) gameManager.TryAdvancePhase();

    }

    public void HeroGotTreasure(Hero hero)
    {
        print("Hero found the treasure!");
        Camera.main.DOShakePosition(0.5f, Vector3.one*0.1f);
        gameManager.HeroGotTreasure(hero);
    }
}
