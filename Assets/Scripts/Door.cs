using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private Animator doorAnimator;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if player goes in front of door, play animation and return to levels page
        if (collision.CompareTag("Player")) {
            doorAnimator.SetTrigger("openDoor"); 
            StartCoroutine(WaitForAnimationAndReturnToLevels());
        }
    }

    private IEnumerator WaitForAnimationAndReturnToLevels()
    {
        // wait until the door open animation to complete
        float animationLength = doorAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(2f);

        if (gameManager != null) gameManager.LevelCompleted();
    }
}
