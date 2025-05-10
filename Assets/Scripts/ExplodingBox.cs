using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ExplodingBox : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Explode()
    { 
        // trigger explode animation
        animator.SetTrigger("explode");
    }

    public void OnExplodeComplete()
    {
        // destroy the box after animation
        Destroy(gameObject);
    }
}
