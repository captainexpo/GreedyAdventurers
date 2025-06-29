using Grid;
using UnityEngine;
using System.Collections.Generic;

public enum Orientation 
{
    Unspecified = -1,
    North,
    East,
    South,
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
    public Orientation orientation;

    private Color highlightColor = new(0.7f, 0.7f, 0.7f);
    public List<GameObject> gfx;

    public GridNodeBase nodeBehavior { get; private set; }

    private void Start()
    {
        nodeBehavior = GetComponent<GridNodeBase>();
        nodeType = nodeBehavior.nodeType;
    }

    public GridNode InitGridNode(GridNodeType nodeType, Orientation orientation, GameObject gameObject, Vector2Int pos)
    {
        this.nodeType = nodeType;
        this.pos = pos;
        transform.rotation = Utils.OrientationToQuaternion(orientation);
        this.orientation = orientation;
        return this;
    }

    public void SetHighlight(bool x, Color? overrideColor = null)
    {
        gfx.ForEach((e)=>
        {
            var mat = e.GetComponent<Renderer>().material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", x ? (overrideColor ?? highlightColor) : Color.black);
        });
        transform.position += new Vector3(0f, x ? 0.05f : -0.05f, 0f);
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