using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
public class SpikeFall : MonoBehaviour
{

    public float fallSpeed = 8.0f;
    private bool playerInBounds;


    // Update is called once per frame
    void Update()
    {
        while(playerInBounds = true)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        }
        
    }
}
*/

 public class SpikeFall : MonoBehaviour
{
    private GameObject[] locationsToSpawn;
    public float fallSpeed = 10.0f;
    private float counter = 0;
    [SerializeField] string[] listOfPossibleTags;
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] float timeBetweenSpawns = 25.0f;

    void Start()
    {
        locationsToSpawn = GameObject.FindGameObjectsWithTag("Spikes");
    }
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > timeBetweenSpawns)
        {            
            GameObject spawnedObject;
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
            spawnedObject = Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)]); //, locationsToSpawn[Random.Range(0, locationsToSpawn.Length)].transform.position, Quaternion.identity) as GameObject;
            spawnedObject.gameObject.tag = listOfPossibleTags[Random.Range(0, listOfPossibleTags.Length)];
            counter = 0;
        }
    }
}