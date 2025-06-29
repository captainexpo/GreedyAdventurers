using DG.Tweening;
using Grid;
using UnityEngine;

public class OilNode : MonoBehaviour, GridNodeBase
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HeroManager heroManager;
    public GridNodeType nodeType { get; } = GridNodeType.OilTrap;
    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        heroManager = FindFirstObjectByType<HeroManager>();
    }
    public void OnHeroEnter(Hero hero)
    {
        hero.weakness += 1;
    }
}