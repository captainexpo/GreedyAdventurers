using Grid;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridNode : MonoBehaviour
{
    public GridNodeType nodeType { get; private set; }
    public Vector2Int pos { get; private set; }

    private Color highlightColor = Color.white;
    public List<GameObject> gfx;
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