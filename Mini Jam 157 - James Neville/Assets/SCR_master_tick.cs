using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_master_tick : MonoBehaviour {

    public static event Action onTick;

    private const float tickTimerMax = .2f; //.2f for the game

    private int tick;
    private float tickTimer;

    private void Awake() {
        tick = 0;
    }

    private void Update() {
        tickTimer += Time.deltaTime;

        if(tickTimer >= tickTimerMax ) {
            tickTimer -= tickTimerMax;
            tick++;
            if(onTick != null) { onTick.Invoke(); }
        }
    }
}
