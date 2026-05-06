using UnityEngine;

/// <summary>
/// Handles player input for placing towers on valid map tiles.
/// Attach this component to a persistent manager GameObject in the scene.
///
/// Usage:
///   1. Call <see cref="SelectTower"/> with a tower prefab to begin a placement.
///   2. Click a valid tile to confirm placement (costs the tower's <see cref="Tower.Cost"/>).
///   3. Call <see cref="CancelPlacement"/> or press Escape to abort.
/// </summary>
public class TowerPlacement : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Inspector
    // ──────────────────────────────────────────────────────────────

    [Tooltip("Layer mask for tiles the player is allowed to build on.")]
    [SerializeField] private LayerMask buildableTileLayer;

    [Tooltip("Semi-transparent ghost prefab shown while the player is choosing a spot.")]
    [SerializeField] private GameObject ghostPrefab;

    // ──────────────────────────────────────────────────────────────
    //  State
    // ──────────────────────────────────────────────────────────────

    private GameObject selectedPrefab;
    private GameObject ghostInstance;
    private Camera mainCam;
    private bool isPlacing = false;

    // ──────────────────────────────────────────────────────────────
    //  Unity Lifecycle
    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (!isPlacing) return;

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
            TryPlaceTower();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacement();
    }

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Begin placement mode for the given tower prefab.</summary>
    public void SelectTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;

        selectedPrefab = towerPrefab;
        isPlacing = true;

        if (ghostPrefab != null)
            ghostInstance = Instantiate(ghostPrefab);
    }

    /// <summary>Cancel an in-progress placement.</summary>
    public void CancelPlacement()
    {
        isPlacing = false;
        selectedPrefab = null;

        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Private
    // ──────────────────────────────────────────────────────────────

    private void UpdateGhostPosition()
    {
        Vector3 worldPos = GetMouseWorldPosition();

        if (ghostInstance != null)
            ghostInstance.transform.position = SnapToGrid(worldPos);
    }

    private void TryPlaceTower()
    {
        Vector3 worldPos   = GetMouseWorldPosition();
        Vector3 snappedPos = SnapToGrid(worldPos);

        // Check the tile is buildable.
        Collider2D hit = Physics2D.OverlapPoint(snappedPos, buildableTileLayer);
        if (hit == null)
        {
            Debug.Log("[TowerPlacement] Cannot place here – tile is not buildable.");
            return;
        }

        // Check the player can afford it.
        Tower prefabTower = selectedPrefab.GetComponent<Tower>();
        if (prefabTower == null || !CurrencyManager.Instance.TrySpend(prefabTower.Cost))
        {
            Debug.Log("[TowerPlacement] Not enough Dream Coins.");
            return;
        }

        Instantiate(selectedPrefab, snappedPos, Quaternion.identity);
        CancelPlacement();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(mainCam.transform.position.z);
        return mainCam.ScreenToWorldPoint(screenPos);
    }

    /// <summary>Snap a world position to the nearest 1-unit grid cell centre.</summary>
    private static Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.FloorToInt(pos.x) + 0.5f,
            Mathf.FloorToInt(pos.y) + 0.5f,
            0f
        );
    }
}
