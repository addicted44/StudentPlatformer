using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 public class SpikeFall : MonoBehaviour
{

    public float fallSpeed;
    //private Rigidbody2D RB2D;
    //public float firstPosition;
    public float pos_x, pos_y, pos_z;
    public float end_pos;


    void Start()
    {
        //RB2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Vector3 Direction = Vector3.down;
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (gameObject.transform.position.y <= end_pos)
        {
            
            gameObject.transform.position = new Vector3(pos_x, pos_y, pos_z);
          
        }
    }
    
}
