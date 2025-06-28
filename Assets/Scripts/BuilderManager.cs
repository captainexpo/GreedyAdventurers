using AYellowpaper.SerializedCollections;
using Grid;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public KeyCode placeKey;
    public KeyCode deleteKey;

    public LayerMask mouseInteractionMask;

    public GridNodeType selectedType;

    public bool isEnabled = false;

    public int funds = 0;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HeroManager heroManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private UIManager uiManager;

    [SerializedDictionary("NodeType", "Cost")]
    public SerializedDictionary<GridNodeType, int> costs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager.SetBuildOptionDropdownSelectionCallback(SelectionCallback);
        foreach (KeyValuePair<GridNodeType, int> item in costs)
        {
            uiManager.AddBuildOptionDropdown(
                title: item.Key.ToString(), 
                text: (item.Value >= 0 ? "-" : "+") + Mathf.Abs(item.Value).ToString() + "$ " + item.Key.ToString()
            );
        }
    }

    GridNode GetClickedGridNode(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 4f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mouseInteractionMask))
        {
            if (!hit.rigidbody) return null; 
            if (hit.rigidbody.gameObject.TryGetComponent(out GridNode node)) return node;
        }
        return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return; // TODO: Something else
        if (Input.GetKeyDown(placeKey))
        {
            var node = GetClickedGridNode(Input.mousePosition);
            if (node != null) OnPlace(node);
        }
        if (Input.GetKeyDown(deleteKey))
        {
            var node = GetClickedGridNode(Input.mousePosition);
            if (node != null) OnDelete(node);
        }
    }
    public bool CheckValidity()
    {
        var allTreasures = gridManager.FindAll(GridNodeType.Treasure);

        if (funds < 0) return false;
        if (allTreasures.Count == 0) return false; // No treasure placed

        foreach (var treasure in allTreasures)
        {
            var path = pathfinder.FindPath(heroManager.activeHero.gridPos, treasure.pos);
            if (path == null) return false; // No path to treasure found
#if UNITY_EDITOR
            pathfinder.DebugDrawPath(path);
#endif
        }

        return true;
    }
    public void OnPlace(GridNode clicked)
    {
        var old = gridManager.GridGetAtPosition(clicked.pos).nodeType;
        var succeded = gridManager.TryReplaceAtPosition(
            clicked.pos,
            gridManager.CreateGridNodeAtPos(
                gridManager.typeToPrefab[selectedType],
                clicked.pos,
                selectedType
            )
        );
        if (succeded)
        {
            UpdateFunds(-costs[old]); // Delete the node first, if it exists
            UpdateFunds(costs[selectedType]);
        }
        uiManager.UpdateValidityText(CheckValidity());
    }
    public void UpdateFunds(int difference)
    {
        print("Updating funds: " + difference);
        funds -= difference;
        uiManager.UpdateValidityText(CheckValidity());
    }
    public void OnDelete(GridNode clicked)
    {
        var succeded = gridManager.TryReplaceAtPosition(
            clicked.pos,
            gridManager.CreateGridNodeAtPos(
                gridManager.typeToPrefab[GridNodeType.Empty],
                clicked.pos,
                GridNodeType.Empty
            )
        );
        if (succeded) UpdateFunds(-costs[clicked.nodeType]);
        uiManager.UpdateValidityText(CheckValidity());
    }
    public void Enable()
    {
        if (isEnabled) return; // Already enabled
        isEnabled = true;
        uiManager.SetBuilderUI(true);
    }

    public void Disable()
    {
        isEnabled = false;
        uiManager.SetBuilderUI(false);
    }

    public void SelectionCallback(string title)
    {
        switch(title)
        {
            case "Wall":
                selectedType = GridNodeType.Wall;
                break;
            case "Empty":
                selectedType = GridNodeType.Empty;
                break;
            case "SpikeTrap":
                selectedType = GridNodeType.SpikeTrap;
                break;
            case "PitTrap":
                selectedType = GridNodeType.PitTrap;
                break;
            case "PushTrap":
                selectedType = GridNodeType.PushTrap;
                break;
            case "Treasure":
                selectedType = GridNodeType.Treasure;
                break;
            default:
                Debug.LogWarning($"Unknown build option: {title}");
                return; // Ignore unknown options
        }
    }
}
