﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Controller2D : RaycastController
{
    public float fallingThroughPlatformResetTimer = 0.1f;
    private float maxClimbAngle = 80f;
    private float maxDescendAngle = 80f;

    private int score = 0;
    private double lives = 3.0;

    public GUIText scoreText;
    public GUIText livesText;

    public PlayerInput other;


    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    GameObject player;
    Vector3 originalPos;

    Player playerScript;

    bool immune = false;

    public override void Start()
    {
        base.Start();

        collisions.faceDir = 1;
        score = 0;
        UpdateScore();

        lives = 3.0;
        UpdateLives();


        player = GameObject.FindGameObjectWithTag("PlayerP");
        Vector3 originalPos = player.transform.position;

        playerScript = GetComponent<Player>();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }
    void UpdateLives()
    {
        livesText.text = "Lives: " + lives;
    }


    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        Move(moveAmount, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    IEnumerator Timing()
    {
        yield return new WaitForSeconds(3);
        playerScript.setSpeed(1.0f);
        immune = false;
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
       

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if(hit.distance<1 && hit.collider.gameObject.CompareTag("PickUp"))
                {
                    hit.collider.gameObject.SetActive(false);
                    score += 100;
                    UpdateScore();
                }

                if (hit.distance < 1 && immune == false && hit.collider.gameObject.CompareTag("Deadly") || hit.collider.gameObject.CompareTag("Spikes"))
                {
                    Vector3 test = new Vector3(-13, -4, 0);
                    player.transform.position = test;
                    if (lives > 0)
                        lives--;
                    else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    Debug.Log(lives);
                    UpdateLives();
                    score -= 100;
                    UpdateScore();
                }
                if (hit.distance < 1 && hit.collider.gameObject.CompareTag("BadCandy"))
                {
                    hit.collider.gameObject.SetActive(false);
                    playerScript.setSpeed(0.5f);
                    StartCoroutine(Timing());
                    score -= 30;
                    lives += 0.5;
                    UpdateScore();
                    UpdateLives();
                }

                if (hit.distance < 1 && hit.collider.gameObject.CompareTag("RedBull"))
                {
                    hit.collider.gameObject.SetActive(false);
                    playerScript.setSpeed(1.5f);
                    immune = true;
                    StartCoroutine(Timing());
                    score += 30;
                    UpdateScore();
                    UpdateLives();
                }

                /*
                if (hit.distance < 1 && hit.collider.gameObject.CompareTag("TriggerArea"))
                {
                    gameObject.CompareTag("Spikes");
                    transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
                }*/

                /*if (hit.distance < 1 && hit.collider.gameObject.CompareTag("ReverseArea"))
                {
                    other.setInputInverse(true);
                }*/


                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }
                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle);
                    moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }

    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }

                }
            }
        }
    }

    

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", fallingThroughPlatformResetTimer);
                        continue;
                    }
                }
                else if (hit.distance < 1 && hit.collider.gameObject.CompareTag("PickUp"))
                {
                    hit.collider.gameObject.SetActive(false);
                    score += 100;
                    UpdateScore();
                }

                if (hit.distance < 1 && immune == false && hit.collider.gameObject.CompareTag("Deadly") || hit.collider.gameObject.CompareTag("Spikes"))
                {
                    Vector3 test = new Vector3(-13, -4, 0);
                    player.transform.position = test;
                    if (lives > 0)
                        lives--;
                    else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    Debug.Log(lives);
                    UpdateLives();
                    score -= 100;
                    UpdateScore();
                }
                if (hit.distance < 1 && hit.collider.gameObject.CompareTag("BadCandy"))
                {
                    hit.collider.gameObject.SetActive(false);
                    score -= 30;
                    lives += 0.5;
                    playerScript.setSpeed(0.5f);
                    StartCoroutine(Timing());
                    UpdateScore();
                    UpdateLives();
                }

                if (hit.distance < 1 && hit.collider.gameObject.CompareTag("RedBull"))
                {
                    hit.collider.gameObject.SetActive(false);
                    playerScript.setSpeed(1.5f);
                    immune = true;
                    StartCoroutine(Timing());
                    score += 30;
                    UpdateScore();
                    UpdateLives();
                }

                /*if (hit.distance < 1 && hit.collider.gameObject.CompareTag("ReverseArea"))
                {
                    other.setInputInverse(true);
                }*/


                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance * skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;
        }
    }
}
