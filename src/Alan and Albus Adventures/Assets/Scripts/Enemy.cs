using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float speed;
    public float damage;
    public bool magicalDamage;
    public bool Attacking;
    public float knockbackForceOnTouch;
    public float experienceValue;
    public Sprite leftSprite;
    public Sprite rightSprite;

    protected Rigidbody2D rb2d;
    protected GameObject target;
    protected Vector2 targetVector;
    protected const float targetSwitchThreshold = 0.6f;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        target = null;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (Attacking)
        {
            GetTarget();
            Move();
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var collisionVitalityController = collision.gameObject.GetComponent<VitalityController>();

            if (!collisionVitalityController.isInvincibilityFrame || collisionVitalityController.isInvincible)
            {
                collisionVitalityController.Damage(damage, magicalDamage, false);

                var forceVector = collision.gameObject.transform.position - transform.position;

                collisionVitalityController.Knockback(forceVector, knockbackForceOnTouch);
            }
        }
    }

    protected void GetTarget()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        if (players[0].GetComponent<VitalityController>().isDead)
        {
            target = players[1];
        }
        else if (players[1].GetComponent<VitalityController>().isDead)
        {
            target = players[0];
        }
        else
        {
            var player1Dist = Vector2.Distance(transform.position, players[0].transform.position);
            var player2Dist = Vector2.Distance(transform.position, players[1].transform.position);

            if (target != null)
            {
                if (target == players[0] && player2Dist < (player1Dist * targetSwitchThreshold))
                {
                    target = players[1];
                }
                else if (target == players[1] && player1Dist < (player2Dist * targetSwitchThreshold))
                {
                    target = players[0];
                }
            }
            else
            {
                target = (player1Dist < player2Dist ? players[0] : players[1]);
            }
        }

        targetVector = target.transform.position - transform.position;

        CheckFlip();
    }

    protected void CheckFlip()
    {
        if (targetVector != null)
        {
            if (targetVector.x < 0 && spriteRenderer.sprite != leftSprite)
            {
                spriteRenderer.sprite = leftSprite;
            }
            else if (0 < targetVector.x && spriteRenderer.sprite != rightSprite)
            {
                spriteRenderer.sprite = rightSprite;
            }
        }
    }

    protected abstract void Move();
}
