using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public PlayerInput other;

    // Update is called once per frame
    void Update () {
        if (playerInBounds)
        {
            other.setInputInverse(true);
        }

        else
        {
            other.setInputInverse(false);
        }
        //Debug.Log(playerInBounds);
        }

    private bool playerInBounds;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision!");
        if (other.gameObject.tag == "PlayerP")
        {
            playerInBounds = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerP")
        {
            playerInBounds = false;
        }
    }
}
