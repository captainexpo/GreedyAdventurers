using Grid;
using UnityEngine;

public class WallNode : MonoBehaviour, GridNodeBase
{
    public GridNodeType nodeType { get; } = GridNodeType.Wall;
}
