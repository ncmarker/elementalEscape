using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public GameObject smokeEffect;
    public AudioClip explodeSound; 
    public AudioClip shootSound;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // shoot the fireball in a given direction
    public void Shoot(Vector2 direction)
    {
        // play shoot sound
        GameManager gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager != null) gameManager.PlaySound(shootSound);

        rb.velocity = direction.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // play explode sound 
        GameManager gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager != null) gameManager.PlaySound(explodeSound);

        // check if fireball hits a box, if so explode it
        ExplodingBox box = collision.gameObject.GetComponent<ExplodingBox>();
        if (box != null) box.Explode();
        
        // Spawn the smoke effect at the fireball's position
        GameObject smoke = Instantiate(smokeEffect, transform.position, Quaternion.identity);
        Destroy(smoke, 1f);

        Destroy(gameObject);
    }
}
