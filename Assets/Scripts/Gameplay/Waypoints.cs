using UnityEngine;

/// <summary>
/// Defines the path that nightmare enemies follow through the dreamworld.
/// Child GameObjects of this component become the ordered waypoints.
/// </summary>
public class Waypoints : MonoBehaviour
{
    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    /// <summary>Returns the ordered array of waypoint transforms.</summary>
    public Transform[] GetWaypoints()
    {
        Transform[] points = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            points[i] = transform.GetChild(i);
        return points;
    }

    // ──────────────────────────────────────────────────────────────
    //  Editor Visualisation
    // ──────────────────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        if (transform.childCount < 2) return;

        Gizmos.color = Color.red;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Transform from = transform.GetChild(i);
            Transform to   = transform.GetChild(i + 1);

            Gizmos.DrawLine(from.position, to.position);
            Gizmos.DrawSphere(from.position, 0.15f);
        }

        // Mark the final waypoint (child's destination / dream portal).
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.GetChild(transform.childCount - 1).position, 0.2f);
    }
}
