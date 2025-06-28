using Grid;
using UnityEngine;

public class EmptyNode : MonoBehaviour, GridNodeBase
{
    public GridNodeType nodeType { get; } = GridNodeType.Empty;

    public void OnHeroEnter(Hero hero)
    {
        return;
    }

    public void OnHeroExit(Hero hero)
    {
        return;
    }
}
