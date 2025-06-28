using Grid;
using UnityEngine;
using System.Collections.Generic;

public enum Orientation 
{
    North,
    South,
    East,
    West,
}

public class Utils
{
    public static Vector2Int OrientationToVector(Orientation orientation)
    {
        return orientation switch
        {
            Orientation.North => new Vector2Int(0, 1),
            Orientation.South => new Vector2Int(0, -1),
            Orientation.East => new Vector2Int(1, 0),
            Orientation.West => new Vector2Int(-1, 0),
            _ => throw new System.ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
    }

    public static Quaternion OrientationToQuaternion(Orientation orientation)
    {
        return orientation switch
        {
            Orientation.North => Quaternion.Euler(0, 0, 0),
            Orientation.South => Quaternion.Euler(0, 180, 0),
            Orientation.East => Quaternion.Euler(0, 90, 0),
            Orientation.West => Quaternion.Euler(0, -90, 0),
            _ => throw new System.ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
    }
}
public class GridNode : MonoBehaviour
{
    public GridNodeType nodeType;
    public Vector2Int pos;
    public Vector2Int orientation = new(1, 0); // (1, 0) for right, (0, 1) for up, (-1, 0) for left, (0, -1) for down

    private Color highlightColor = new(0.7f, 0.7f, 0.7f);
    public List<GameObject> gfx;

    public GridNodeBase nodeBehavior { get; private set; }

    private void Start()
    {
        nodeBehavior = GetComponent<GridNodeBase>();
        nodeType = nodeBehavior.nodeType;
    }

    public void InitGridNode(GridNodeType nodeType, GameObject gameObject, Vector2Int pos)
    {
        this.nodeType = nodeType;
        this.pos = pos;
    }

    public void SetHighlight(bool x)
    {
        gfx.ForEach((e)=>
        {
            var mat = e.GetComponent<Renderer>().material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", x ? highlightColor : Color.black);
        });
        this.transform.position += new Vector3(0f, x ? 0.05f : -0.05f, 0f);
    }

    private void OnMouseEnter()
    {
        SetHighlight(true);
    }

    private void OnMouseExit()
    {
        SetHighlight(false);
    }
}