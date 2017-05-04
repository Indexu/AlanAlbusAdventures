using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ProjectileDirection : MonoBehaviour {
	
	public int playerID;
    public float radius;
	private Vector2 playerPos;
	private Player player;
	public bool magicalDamage;
	public float projectileForce;
	public GameObject projectile;
	public AudioClip projectileSound;
	private float nextFire;

	private void Start()
	{
		player = ReInput.players.GetPlayer(playerID);
		playerPos = transform.parent.position;
		transform.position = playerPos;
	}
	
	public void SetCrosshair(Vector2 coords)
	{
		float angle = Mathf.Atan2(-coords.x, coords.y);
		angle *= Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		playerPos = transform.parent.position;
		var offset = coords.normalized * radius;
		transform.position = playerPos + offset;
	}
	public void Shoot(Stats stats, Vector2 rotationVector)
	{	
		if (nextFire < Time.time)
		{
			SoundManager.instance.PlaySingle(projectileSound);
			
			nextFire = Time.time + stats.attackSpeed;

			var projectileInstance = Instantiate(projectile, transform.position, transform.rotation) as GameObject;

			projectileInstance.GetComponent<Rigidbody2D>().AddForce(rotationVector.normalized * projectileForce, ForceMode2D.Impulse);

			var projectileComponent = projectileInstance.GetComponent<Projectile>();
			projectileComponent.damage = stats.baseDamage;
			projectileComponent.isMagical = magicalDamage;
			projectileComponent.playerFired = true;

			if (Random.value * 100 < stats.critHitChance)
			{
				projectileComponent.damage *= stats.critHitDamage;
			}

			projectileComponent.Init();
		}
	}
}
