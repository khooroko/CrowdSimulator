using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor {

    // Crude way of getting floor number for now
	public static int GetFloor(Transform transform) {
        return transform.position.x < 52 ? 1 : 2;
    }
}
