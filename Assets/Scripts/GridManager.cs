using AYellowpaper.SerializedCollections;
using EasyButtons;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public enum GridNodeType
    {
        Wall,
        Empty,
        SpikeTrap,
        PitTrap,
        PushTrap,
        PlayerEntrance,
        Treasure,
        OilTrap,
        BoulderTrap,
        Void,
    }

    public class GridManager : MonoBehaviour
    {

        public List<List<GridNode>> grid;
        public List<Vector2Int> reservedPositions;
        public List<Vector2Int> startPositions;

        public Orientation selectedOrientation = Orientation.North;

        public Vector2Int gridSize;

        public float tileWidth, tileHeight;

        public Vector3 gridWorldCenter;

        [SerializedDictionary("NodeType", "Prefab")]
        public SerializedDictionary<GridNodeType, GameObject> typeToPrefab;

        private Vector2 maxPos;
        public void Awake()
        {
            ResetGrid();
        }
        public bool IsInBounds(Vector2Int pos)
        {
            return !(pos.x < 0 || pos.x >= gridSize.x || pos.y < 0 || pos.y >= gridSize.y);
        }
        public void InitGrid()
        {
            grid = new();
            for (int y = 0; y < gridSize.y; y++)
            {
                grid.Add(new());
                for (int x = 0; x < gridSize.x; x++)
                {
                    grid[y].Add(null);
                }
            }
            maxPos = TileToWorldRaw(gridSize.x, gridSize.y);
        }
        public List<GridNode> GetNeighbors(Vector2Int center)
        {
            List<GridNode> n = new();
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x != 0 && y != 0) continue; // Skip diagonals
                    var cx = center.x + x;
                    var cy = center.y + y;
                    if (gridSize.x <= cx || cx < 0 || cy < 0 || gridSize.y <= cy) continue;
                    n.Add(GridGetAtPosition(new(cx, cy)));
                }
            }
            return n;
        }
        public Vector3 TileToWorld(int x, int y)
        {
            Vector3 gridOffset = new(
                -gridSize.x / 2f + 0.5f,
                0,
                -gridSize.y / 2f + 0.5f 
            );

            return new Vector3(x, 0, y) + gridOffset + gridWorldCenter;
        }
        public Vector3 TileToWorldRaw(int x, int y)
        {
            return new Vector3(x, 0 , y);
        }
        public bool IsAvailable(Vector2Int position)
        {
            return IsInBounds(position) && !reservedPositions.Contains(position);
        }
        public GridNode CreateGridNodeAtPos(GameObject prefab, Vector2Int gpos, GridNodeType type, Orientation orientation = Orientation.Unspecified)
        {
            if(orientation == Orientation.Unspecified) orientation = selectedOrientation;
            if (!IsAvailable(gpos)) return null;
            
            int x = gpos.x;
            int y = gpos.y;

            Vector3 pos = TileToWorld(x, y);
            
            var tile = Instantiate(prefab, pos, Quaternion.identity);

            tile.name = "" + x + "," + y;
            tile.transform.parent = this.gameObject.transform;

            var gn = tile.GetComponent<GridNode>();

            gn.InitGridNode(type, orientation, tile, new Vector2Int(x, y));

            return gn;
        }
        public void GenerateGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    grid[y][x] = CreateGridNodeAtPos(typeToPrefab[GridNodeType.Empty], new Vector2Int(x, y), GridNodeType.Empty);
                }
            }

            reservedPositions = new();
            foreach (var position in startPositions)
            {
                TryReplaceAtPosition(position, CreateGridNodeAtPos(
                    typeToPrefab[GridNodeType.PlayerEntrance],
                    position,
                    GridNodeType.PlayerEntrance,
                    orientation: Orientation.North
                ));
                reservedPositions.Add(position);
            }
        }
        [Button]
        public void ResetGrid()
        {
            if (grid != null)
            {
                for (int i = 0; i < grid.Count; i++)
                {
                    for (int j = grid[i].Count - 1; j >= 0; j--)
                    {
                        Destroy(grid[i][j].gameObject);
                    }
                    grid[i].Clear();
                }

                grid.Clear();
            }
            InitGrid();
            GenerateGrid();
        }
        public bool TryReplaceAtPosition(Vector2Int gpos, GridNode replacement)
        {
            if(!IsAvailable(gpos)) return false;
            Destroy(grid[gpos.y][gpos.x].gameObject);
            grid[gpos.y][gpos.x] = replacement;
            if (replacement.nodeType == GridNodeType.Void) reservedPositions.Add(gpos);
            return true;
        }

        public GridNode FindFirst(GridNodeType type)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    if (grid[y][x].nodeType == type)
                    {
                        return grid[y][x];
                    }
                }
            }
            return null;
        }

        public List<GridNode> FindAll(GridNodeType type)
        {
            List<GridNode> found = new();
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    if (grid[y][x].nodeType == type)
                    {
                        found.Add(grid[y][x]);
                    }
                }
            }
            return found;
        }

        public GridNode GridGetAtPosition(Vector2Int pos)
        {
            return !IsInBounds(pos) ? null : grid[pos.y][pos.x];
        }
        
        public Vector2Int GetRandomPosition(bool forceEmpty = true)
        {
            var availablePositions = new List<Vector2Int>();
            for (var y = 0; y < gridSize.y; y++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (IsAvailable(pos) && (!forceEmpty || grid[y][x].nodeType == GridNodeType.Empty))
                    {
                        availablePositions.Add(pos);
                    }
                }
            }
            if (availablePositions.Count != 0)             return availablePositions[Random.Range(0, availablePositions.Count)];
            Debug.LogWarning("No available positions found in the grid.");
            return Vector2Int.zero; 
        }
    }
}