using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{

    [Header("Movement Settings")]
    public float baseMovementSpeed;

    //Get the scenes main camera for use
	void Start ()
    {

	}
	

	void Update ()
    {

	}

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        //Get player input
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Cap magnitude to 1.0 (so diagonal movement isn't faster)
        if (input.sqrMagnitude > 1.0f)
        {
            input = input.normalized;
        }

        //Calculate movement
        Vector3 movement = ((this.transform.forward * input.y) + (this.transform.right * input.x)) * baseMovementSpeed * Time.fixedDeltaTime;

        //Move player using input
        GetComponent<Rigidbody>().MovePosition(this.transform.position + movement);
    }

}
