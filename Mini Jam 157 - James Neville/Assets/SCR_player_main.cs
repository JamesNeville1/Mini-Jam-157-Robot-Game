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

    [Header("Other")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private float groundedSpriteRotationSpeed;
    [SerializeField] private float airborneSpriteRotationSpeed;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField][MyReadOnly] private Collider2D col;

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
        jumpCheck();
        spriteRotation();
    }
    private void FixedUpdate() {
        rb.AddForce(new Vector2(inputX, 0) * speed, ForceMode2D.Force);
    }

    private void playerWalkCalculations() {
        if(inputX > 0 && rb.velocity.x < 0 || inputX < 0 && rb.velocity.x > 0) { //If player is trying to go in opposite direction of force
            inputX = Input.GetAxisRaw("Horizontal") * counterCorrectingSpeed;
            print(inputX);
        }
        else {
            inputX = Input.GetAxisRaw("Horizontal");
        }

        //if (canJump) {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -velocityMax, velocityMax), rb.velocity.y);
        //}
    }
    private void jumpCheck() {        
        RaycastHit2D groundedBoxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, .1f, ground);
        if (groundedBoxCast.collider != null && Mathf.Abs(rb.velocity.y) <= .01f) {
            grounded = true;
        }
        else {
            grounded = false;
        }

        if (Input.GetKey(KeyCode.Space)) {
            if (grounded) {
                jump();
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
        Quaternion unclampedSlerp = Quaternion.Lerp(sr.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, Mathf.Clamp(inputX, -1, 1) * 30), rotationSpeed * Time.deltaTime);

        sr.gameObject.transform.rotation = unclampedSlerp;
        
        //sr.gameObject.transform.rotation = Quaternion.Lerp(sr.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, 0f), airborneSpriteRotationSpeed * Time.deltaTime);
    }
}
