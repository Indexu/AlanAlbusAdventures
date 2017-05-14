using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileDirection : MonoBehaviour
{
    public float radius;
    public float attackForce;
    public GameObject projectile;
    public List<AudioClip> projectileSound;
    public List<AudioClip> projectileHitSound;
    public List<AudioClip> critProjectileHitSound;
    public Sprite[] AlanAttackSprites;
    public Sprite[] AlbusAttackSprites;


    private float nextFire;
    private int playerID;
    private bool magicalDamage;
    private Vector2 playerPos;
    private List<Collider2D> collidingEnemies;
    private Stats stats;
    private Inventory inventory;
    private SpriteRenderer spriteRenderer;

    public void SetCrosshair(Vector2 coords)
    {
        float angle = Mathf.Atan2(-coords.x, coords.y);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        playerPos = transform.parent.position;

        var offset = coords.normalized * radius;
        transform.position = playerPos + offset;
    }

    public void ClearCollidingEnemies()
    {
        collidingEnemies.Clear();
    }

    public void Shoot(Vector2 rotationVector)
    {
        if (nextFire < Time.time)
        {
            nextFire = Time.time + (stats.attackSpeed / inventory.GetStatBonus(Property.ATTACKSPEED));

            StartCoroutine(AlbusAttackAnimation());

            var projectileInstance = Instantiate(projectile, transform.position, transform.rotation) as GameObject;

            projectileInstance.GetComponent<Rigidbody2D>().AddForce(rotationVector.normalized * attackForce, ForceMode2D.Impulse);

            var projectileComponent = projectileInstance.GetComponent<Projectile>();
            projectileComponent.damage = stats.baseDamage * inventory.GetStatBonus(Property.BASEDAMAGE);
            projectileComponent.isMagical = magicalDamage;
            projectileComponent.playerFired = true;
            var isCrit = false;

            if (Random.value * 100 < (stats.critHitChance * inventory.GetStatBonus(Property.CRITCHANCE)))
            {
                projectileComponent.damage *= (stats.critHitDamage * inventory.GetStatBonus(Property.CRITDAMAGE));
                isCrit = true;
            }

            if (!collidingEnemies.Any())
            {
                int index = Random.Range(0, projectileSound.Count);
                SoundManager.instance.PlaySounds(projectileSound.ElementAt(index));
            }

            projectileComponent.isCrit = isCrit;

            projectileComponent.Init();
        }
    }

    public void Slash(Vector2 rotationVector)
    {
        if (nextFire < Time.time)
        {
            nextFire = Time.time + (stats.attackSpeed / inventory.GetStatBonus(Property.ATTACKSPEED));

            StartCoroutine(AlanAttackAnimation());

            var damage = stats.baseDamage * inventory.GetStatBonus(Property.BASEDAMAGE);
            var isCrit = false;

            if (Random.value * 100 < (stats.critHitChance * inventory.GetStatBonus(Property.CRITCHANCE)))
            {
                damage *= (stats.critHitDamage * inventory.GetStatBonus(Property.CRITDAMAGE));
                isCrit = true;
            }

            if (!collidingEnemies.Any())
            {
                int index = Random.Range(0, projectileSound.Count);
                SoundManager.instance.PlaySounds(projectileSound.ElementAt(index));
            }
            else
            {
                int index = Random.Range(0, critProjectileHitSound.Count);
                SoundManager.instance.PlaySounds(critProjectileHitSound.ElementAt(index));
            }

            for (int i = collidingEnemies.Count - 1; (i >= 0 && collidingEnemies.Any()); i--)
            {
                var enemy = collidingEnemies[i].gameObject;

                if (enemy != null && enemy.activeSelf)
                {
                    var vc = enemy.GetComponent<VitalityController>();

                    Vector2 knockbackVector = (enemy.transform.position - gameObject.transform.position).normalized;

                    vc.Knockback(knockbackVector, attackForce);
                    vc.Damage(damage, magicalDamage, isCrit);
                }
            }
        }
    }

    private void Start()
    {
        playerID = gameObject.transform.parent.gameObject.GetComponent<PlayerController>().playerID;
        stats = gameObject.transform.parent.gameObject.GetComponent<Stats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventory = gameObject.transform.parent.gameObject.GetComponent<Inventory>();
        magicalDamage = (playerID == 1);
        SetCrosshair(Vector2.up);
        collidingEnemies = new List<Collider2D>();
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

    private IEnumerator AlanAttackAnimation()
    {
        var interval = (stats.attackSpeed / inventory.GetStatBonus(Property.ATTACKSPEED)) / AlanAttackSprites.Length;
        foreach (var sprite in AlanAttackSprites)
        {
            yield return new WaitForSeconds(interval);
            spriteRenderer.sprite = sprite;
        }
    }

    private IEnumerator AlbusAttackAnimation()
    {
        var interval = (stats.attackSpeed / inventory.GetStatBonus(Property.ATTACKSPEED)) / AlbusAttackSprites.Length;
        foreach (var sprite in AlbusAttackSprites)
        {
            yield return new WaitForSeconds(interval);
            spriteRenderer.sprite = sprite;
        }
    }
}
