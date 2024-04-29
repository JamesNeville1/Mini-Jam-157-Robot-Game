using SCR_izzet_utils.IzzetAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_AI_human : MonoBehaviour {
    
    private const int startingDirection = 1; //Always go right to start

    [Header("Main")]
    [SerializeField] private float speed;
    [SerializeField] private LayerMask toBump;
    [SerializeField] private float wallCheckRange;
    [SerializeField] private int giveEnergy;

    [Header("Other")]
    [SerializeField] [Tooltip("Runs every 0.05 seconds")] private float scaleByOnDeath;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private SCR_unit_animation ani;
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;
    [SerializeField] [MyReadOnly] private Collider2D col;
    [SerializeField] [MyReadOnly] private SpriteRenderer sr;

    [Header("Info (Read Only)")]
    [SerializeField] [MyReadOnly] private int currentDirection;

    //
    private Action onUpdate;

    private void Start() {
        ani = GetComponentInChildren<SCR_unit_animation>();
        ani.setup("human");

        rb = GetComponent<Rigidbody2D>();

        col = GetComponentInChildren<Collider2D>();

        sr = GetComponentInChildren<SpriteRenderer>();

        currentDirection = startingDirection;

        onUpdate = () => {
            transform.position += new Vector3(currentDirection, 0) * speed * Time.deltaTime;
            ani.play(SCR_unit_animation.AnimationType.WALK);

            RaycastHit2D groundedBoxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, new Vector2(currentDirection, 0), .5f, toBump);
            if (groundedBoxCast.collider == null) return;

            bool shouldTurn =
                groundedBoxCast.collider.gameObject.layer == LayerMask.NameToLayer("Ground") //||
                                                                                             //groundedBoxCast.collider.gameObject.layer == LayerMask.NameToLayer("AI Slam")
                ;
            if (shouldTurn)
            {
                //print("ChangeDir");
                currentDirection = -currentDirection;
                sr.flipX = !sr.flipX;
            }
        };
    }
    private void Update() {
        onUpdate.Invoke();
    }
    public void hit() {
        StartCoroutine(death());
        onUpdate = () => { };
        SCR_player_main.instance.adjustEnergy(giveEnergy);
        Destroy(col.gameObject);
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