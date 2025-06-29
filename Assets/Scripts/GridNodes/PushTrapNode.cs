using System.Collections;
using DG.Tweening;
using Grid;
using UnityEngine;

public class PushTrapNode : MonoBehaviour, GridNodeBase
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HeroManager heroManager;
    private GridNode nodeScript;
    public bool trapEnabled = true;
    private float defaultY;
    public GridNodeType nodeType { get; } = GridNodeType.OilTrap;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        heroManager = FindFirstObjectByType<HeroManager>();
        nodeScript = GetComponent<GridNode>();
        defaultY = nodeScript.gfx[0].transform.localPosition.y;
    }

    public void OnHeroEnter(Hero hero)
    {
        if (!trapEnabled) return;

        Vector2Int newPos = hero.gridPos + Utils.OrientationToVector(nodeScript.orientation);
        if (hero.TryMoveTo(newPos, out _, overrideDefault: true))
        {
            // Immediately update the visual position to match the grid position
            hero.transform.position = gridManager.TileToWorld(newPos.x, newPos.y);

            // Then start the tween animation for smoother movement
            hero.transform.DOMove(gridManager.TileToWorld(newPos.x, newPos.y), 0.3f);
            StartCoroutine(PlayPushAnimation());
            trapEnabled = false;
        }
    }

    private IEnumerator PlayPushAnimation()
    {
        // Tween one
        var pushGfx = nodeScript.gfx[0];
        var t1 = pushGfx.transform.DOLocalMove(new(0f, -0.1f, -0.3f), 0.3f);
        var t2 = pushGfx.transform.DOLocalRotate(new(30f, 0f, 0f), 0.3f);
        yield return t1.WaitForCompletion();
        pushGfx.transform.DOLocalMove(new Vector3(0f, -1f, 0f), 0.3f); // Hide from view
        pushGfx.transform.DOLocalRotate(Vector3.zero, 0.3f);
    }

    public void OnStartHeroMove(Hero hero)
    {
        trapEnabled = true;
        nodeScript.gfx[0].transform.localPosition = new Vector3(0f, defaultY, 0f);
    }

    public void OnStartBuild()
    {
        trapEnabled = true;
        nodeScript.gfx[0].transform.localPosition = new Vector3(0f, defaultY, 0f);
    }
}