using Grid;
using UnityEngine;

public class TreasureNode : MonoBehaviour, GridNodeBase
{
    public GridNodeType nodeType { get; } = GridNodeType.Treasure;

    public void OnHeroEnter(Hero hero)
    {
        return;
    }

    public void OnHeroExit(Hero hero)
    {
        return;
    }
}
