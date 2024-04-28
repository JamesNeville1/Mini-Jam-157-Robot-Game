using SCR_izzet_utils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AI_human : MonoBehaviour {
    
    private const int startingDirection = 1; //Always go right to start

    [Header("Main")]
    [SerializeField] private float speed;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float wallCheckRange;

    [Header("Other")]
    [SerializeField] [Tooltip("Runs every 0.05 seconds")] private float scaleByOnDeath;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private SCR_unit_animation ani;
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private Collider2D col;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;

    [Header("Info (Read Only)")]
    [SerializeField] [MyReadOnly] private int currentDirection;

    private void Start() {
        ani = GetComponentInChildren<SCR_unit_animation>();
        ani.setup("human");

        rb = GetComponent<Rigidbody2D>();

        col = GetComponentInChildren<Collider2D>();

        sr = GetComponentInChildren<SpriteRenderer>();

        currentDirection = startingDirection;
    }
    private void Update() {
        transform.position += new Vector3(currentDirection, 0) * speed * Time.deltaTime;
        ani.play(SCR_unit_animation.AnimationType.WALK);
    }
    private void OnTriggerStay2D(Collider2D collision) {
        RaycastHit2D groundedBoxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, new Vector2(currentDirection, 0), .25f, ground);
        if (groundedBoxCast.collider != null) {
            print("ChangeDir");
            currentDirection = -currentDirection;
            sr.flipX = !sr.flipX;
        }
    }
    public void hit() {
        StartCoroutine(death());
    }

    private IEnumerator death() {
        bool aliveForDisplay = true;
        while (aliveForDisplay) {
            yield return new WaitForSeconds(0.05f);
            sr.gameObject.transform.localScale -= new Vector3(0, scaleByOnDeath);
            if (sr.gameObject.transform.localScale.y <= .05f) aliveForDisplay = false;
        }
        Destroy(this.gameObject);
    }
}