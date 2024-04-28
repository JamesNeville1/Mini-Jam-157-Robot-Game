using SCR_izzet_utils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_player_main : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float counterCorrectingSpeed;
    [SerializeField] private float velocityMax;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpForceX;
    
    [Header("Children References")]
    [SerializeField] private GameObject playerShootVFX;

    [SerializeField] private Transform shootPoint;

    [Header("Other")]
    [SerializeField] private LayerMask boxCastLayerMask;
    [SerializeField] private string groundLayer;
    [SerializeField] private float groundedSpriteRotationSpeed;
    [SerializeField] private float airborneSpriteRotationSpeed;
    [SerializeField] private float maximumSpriteRotationRange;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField] [MyReadOnly] private Collider2D col;

    [Header("Info (Read Only)")]
    [SerializeField] [MyReadOnly] private float inputX;
    [SerializeField] [MyReadOnly] private bool grounded;

    public static SCR_player_main instance { get; private set; }

    private void Awake() {
        instance ??= this;
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponentInChildren<Collider2D>();

        
    }
    private void Update() {
        playerWalkCalculations();

        playerShootCheck();

        spriteRotation();
    }
    private void OnCollisionStay2D(Collision2D collision) {
        jumpCheck(); //Optimisation
    }
    private void FixedUpdate() {
        rb.AddForce(new Vector2(inputX, 0) * speed, ForceMode2D.Force);
    }

    private void playerWalkCalculations() {
        inputX = Input.GetAxisRaw("Horizontal");
        if (inputX > 0 && rb.velocity.x < 0 || inputX < 0 && rb.velocity.x > 0) { //If player is trying to go in opposite direction of force
            inputX *= counterCorrectingSpeed;
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -velocityMax, velocityMax), rb.velocity.y); //This causes the velocity bug, y is overriden
    }
    private void playerShootCheck() {
        if(Input.GetMouseButtonDown(0)) {
            //shoot();
        }
    }
    private void shoot() {
        playerShootVFX.transform.position = shootPoint.transform.position;
        playerShootVFX.SetActive(false); playerShootVFX.SetActive(true);
    }
    private void jumpCheck() {        
        RaycastHit2D groundedBoxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size * 1.2f, 0, Vector2.down, .25f, boxCastLayerMask);
        if (groundedBoxCast.collider != null) {
            SCR_AI_human enemyScript = groundedBoxCast.collider.gameObject.GetComponentInParent<SCR_AI_human>();
            if (enemyScript != null) {
                enemyScript.hit();
                print("a");
            }
            else if(groundedBoxCast.collider.gameObject.layer == LayerMask.NameToLayer(groundLayer) && Mathf.Abs(rb.velocity.y) <= .01f) {
                grounded = true;
            }
        }
        else {
            grounded = false;
        }

        if (Input.GetKey(KeyCode.Space)) {
            if (grounded) {
                jump();
                grounded = false; //Fix l8r
            }
        }
    }
    private void jump() {
        rb.AddForce(new Vector2(0, 1) * jumpForce, ForceMode2D.Impulse);

        if(Mathf.Abs(rb.velocity.x) > 0.3f) {
            float dir;
            dir = rb.velocity.normalized.x;
            rb.AddForce(new Vector2(dir, 0) * jumpForceX * Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0, 2), ForceMode2D.Impulse);
        }
    }
    private void spriteRotation() {
        float rotationSpeed = (grounded) ? groundedSpriteRotationSpeed : airborneSpriteRotationSpeed;
        Quaternion unclampedSlerp = Quaternion.Lerp(sr.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, Mathf.Clamp(inputX, -1, 1) * maximumSpriteRotationRange), rotationSpeed * Time.deltaTime);

        sr.gameObject.transform.rotation = unclampedSlerp;
        
        //sr.gameObject.transform.rotation = Quaternion.Lerp(sr.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, 0f), airborneSpriteRotationSpeed * Time.deltaTime);
    }
}
