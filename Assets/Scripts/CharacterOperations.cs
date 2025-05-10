using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public enum CharacterType { Fire, Water, Earth, Air }

public class CharacterOperations : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 2f;
    public float jumpCooldown = 1f;
    private bool isJumpCooldown = false;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    public Sprite[] powerButtons;
    private GameObject powerButtonObject;
    public Tilemap tilemap;
    private Vector3 minBounds;
    private Vector3 maxBounds;

    [Header("Character Type")]
    public CharacterType currentType;
    public RuntimeAnimatorController[] characterControllers;
    private Animator animator;

    [Header("UI Buttons (for mobile)")]
    public ButtonManager leftButton;
    public ButtonManager rightButton;
    public ButtonManager jumpButton;
    public ButtonManager powerButton;

    private float horizontalInput;

    public float powerCooldown = 1f;
    private bool isCooldown = false;

    // for fireball
    [Header("Fire Power")]
    public GameObject fireballPrefab;
    private Vector2 movementDirection;

    // for earth power
    [Header("Earth Power")]
    public EarthPowerManager earthPowerManager;
    public bool canMove = true;

    // for wind power
    [Header("Wind Power")]
    public GameObject windJumpPrefab;
    public AudioClip windSound;

    // for water power
    [Header("Water Power")]
    public AudioClip splashSound;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        TeleportManager.InitializePuddles();

        // Get the bounds of the tilemap 
        Bounds tilemapBounds = tilemap.localBounds;
        minBounds = tilemapBounds.min;
        maxBounds = tilemapBounds.max;

        AssignButtonManagers();
        SetCharacterController();
    }


    private void AssignButtonManagers()
    {
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (canvas != null)
        {
            powerButtonObject = canvas.transform.Find("btn_power")?.gameObject;
            if (powerButtonObject != null) powerButton = powerButtonObject.GetComponent<ButtonManager>();
            else Debug.LogError("Power button not found under Canvas.");

            GameObject leftButtonObject = canvas.transform.Find("btn_move_left")?.gameObject;
            if (leftButtonObject != null) leftButton = leftButtonObject.GetComponent<ButtonManager>();
            else Debug.LogError("Left Button not found!");

            GameObject rightButtonObject = canvas.transform.Find("btn_move_right")?.gameObject;;
            if (rightButtonObject != null) rightButton = rightButtonObject.GetComponent<ButtonManager>();
            else Debug.LogError("Right Button not found!");

            GameObject jumpButtonObject = canvas.transform.Find("btn_jump")?.gameObject;;
            if (jumpButtonObject != null) jumpButton = jumpButtonObject.GetComponent<ButtonManager>();
            else Debug.LogError("Jump Button not found!");
        }
    }


    public void TransformCharacter(Potion.PotionType potionType)
    {
        // Transform the character based on the potion type
        switch (potionType)
        {
            case Potion.PotionType.Fire:
                currentType = CharacterType.Fire;
                break;
            case Potion.PotionType.Water:
                currentType = CharacterType.Water;
                break;
            case Potion.PotionType.Earth:
                currentType = CharacterType.Earth;
                break;
            case Potion.PotionType.Air:
                currentType = CharacterType.Air;
                break;
        }

        // Update the character's prefab to match the new type
        SetCharacterController();
    }

    private void SetCharacterController()
    {
        // validate character controller list
        if (characterControllers == null || characterControllers.Length == 0) {
            Debug.LogError("CharacterControllers array is not set or empty!");
            return;
        }

        if ((int)currentType >= characterControllers.Length) {
            Debug.LogError("No Animator Controller found for the selected character type!");
            return;
        }

        // change Animator's controller
        animator.runtimeAnimatorController = characterControllers[(int)currentType];

        // change the sprite for powerup button 
        Image powerButtonImage = powerButtonObject.GetComponent<Image>();
        if (powerButtonImage != null) powerButtonImage.sprite = powerButtons[(int)currentType];
    }

    void Update()
    {
        if (!canMove) return;
        // Keyboard input
        horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 for A, 1 for D
        if (Input.GetKeyDown(KeyCode.Space) && !isJumpCooldown) {
            Jump();
            StartCoroutine(JumpCooldown());
        }
        if (Input.GetKeyDown(KeyCode.Return) && !isCooldown) {
            ExecutePower();
            StartCoroutine(PowerCooldown());
        }

        // mobile UI input 
        if (leftButton.pressed) horizontalInput = -1;
        if (rightButton.pressed) horizontalInput = 1;
        if (jumpButton.pressed && !isJumpCooldown) {
            Jump();
            StartCoroutine(JumpCooldown());
        }
        if (powerButton.pressed && !isCooldown) {
            ExecutePower();
            StartCoroutine(PowerCooldown());
        }

        if (horizontalInput != 0) movementDirection = new Vector2(horizontalInput, 0);

        // apply horizontal movement if the player is not touching a wall mid-air
        if (isGrounded || !isTouchingWall) {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        ClampPlayerPosition();
    }

    // clamp player position to tilemap width
    private void ClampPlayerPosition() {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x+1.2f, maxBounds.x+1f);
        transform.position = clampedPosition;
    }


    public void Jump()
    {
        if (isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            Debug.Log("jumped");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0) {
            foreach (var contact in collision.contacts) {
                if (contact.normal.y > 0.5f) {
                    isTouchingWall = false; 
                    Debug.Log("landed");
                }

                if (Mathf.Abs(contact.normal.x) > 0.5f) {
                    isTouchingWall = true;
                }

                if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
            }
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0) {
            foreach (var contact in collision.contacts) {
                if (Mathf.Abs(contact.normal.x) > 0.5f && !isGrounded) isTouchingWall = true;
                if (contact.normal.y > 0.5f) isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }


    public void ExecutePower()
    {
        switch (currentType)
        {
            case CharacterType.Fire:
                FirePower();
                break;
            case CharacterType.Water:
                WaterPower();
                break;
            case CharacterType.Earth:
                EarthPower();
                break;
            case CharacterType.Air:
                AirPower();
                break;
        }
    }

    // cooldown time between power usage
    private IEnumerator PowerCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(powerCooldown);
        isCooldown = false;
    }
    // cooldown time between jumps
    private IEnumerator JumpCooldown() {
        isJumpCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        isJumpCooldown = false;
    }

    // execut the fire powerup (fireball)
    private void FirePower() { 
        // default direction if none set
        if (movementDirection == Vector2.zero) movementDirection = new Vector2(1,0);

        Vector3 spawn = transform.position;
        spawn.x += 0.5f * movementDirection.x;
        GameObject fireball = Instantiate(fireballPrefab, spawn, Quaternion.identity);

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        StartCoroutine(DelayedShoot(fireballScript, movementDirection));
    }

    // delay the shoot call until after Start() has executed
    private IEnumerator DelayedShoot(Fireball fireballScript, Vector2 direction) {
        yield return null;
        fireballScript.Shoot(direction);
    }

    // execute the water powerup (teleport)
    private void WaterPower() { 
        GameManager gameManager = FindObjectOfType<GameManager>(); 
        if (TeleportManager.TryTeleport(gameObject, splashSound, gameManager)) {
            Debug.Log("Water power activated and player teleported!");
        }
    }

    // execute earth power (raise ground)
    private void EarthPower() { 
        if (!isGrounded) return;
        Vector3 playerPos = transform.position;
        if (earthPowerManager != null) earthPowerManager.ActivateEarthPower(new Vector3(transform.position.x, transform.position.y - 2, transform.position.z - 5f), gameObject);
    }

    // execute air power (boosted jump)
    private void AirPower() { 
        // wind smoke effect at the player's position
        GameObject wind = Instantiate(windJumpPrefab, transform.position, Quaternion.identity);
        GameManager gameManager = FindObjectOfType<GameManager>(); 
        if (gameManager != null) gameManager.PlaySound(windSound);
        Destroy(wind, 1f); 

        if (isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, 2*jumpForce);
            isGrounded = false;
        }
    }

}
