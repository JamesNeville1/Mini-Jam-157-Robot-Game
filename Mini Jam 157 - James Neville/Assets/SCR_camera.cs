using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_camera : MonoBehaviour {
    private void Update() {
        transform.position  = new Vector3(SCR_player_main.instance.transform.position.x, transform.position.y, -10);
    }
}
