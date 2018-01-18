using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    public bool inverted = false;
    public AudioClip jump;


    private void Start()
    {
        player = GetComponent<Player>();
    
    }

    private void Update()
    {
        Vector2 directionalInput = new Vector2();
        if(inverted == false)
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        else if(inverted == true) directionalInput =  new Vector2(-Input.GetAxisRaw("Horizontal"), -Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
        //Debug.Log(inverted);

        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
            AudioSource.PlayClipAtPoint(jump, player.transform.position, 1.0f);
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
    }

    public void setInputInverse(bool inverse)
    {
        inverted = inverse;
    }
}

