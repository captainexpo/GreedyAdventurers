
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

    public List<Vector2Int> visitedPositions;

    public int health;

    public int weakness = 0;
    
    public float speed = 1f; // Game speed

    public bool moving;

    public bool waiting = false;
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

    public void KillSelf(bool force = true)
    {
        print("Killing Hero");
        if (!force)
        {
            StopMoving();
            heroManager.HeroKilled(this);
        }
        Destroy(gameObject);
    }
    public bool WillMove(GridNode node)
    {
        return node.nodeType != GridNodeType.Wall;
    }
    IEnumerator StopGotTreasureColor(GridNode node) 
    {
        print("Stopping highlight for treasure node: " + node.pos);
        yield return new WaitForSeconds(1.0f); 
        node.GetComponent<GridNode>().SetHighlight(false); 
    }
    public DecisionResult MakeDecision(List<GridNode> neighbors)
    {
        // Check if neighbor is a treasure
        foreach (var item in neighbors)
        {
            if (item.nodeType != GridNodeType.Treasure) continue;
            
            visitedPositions.Add(item.pos);
            var n = item.GetComponent<GridNode>();
            n.SetHighlight(true, Color.yellow);
            StartCoroutine(StopGotTreasureColor(item)); // Stop highlight after 0.5 seconds
            return DecisionResult.TreasureFound; 
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
            if (WillMove(nextNode))
            {
                visitedPositions.Add(nextPos);
                return DecisionResult.Move;
            }  
            // If next position is already visited or not movable, stop
            return DecisionResult.Finished;
        }
        // No path to treasure, pick a random valid neighbor
        var filtered = neighbors.Where(e => WillMove(e)).ToList();
        if (filtered.Count == 0) return DecisionResult.Finished; // No moves available
        var choice = filtered[Random.Range(0, filtered.Count)];
        visitedPositions.Add(choice.pos);
        return DecisionResult.Move;
    }
    public bool TryMoveTo(Vector2Int newPos, out Tween tween, bool isInternalLoop = false, bool overrideDefault = false)
    {
        tween = null;
        if (isInternalLoop && waiting) return false; 
        
        var g = gridManager.GridGetAtPosition(newPos);
        var nt = g.nodeType;
        if (nt == GridNodeType.Wall || nt == GridNodeType.Treasure || nt == GridNodeType.Void) return false;

        FaceMovingDir(newPos - gridPos);


        tween = gameObject.transform.DOMove(gridManager.TileToWorld(newPos.x, newPos.y), 1f);
        gridManager.GridGetAtPosition(gridPos).nodeBehavior.OnHeroExit(this);
        gridPos = newPos;
        g.nodeBehavior.OnHeroEnter(this);
        return true;
    }
    public void StartMoving()
    {
        if(moving) return;
        visitedPositions = new() { gridPos };
        moving = true;
        StartCoroutine(MovementLoop());
    }
    IEnumerator MovementLoop()
    {
        while (true)
        {
            var neighbors = gridManager.GetNeighbors(gridPos);
            var result = MakeDecision(neighbors);
            if (result == DecisionResult.Finished) 
                break;
            if(result == DecisionResult.Move || result == DecisionResult.TreasureFound)
            {
                var toMove = visitedPositions.Last();
                TryMoveTo(toMove, out var tween, isInternalLoop: true);
                if (tween != null)
                {
                    yield return tween.WaitForCompletion(); 
                }
            }
            if (result == DecisionResult.TreasureFound)
            {
                StopMoving();
                heroManager.HeroGotTreasure(this);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
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

    public void Damage(int amount)
    {
        health -= amount + weakness;
        weakness = 0;
        if (health <= 0)
        {
            heroManager.HeroKilled(this);
            KillSelf();
        }

        Camera.main.DOShakePosition(0.5f, Vector3.one*0.1f);
    }
}
