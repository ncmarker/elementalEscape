using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public enum PotionType { Fire, Water, Earth, Air }
    public PotionType potionType; 
    public Sprite[] potionSprites; 

    public GameObject transformPrefab;
    public float transformDuration = 1f;
    public AudioClip potionSound;
    
    private void Start()
    {
        // set the potion's sprite based on its type
        GetComponent<SpriteRenderer>().sprite = potionSprites[(int)potionType];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // play sound
            GameManager gameManager = FindObjectOfType<GameManager>(); 
            if (gameManager != null) gameManager.PlaySound(potionSound); 
            
            // smoke effect at the potion's position
            GameObject smoke = Instantiate(transformPrefab, transform.position, Quaternion.identity);
            Destroy(smoke, 1f);

            Destroy(gameObject);
            // change player 
            other.GetComponent<CharacterOperations>().TransformCharacter(potionType);
        }
    }
}
