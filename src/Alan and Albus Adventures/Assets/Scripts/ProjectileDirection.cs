﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDirection : MonoBehaviour
{
    public float radius;
    public float attackForce;
    public GameObject projectile;
    public AudioClip projectileSound;

    private float nextFire;
    private int playerID;
    private bool magicalDamage;
    private Vector2 playerPos;
    private List<Collider2D> collidingEnemies;

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

            projectileInstance.GetComponent<Rigidbody2D>().AddForce(rotationVector.normalized * attackForce, ForceMode2D.Impulse);

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

    public void Slash(Stats stats, Vector2 rotationVector)
    {
        if (nextFire < Time.time)
        {
            SoundManager.instance.PlaySingle(projectileSound);

            nextFire = Time.time + stats.attackSpeed;

            var damage = stats.baseDamage;

            if (Random.value * 100 < stats.critHitChance)
            {
                damage *= stats.critHitDamage;
            }

            for (int i = collidingEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = collidingEnemies[i].gameObject;
                var vc = enemy.GetComponent<VitalityController>();

                Vector2 knockbackVector = (enemy.transform.position - gameObject.transform.position).normalized;

                vc.Knockback(knockbackVector, attackForce);
                vc.Damage(damage, magicalDamage);

                if (enemy == null)
                {
                    collidingEnemies.RemoveAt(i);
                }
            }
        }
    }

    private void Start()
    {
        playerID = gameObject.transform.parent.gameObject.GetComponent<PlayerController>().playerID;
        magicalDamage = (playerID == 1);
        SetCrosshair(Vector2.up);
        collidingEnemies = (playerID == 0) ? new List<Collider2D>() : null;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            collidingEnemies.Add(collider);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            collidingEnemies.Remove(collider);
        }
    }
}
