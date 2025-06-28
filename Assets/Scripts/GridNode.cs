using Grid;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GridNode : MonoBehaviour
{
    public GridNodeType nodeType;
    public Vector2Int pos;

    private Color highlightColor = new Color(0.7f, 0.7f, 0.7f);
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