using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour 
{
	public int playerID; // ReWired player ID
	public float speed;

	private Rigidbody2D rb2d;
	private Player player;
	private Vector2 moveVector;
	private Vector2 rotationVector;

	// Use this for initialization
	private void Start() 
	{
		player = ReInput.players.GetPlayer(playerID);
		rb2d = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		
	}
	
	// Update is called once per frame
	private void FixedUpdate()
	{
		moveVector.x = player.GetAxis("Move Horizontal");
		moveVector.y = player.GetAxis("Move Vertical");

		rotationVector.x = player.GetAxis("Look Horizontal");
		rotationVector.y = player.GetAxis("Look Vertical");

		if (rotationVector != Vector2.zero)
		{
			float angle = Mathf.Atan2 (-rotationVector.x, rotationVector.y);

			angle *= Mathf.Rad2Deg;

			rb2d.freezeRotation = false;
			rb2d.MoveRotation (angle);
		}
		else
		{
			rb2d.freezeRotation = true;
		}

		rb2d.MovePosition(rb2d.position + moveVector * speed * Time.fixedDeltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		
		// Uncomment for no collision between players
    	// if (collision.gameObject.tag == "Player")
		// {
		//	  Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
		// }
	}
}
