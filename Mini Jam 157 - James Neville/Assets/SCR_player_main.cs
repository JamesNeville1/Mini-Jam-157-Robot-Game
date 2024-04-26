using SCR_izzet_utils.IzzetAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_player_main : MonoBehaviour {
    [SerializeField] private Transform centerOfMass;

    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Components (Read Only)")]
    [SerializeField] [MyReadOnly] private Rigidbody2D rb;

    [Header("Info (Read Only)")]
    [SerializeField] [MyReadOnly] private float inputX;

    public static SCR_player_main instance { get; private set; }

    private void Awake() {
        instance ??= this;
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        rb.centerOfMass = centerOfMass.position;
    }
    private void Update() {
        inputX = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate() {
        rb.AddForce(new Vector2(inputX, 0) * speed, ForceMode2D.Force);

        selfStablise();
    }
    
    private void selfStablise() {

    }
}
