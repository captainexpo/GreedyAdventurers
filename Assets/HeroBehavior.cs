
using DG.Tweening;
using Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Vector2Int gridPos;

    public HeroType type;

    public HeroManager heroManager;
    private GridManager gridManager;

    public HashSet<Vector2Int> visitedPositions;

    public int health;

    public static Hero New(Vector2Int gridPos, GameObject prefab, GridManager gm, HeroManager hm, HeroType type = HeroType.Normal)
    {
        var heroGm = Instantiate(prefab, gm.TileToWorld(gridPos.x, gridPos.y), Quaternion.identity);
        var heroScript = heroGm.GetComponent<Hero>();

        heroScript.gridPos = gridPos;
        heroScript.gridManager = gm;
        heroScript.type = type;
        heroScript.heroManager = hm;

        return heroScript;
    }

    public void KillSelf()
    {
        Destroy(gameObject);
    }
    public bool WillMove(GridNode node)
    {
        return node.nodeType != GridNodeType.Wall;
    }
    public bool MakeDecision(List<GridNode> neighbors)
    {
        var filtered = neighbors.Where((e) => !visitedPositions.Contains(e.pos) && WillMove(e));
        if (filtered.Count() == 0) return false; // Stop if there's no more places to move
        var choice = filtered.ToList()[Random.Range(0, filtered.Count())];

        visitedPositions.Add(choice.pos);
        return true;
    }

    public bool TryMoveTo(Vector2Int gpos)
    {
        if (gridManager.GridGetAtPosition(gpos).nodeType == GridNodeType.Wall) return false;
        gameObject.transform.DOMove(gridManager.TileToWorld(gridPos.x, gridPos.y), 1f);
        this.gridPos = gpos;
        return true;
    }
}
