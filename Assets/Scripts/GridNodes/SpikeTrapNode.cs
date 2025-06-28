using Grid;
using UnityEngine;

public class SpikeTrapNode : MonoBehaviour, GridNodeBase
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HeroManager heroManager;
    public GridNodeType nodeType { get; } = GridNodeType.SpikeTrap;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        heroManager = FindFirstObjectByType<HeroManager>();
    }

    public void OnHeroEnter(Hero hero)
    {
        hero.Damage(1);
    }

    public void OnHeroExit(Hero hero)
    {

    }
}
