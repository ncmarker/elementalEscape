using System.Collections;
using UnityEngine;

public class TeleportManagerBehavior : MonoBehaviour
{
    public static TeleportManagerBehavior Instance;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DelayedTeleport(GameObject player, Vector3 targetPosition, float delay)
    {
        StartCoroutine(TeleportWithHiding(player, targetPosition, delay));
    }

    private IEnumerator TeleportWithHiding(GameObject player, Vector3 targetPosition, float delay)
    {
        // hide player during teleport delay
        TogglePlayerVisibility(player, false);
        yield return new WaitForSeconds(delay);
        player.transform.position = targetPosition;
        TogglePlayerVisibility(player, true);
    }

    // helper for show/hiding the player during teleport
    private void TogglePlayerVisibility(GameObject player, bool isVisible)
    {
        Renderer renderer = player.GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = isVisible;
        
        foreach (Renderer childRenderer in player.GetComponentsInChildren<Renderer>()) {
            childRenderer.enabled = isVisible;
        }
    }
}
