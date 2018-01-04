using UnityEngine;
using System.Collections;

public class MovingPlatforms : MonoBehaviour
{
    public Transform[] Waypoints;
    public float speed = 2;

    public int CurrentPoint = 0;

    void Update()
    {
        if (transform.position.y != Waypoints[CurrentPoint].transform.position.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[CurrentPoint].transform.position, speed * Time.deltaTime);
        }

        if (transform.position.y == Waypoints[CurrentPoint].transform.position.y)
        {
            CurrentPoint += 1;
        }
        if (CurrentPoint >= Waypoints.Length)
        {
            CurrentPoint = 0;
        }
    }
}