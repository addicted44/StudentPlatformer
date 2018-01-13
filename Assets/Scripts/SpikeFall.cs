using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFall : MonoBehaviour {

    public float fallSpeed = 8.0f;

	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
    }
}
