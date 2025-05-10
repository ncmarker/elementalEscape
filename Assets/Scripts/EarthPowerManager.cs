using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine;

public class EarthPowerManager : MonoBehaviour
{
    public Tilemap groundTilemap; 
    public Tile newRockTile; 
    public Tile allGroundTile;
    private List<GameObject> boxes = new List<GameObject>();
    public AudioClip rockSound;

    // fetch current boxes in scene
    private void GetBoxes() {
        GameObject[] boxObjects = GameObject.FindGameObjectsWithTag("Box");
        boxes = new List<GameObject>(boxObjects);
    }

    public void ActivateEarthPower(Vector3 spawnPosition, GameObject player) {
        if (boxes.Count == 0) GetBoxes();

        Vector3 playerPos = player.transform.position;
        playerPos.z = -7;
    
        // check if player is too close to a box to use power
        foreach (GameObject box in boxes) {
            float distance = Vector3.Distance(player.transform.position, box.transform.position);
            if (distance < 1.25) {
                Debug.Log("Cannot raise ground: Box is too close.");
                return;
            }
        }

        // use power if valid distance
        StartCoroutine(RaiseGroundGradually(player.transform.position, player, 1f));
    }

    public IEnumerator RaiseGroundGradually(Vector3 spawnPosition, GameObject player, float duration)
    {
        player.GetComponent<CharacterOperations>().canMove = false;
        Vector3Int tilePosition = groundTilemap.WorldToCell(player.transform.position);

        // check if there are 3+ open slots above the current tile
        if (groundTilemap.GetTile(tilePosition) == null && 
            groundTilemap.GetTile(tilePosition + Vector3Int.up) == null && 
            groundTilemap.GetTile(tilePosition + 2 * Vector3Int.up) == null && 
            !TeleportManager.GetPlayerOnPuddle(player))
        {
            int numberOfTiles = 2;  
            float timePerTile = duration / numberOfTiles;

            GameManager gameManager = FindObjectOfType<GameManager>(); 
            if (gameManager != null) gameManager.PlaySound(rockSound);

            // gradually add tiles
            for (int i = 0; i < numberOfTiles; i++)
            {
                groundTilemap.SetTile(tilePosition - Vector3Int.up + Vector3Int.up * i, allGroundTile);  
                groundTilemap.SetTile(tilePosition + Vector3Int.up * i, newRockTile);  
                groundTilemap.RefreshAllTiles();  

                // move the player upward as the ground rises
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);

                yield return new WaitForSeconds(timePerTile);
            }
        }
        player.GetComponent<CharacterOperations>().canMove = true;
    }

}
