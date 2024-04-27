using SCR_izzet_utils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_player_main : MonoBehaviour {
    [SerializeField] private Transform centerOfMass;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float velocityMax;
    [SerializeField] private float jumpForce;

    [Header("Other")]
    [SerializeField] private LayerMask ground;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;
    [SerializeField][MyReadOnly] private Collider2D col;

    [Header("Info (Read Only)")]
    [SerializeField] [MyReadOnly] private float inputX;
    [SerializeField] [MyReadOnly] private bool canJump;

    public static SCR_player_main instance { get; private set; }

    private void Awake() {
        instance ??= this;
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponentInChildren<Collider2D>();

        rb.centerOfMass = centerOfMass.position;
    }
    private void Update() {
        playerWalkCalculations();
        jumpCheck();
    }
    private void FixedUpdate() {
        rb.AddForce(new Vector2(inputX, 0) * speed, ForceMode2D.Force);
    }

    private void playerWalkCalculations() {
        inputX = Input.GetAxis("Horizontal");

        Vector3 unclampedSlerp = Vector3.Slerp(sr.gameObject.transform.up, -rb.velocity.normalized, Time.deltaTime);
        Vector3 clampedSlerp = new Vector3(Mathf.Clamp(unclampedSlerp.x, -.3f, .3f), Mathf.Clamp(unclampedSlerp.y, -3, 3));

        sr.gameObject.transform.up = clampedSlerp; //Quaternion.Euler(0, 0, Mathf.Clamp(rb.velocity.x * 10, -8, 8));

        //rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, velocityMax).x, rb.velocity.y);
    }
    private void jumpCheck() {
        if (Input.GetKey(KeyCode.Space)) {
            if (canJump) {
                jump();
            }
        }
        
        RaycastHit2D groundedBoxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, .1f, ground);
        if (groundedBoxCast.collider != null) {
            canJump = true;
        }
    }
    private void jump() {
        rb.AddForce(new Vector2(0, 1) * jumpForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        canJump = false;
    }
}
