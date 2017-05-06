using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public float speed;
	public float damage;
	public bool isMagical;
	public bool playerFired;

	public void Init()
	{
		var collider = GetComponent<Collider2D>();

		if (playerFired)
		{
			var players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject player in players)
			{
				Physics2D.IgnoreCollision (player.GetComponent<Collider2D> (), collider);
			}
		}
		else
		{
			var enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in enemies)
			{
				Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), collider);
			}
		}
	}

    private void Start()
    {
        GetComponent<TrailRenderer>().sortingLayerName = "Projectiles";
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		var tag = collider.gameObject.tag;

		if (tag == "Player" || tag == "Enemy")
		{
			var vitalityController = collider.gameObject.GetComponent<VitalityController> ();
			vitalityController.Damage (damage, isMagical);
		}

		GameObject.Destroy(gameObject);
	}
}
