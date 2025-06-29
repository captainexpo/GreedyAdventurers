using DG.Tweening;
using Grid;
using UnityEngine;

public class PitTrapNode : MonoBehaviour, GridNodeBase
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HeroManager heroManager;

    public GameObject coverGFX;
    public bool trapEnabled = true;

    public GridNodeType nodeType { get; } = GridNodeType.SpikeTrap;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        heroManager = FindFirstObjectByType<HeroManager>();
        coverGFX.SetActive(false);
    }


    public void OnHeroEnter(Hero hero)
    {
        if (!trapEnabled) return;
        hero.Damage(2);

        if (heroManager.heroIsDead)
        {
            // Do nothing else to hero, because they're dead lmao
            return;
        }
        
        // Move hero to a random position
        Vector2Int randomPosition = gridManager.GetRandomPosition();
        hero.TryMoveTo(randomPosition, out _, overrideDefault:true);
        // Make hero fall from the sky
        
        Cover();
    }

    public void OnHeroExit(Hero hero)
    {
        // Logic for when a hero exits the spike trap node
        Debug.Log("Hero has exited the Spike Trap Node!");
        // Here you can add logic to reset effects or states
    }
    public void Cover()
    {
        trapEnabled = false;
        coverGFX.SetActive(true);
    }
    public void OnStartHeroMove(Hero hero)
    {
        trapEnabled = true;
        coverGFX.SetActive(false);
    }

    public void OnStartBuild()
    {
        trapEnabled = true;
        coverGFX.SetActive(false);
    }
}
