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

        // Move hero to a random position
        Vector2Int randomPosition = gridManager.GetRandomPosition(); 
        hero.gridPos = randomPosition; // Update hero's position

        // Make hero fall from the sky
        hero.transform.position = gridManager.TileToWorld(randomPosition.x, randomPosition.y) + new Vector3(0, 5f, 0);
        hero.transform.DOMove(gridManager.TileToWorld(randomPosition.x, randomPosition.y), 0.4f);

        Invoke(nameof(Cover), 0.75f);
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
