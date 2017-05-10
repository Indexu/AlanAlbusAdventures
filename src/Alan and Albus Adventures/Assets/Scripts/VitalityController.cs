﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalityController : MonoBehaviour
{
    public float currentHealth;
    public float magicResistance;
    public float physicalResistance;
    public bool isDead = false;
    public bool isInvincible;
    public bool isInvincibilityFrame;
    public float invincibilityFrameTime;
    public Text healthText;
    public Slider healthSlider;
    public GameObject hitParticle;
    public GameObject critParticle;
    public GameObject deathParticle;
    public bool boss;
    public bool player;
    public List<AudioClip> blobDeathSound;
    public bool doUpdateUI;

    private float damageAmount = -1;
    private Stats stats;
    private GameManager gameManager;
    private Rigidbody2D rb2d;
    private float enemyMaxHealth;
    private bool knockback;
    private Vector2 knockbackVector;
    private float knockbackForce;
    private RectTransform healthBar;
    private Enemy enemyComponent;
    private const float healthBarOffset = 133.7f;
    private Bounds bounds;
    private GameObject selectedParticle;
    private AnimationCurveController acc;
    private float damageTextOffset;

    public void Damage(float amount, bool isMagical, bool isCrit)
    {
        if (!isInvincible && !isInvincibilityFrame && !isDead)
        {
            amount /= (isMagical ? magicResistance : physicalResistance);

            currentHealth -= amount;

            doUpdateUI = true;
            damageAmount = amount;

            selectedParticle = (isCrit ? critParticle : hitParticle);

            acc.Hit();

            CheckHealth();

            if (player)
            {
                isInvincibilityFrame = true;
                StartCoroutine(InvincibiltyFrame());
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
    }

    public void Knockback(Vector2 direction, float force)
    {
        knockback = true;
        knockbackForce = force;
        knockbackVector = direction.normalized;
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
        bounds = GetComponent<Renderer>().bounds;
        acc = GetComponent<AnimationCurveController>();
        damageTextOffset = bounds.extents.y * healthBarOffset;

        if (gameObject.tag == "Player")
        {
            player = true;
            doUpdateUI = true;
            stats = GetComponent<Stats>();

            FindHealthBar();
        }
        else
        {
            player = false;
            doUpdateUI = false;
        }

        if (!player)
        {
            enemyMaxHealth = currentHealth;
            enemyComponent = GetComponent<Enemy>();
        }
    }

    private void OnGUI()
    {
        if (doUpdateUI)
        {
            UpdateUI();
        }
        if (!player && !boss)
        {
            PositionHealthbar();
        }
    }
    private void FixedUpdate()
    {
        CheckKnockback();
    }

    private void CheckKnockback()
    {
        if (knockback)
        {
            knockback = false;
            rb2d.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            if (player)
            {
                isDead = true;
                gameManager.PlayerKilled();
            }
            else
            {
                SoundManager.instance.PlayBlobDeath(blobDeathSound);
                gameManager.EnemyKilled();
                healthSlider.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }

            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(selectedParticle, transform.position, Quaternion.identity);
        }
    }

    private void UpdateUI()
    {
        if (boss)
        {
            healthText.text = currentHealth.ToString("0") + "/" + enemyMaxHealth;
            healthSlider.value = currentHealth / enemyMaxHealth;

            if (0 <= damageAmount)
            {
                UIManager.instance.DisplayDamageText(transform.position, damageAmount, damageTextOffset);
            }
        }
        else if (player)
        {
            healthText.text = currentHealth.ToString("0") + "/" + stats.maxHealth;
            healthSlider.value = currentHealth / stats.maxHealth;
        }
        else
        {
            healthSlider.value = currentHealth / enemyMaxHealth;
            if (0 <= damageAmount)
            {
                UIManager.instance.DisplayDamageText(transform.position, damageAmount, damageTextOffset);
            }
        }

        doUpdateUI = false;
    }

    private void PositionHealthbar()
    {
        if (healthBar == null)
        {
            if (healthSlider != null)
            {
                healthBar = healthSlider.GetComponent<RectTransform>();
                healthBar.anchorMin = new Vector2(0.5f, 0.5f);
                healthBar.anchorMax = new Vector2(0.5f, 0.5f);
            }
        }

        if (enemyComponent.Attacking && healthBar != null)
        {
            Vector2 screenPos = UIManager.instance.PositionToUI(transform.position);
            screenPos.y += bounds.extents.y * healthBarOffset;

            healthBar.anchoredPosition = screenPos;
        }
    }

    private void FindHealthBar()
    {
        var playerID = GetComponent<PlayerController>().playerID;
        var searchString = (playerID == 0 ? "AlanHealthSlider" : "AlbusHealthSlider");
        healthSlider = UIManager.instance.canvas.transform.Find(searchString).GetComponent<Slider>();
        healthText = healthSlider.transform.Find("HealthText").GetComponent<Text>();
    }

    private IEnumerator InvincibiltyFrame()
    {
        yield return new WaitForSeconds(invincibilityFrameTime);
        isInvincibilityFrame = false;
    }


}
