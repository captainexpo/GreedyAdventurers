using UnityEngine;
using Grid;
using System.Collections.Generic;
using EasyButtons;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public List<GridNodeType> blockingTypes;

    public List<List<float>> distances;

#if UNITY_EDITOR
    [Header("Debug Settings")]
    public float debugDrawTime = 5f;
    public float debugDrawYOffset = 2f;
#endif


    bool DoesBlock(GridNodeType type) => blockingTypes.Contains(type);
    List<Vector2Int> Dijkstras(Vector2Int start, Vector2Int end)
    {
        var previous = new Dictionary<Vector2Int, Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        // Min-heap using sorted list for simplicity
        var frontier = new List<Vector2Int> { start };

        while (frontier.Count > 0)
        {
            // Sort by distance
            frontier.Sort((a, b) => distances[a.y][a.x].CompareTo(distances[b.y][b.x]));
            var current = frontier[0];
            frontier.RemoveAt(0);

            if (current == end) break;
            if (visited.Contains(current)) continue;

            visited.Add(current);

            foreach (var neighbor in gridManager.getNeighbors(current))
            {
                if (visited.Contains(neighbor.pos)) continue;
                var node = gridManager.grid[neighbor.pos.y][neighbor.pos.x];
                if (DoesBlock(node.nodeType)) continue;

                float tentative = distances[current.y][current.x] + 1f;

                if (tentative < distances[neighbor.pos.y][neighbor.pos.x])
                {
                    distances[neighbor.pos.y][neighbor.pos.x] = tentative;
                    previous[neighbor.pos] = current;
                    frontier.Add(neighbor.pos);
                }
            }
        }

        // Reconstruct path
        var path = new List<Vector2Int>();
        var cur = end;

        if (!previous.ContainsKey(cur)) return null; // No path found

        while (cur != start)
        {
            path.Add(cur);
            cur = previous[cur];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
    [Button]
    public List<Vector2Int> FindPath(Vector2 start, Vector2 end)
    {
        // Init distances
        distances = new();
        var grid = gridManager.grid;
        for (int y = 0; y < grid.Count; y++)
        {
            distances.Add(new());
            for (int x = 0; x < grid[y].Count; x++)
            {
                distances[y].Add(start == new Vector2Int(x, y) ? 0f : float.PositiveInfinity);
            }
        }
        var p = Dijkstras(
            new Vector2Int((int)start.x, (int)start.y),
            new Vector2Int((int)end.x, (int)end.y)
        );
        if (p == null) return null;
        foreach (var item in p)
        {
            print(item);
        }
        return p;
    }

#if UNITY_EDITOR
    public void DebugDrawPath(List<Vector2Int> path)
    {
        for(int i = 0; i < path.Count - 1; i++)
        {
            var start = gridManager.TileToWorld(path[i].x, path[i].y);
            var end = gridManager.TileToWorld(path[i + 1].x, path[i + 1].y);
            Debug.DrawLine(start + Vector3.up * debugDrawYOffset, end + Vector3.up * debugDrawYOffset, Color.red, debugDrawTime);
        }
    }
#endif
}
