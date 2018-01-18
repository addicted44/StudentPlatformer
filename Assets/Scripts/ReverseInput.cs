using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseInput : MonoBehaviour {

    // Use this for initialization

    public Player player;

	void Start () {
		
	}

    // Update is called once per frame
    void Update () {
        if (player.transform.position.x > 74.6 && player.transform.position.x < 95.5 && player.transform.position.y > -27 && player.transform.position.y < -16)
        {
            player.GetComponent<PlayerInput>().setInputInverse(true);
        }

        else
        {
            player.GetComponent<PlayerInput>().setInputInverse(false);
        }
        //Debug.Log(playerInBounds);
        }

    //private bool playerInBounds;

    /*void OnTriggerEnter(Collider other)
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
    }*/
}
