using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrap : MonoBehaviour {
    public float fallSpeed = 8.0f;
    private bool playerInBounds;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.tag == "PlayerP")
        {
            playerInBounds = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.tag == "PlayerP")
        {
            playerInBounds = false;

        }
    }

    // Update is called once per frame
    void Update () {

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

    }
}
