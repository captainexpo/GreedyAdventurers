
using DG.Tweening;
using Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DecisionResult
{
    Move,
    Finished,
    TreasureFound,
}

public class Hero : MonoBehaviour
{
    public Vector2Int gridPos;

    public HeroType type;

    public HeroManager heroManager;
    private GridManager gridManager;

    public HashSet<Vector2Int> visitedPositions;

    public int health;

    public float speed = 1f; // Game speed

    public bool moving = false;
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
    public DecisionResult MakeDecision(List<GridNode> neighbors)
    {
        // Check if neighbor is a treasure
        foreach (var item in neighbors)
        {
            if(item.nodeType == GridNodeType.Treasure)
            {
                visitedPositions.Add(item.pos);
                return DecisionResult.TreasureFound; 
            }
        }

        // Find all treasures on the grid
        var treasures = gridManager.FindAll(GridNodeType.Treasure);
        List<Vector2Int> shortestPath = null;

        // Find the shortest path to any treasure
        foreach (var treasure in treasures)
        {
            var path = heroManager.pathfinder.FindPath(gridPos, treasure.pos);
            if (path != null && path.Count > 1 && (shortestPath == null || path.Count < shortestPath.Count))
            {
                shortestPath = path;
            }
        }

        if (shortestPath != null)
        {
            // Move to the next position along the shortest path
            var nextPos = shortestPath[1]; // [0] is current position
            var nextNode = gridManager.GridGetAtPosition(nextPos);
            if (!visitedPositions.Contains(nextPos) && WillMove(nextNode))
            {
                visitedPositions.Add(nextPos);
                return DecisionResult.Move;
            }
            else
            {
                // If next position is already visited or not movable, stop
                return DecisionResult.Finished;
            }
        }
        else
        {
            // No path to treasure, pick a random valid neighbor
            var filtered = neighbors.Where(e => !visitedPositions.Contains(e.pos) && WillMove(e)).ToList();
            if (filtered.Count == 0) return DecisionResult.Finished; // No moves available
            var choice = filtered[Random.Range(0, filtered.Count)];
            visitedPositions.Add(choice.pos);
            return DecisionResult.Move;
        }
    }
    public bool TryMoveTo(Vector2Int gpos)
    {
        if (gridManager.GridGetAtPosition(gpos).nodeType == GridNodeType.Wall) return false;
        var tween = gameObject.transform.DOMove(gridManager.TileToWorld(gridPos.x, gridPos.y), 1f);
        FaceMovingDir(gpos - gridPos);
        this.gridPos = gpos;
        return true;
    }
    public void StartMoving()
    {
        if(moving) return; // Already moving, do nothing
        visitedPositions = new HashSet<Vector2Int> { gridPos };
        moving = true;
        StartCoroutine(MovementLoop());
    }
    IEnumerator MovementLoop()
    {
        while (moving)
        {
            var neighbors = gridManager.getNeighbors(gridPos);
            var result = MakeDecision(neighbors);
            
            if (result == DecisionResult.Finished) 
                break;
            
            if(result == DecisionResult.Move || result == DecisionResult.TreasureFound) 
                TryMoveTo(visitedPositions.Last());
            
            if (result == DecisionResult.TreasureFound)
            {
                this.StopMoving();
                heroManager.HeroGotTreasure(this);
                break;
            }
            yield return new WaitForSeconds(speed);
        }
        heroManager.HeroDoneMoving(this);
    }
    public void StopMoving()
    {
        visitedPositions?.Clear();
        moving = false;
    }
    public void FaceMovingDir(Vector2Int dir)
    {
        // (0,0) is down, (1,0) is right, (0,1) is up, (-1,0) is left
        if (dir == Vector2Int.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir == Vector2Int.right)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (dir == Vector2Int.down)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (dir == Vector2Int.left)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
}
