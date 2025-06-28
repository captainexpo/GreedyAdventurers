using Grid;
using UnityEngine;

public class PlayerEntranceNode : MonoBehaviour, GridNodeBase
{
    public GridNodeType nodeType { get; } = GridNodeType.PlayerEntrance;

    public void OnHeroEnter(Hero hero)
    {
        return;
    }

    public void OnHeroExit(Hero hero)
    {
        return;
    }
}
