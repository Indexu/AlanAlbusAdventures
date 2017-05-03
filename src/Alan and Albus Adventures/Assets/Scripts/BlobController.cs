using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobController : Enemy 
{
	protected override void Move()
	{
		targetVector.x = target.transform.position.x - transform.position.x;
		targetVector.y = target.transform.position.y - transform.position.y;

		rb2d.AddForce(targetVector.normalized * speed);
	}
}
