using Grid;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public KeyCode placeKey;
    public KeyCode deleteKey;

    public LayerMask mouseInteractionMask;

    public GridNodeType selectedType;

    public bool isEnabled = false;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HeroManager heroManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private UIManager uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    GridNode GetClickedGridNode(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 4f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mouseInteractionMask))
        {
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
        gridManager.TryReplaceAtPosition(
            clicked.pos,
            gridManager.CreateGridNodeAtPos(
                gridManager.typeToPrefab[selectedType],
                clicked.pos,
                selectedType
            )
        );
        uiManager.UpdateValidityText(CheckValidity());
    }
    public void OnDelete(GridNode clicked)
    {
        gridManager.TryReplaceAtPosition(
            clicked.pos,
            gridManager.CreateGridNodeAtPos(
                gridManager.typeToPrefab[GridNodeType.Empty],
                clicked.pos,
                GridNodeType.Empty
            )
        );
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
}
