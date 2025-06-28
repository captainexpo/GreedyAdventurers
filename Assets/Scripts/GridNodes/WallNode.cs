using Grid;
using UnityEngine;

public class WallNode : MonoBehaviour, GridNodeBase
{
    public GridNodeType nodeType { get; } = GridNodeType.Wall;

    public void OnHeroEnter(Hero hero)
    {
        return;
    }

    public void OnHeroExit(Hero hero)
    {
        return;
    }
}
