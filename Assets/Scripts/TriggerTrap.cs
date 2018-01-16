using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrap : MonoBehaviour {

    private bool playerInBounds;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lastMoveDirection = Vector3.zero;
    private float moveSpeed = 2f;
    private float slideSpeed = 1.75f;
    private bool icy = false;

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
    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //Vector3 moveDirection = v * camera.forward + h * camera.right;

        float inputMagnitude = Mathf.Min(new Vector3(h, 0, v).sqrMagnitude, 1f);

        // store last direction when received some movement
        if (inputMagnitude > 0.225f)
        {
            lastMoveDirection = moveDirection;
        }
        // add speed
        // keeps sliding when still, runs slowly when moving
        if (icy)
        {
            moveDirection = lastMoveDirection * slideSpeed;
        }
        else
        {
            moveDirection *= moveSpeed;
        }

       // controller.Move(moveDirection * Time.deltaTime)
 }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        icy = hit.collider.CompareTag("Ice");
    }

    
}
