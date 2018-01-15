using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;//Allows us to use SceneManager


[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour


{

    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    private float moveSpeed = 6f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public Animator anim;
    public SpriteRenderer rend;
    private int food;

    public bool canDoubleJump;
    private bool isDoubleJumping = false;

    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;
    //new *sliding platform*
    private float speed = 6.0f;
    private float jumpSpeed = 8.0f;
    private float friction = 1.0f; // 0 means no friction; private var curVel = Vector3.zero; private var velY: float = 0; private var character: CharacterController;
    //end new
    bool facingRight;
 


    private void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        facingRight = true;

    }
    //This function is called when the behaviour becomes disabled or inactive.


    void Flip()
    { 
        // Switch the way the player is labelled as facing
        facingRight = !facingRight;
        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    private void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        anim.SetFloat("pX", velocity.x);
        anim.SetFloat("pY", velocity.y);

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.5f && !facingRight)
            {
                //If we're moving right but not facing right, flip the sprite and set     facingRight to true.
                Flip();
                facingRight = true;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0.5f && facingRight)
            {
                //If we're moving left but not facing left, flip the sprite and set facingRight to false.
                Flip();
                facingRight = false;
            }

            //If we're not moving horizontally, check for vertical movement. The "else if" stops diagonal movement. Change to "if" to allow diagonal movement.
        }

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }

        

        //new
        // get the CharacterController only the first time: if (!character) character = GetComponent(CharacterController); // get the direction from the controls: var dir = Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // calculate the desired velocity: var vel = transform.TransformDirection(dir) * speed;

        // here's where the magic happens: curVel = Vector3.Lerp(curVel, vel, 5 friction friction * Time.deltaTime);

        // apply gravity and jump after the friction! if (character.isGrounded){ velY = 0; if (Input.GetKeyDown("Jump")){ velY = jumpSpeed; } velY -= gravity Time.deltaTime; } curVel.y = velY; character.Move(curVel Time.deltaTime); }
        /*
                public void OnTriggerEnter(other: Collider){
                    if (other.name == "Ice") {
                        friction = 0.1; // set low friction 
                    }
                    //Reverse Room 
                    if (other.name == "ReverseArea"){
                    ....

                    }
                }

                    function OnTriggerExit(other: Collider){
                        if (other.name == "Ice") {
                        friction = 1; // restore regular friction  

                    }

                }
                //end new
            */
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            isDoubleJumping = false;
        }
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0f)
            {
                velocityXSmoothing = 0f;
                velocity.x = 0f;
                if (directionalInput.x != wallDirX && directionalInput.x != 0f)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;
    }

    public void LoseFood(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        anim.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        food -= loss;

        //Check to see if game has ended.
        CheckIfGameOver();
    }
    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (food <= 0)
        {

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }

}

