using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class TeleportManager
{
    private static List<GameObject> puddles = new List<GameObject>();
    private static GameObject lastPuddle;

    public static void InitializePuddles()
    {
        // Find all puddles tagged "Puddle" in the scene
        GameObject[] puddleObjects = GameObject.FindGameObjectsWithTag("Puddle");
        puddles = new List<GameObject>(puddleObjects);

        if (puddles.Count != 2) Debug.LogWarning("Expected exactly 2 puddles, but found: " + puddles.Count);
        else lastPuddle = puddles[0]; 
    }

    // attempts to execute the teleport
    public static bool TryTeleport(GameObject player, AudioClip splashSound, GameManager gameManager)
    {
        if (puddles.Count != 2) {
            Debug.LogError("Teleportation requires exactly 2 puddles.");
            return false;
        }

        GameObject currentPuddle = GetPlayerOnPuddle(player);
        if (currentPuddle == null) {
            Debug.Log("Player is not on a puddle.");
            return false;
        }

        // play splash sound
        if (gameManager != null) gameManager.PlaySound(splashSound);

        TeleportPlayer(player, currentPuddle);
        return true;
    }

    // validates that the player is actually on a puddle
    public static GameObject GetPlayerOnPuddle(GameObject player)
    {
        foreach (GameObject puddle in puddles) {
            float distance = Vector2.Distance(player.transform.position, puddle.transform.position);
            if (distance < 0.5f) return puddle;
            
        }
        return null;
    }

    // executes the teleport action
    private static void TeleportPlayer(GameObject player, GameObject currentPuddle)
    {
        GameObject targetPuddle = currentPuddle == puddles[0] ? puddles[1] : puddles[0];

        TriggerSplashAnimation(currentPuddle);
        TriggerSplashAnimation(targetPuddle);

        float animationDelay = 0.5f; 
        Vector3 targetPosition = targetPuddle.transform.position;
        TeleportManagerBehavior.Instance.DelayedTeleport(player, targetPosition, animationDelay);
    }

    // helper to initiate the splash animation
    private static void TriggerSplashAnimation(GameObject puddle)
    {
        Animator animator = puddle.GetComponent<Animator>();
        if (animator != null) animator.SetTrigger("teleport");
        else Debug.LogWarning($"No Animator found on puddle {puddle.name}");
    }
}
