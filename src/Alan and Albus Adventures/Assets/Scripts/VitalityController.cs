using System.Collections;
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
    public bool boss;
    public bool player;
    public bool doUpdateUI;

    private float nextDamage;
    private Stats stats;
    private GameManager gameManager;
    private Rigidbody2D rb2d;
    private float bossMaxHealth;
    private bool knockback;
    private Vector2 knockbackVector;
    private float knockbackForce;

    public void Damage(float amount, bool isMagical)
    {
        if (!isInvincible && !isInvincibilityFrame && !isDead)
        {
            amount /= (isMagical ? magicResistance : physicalResistance);

            currentHealth -= amount;

            if (player || boss)
            {
                doUpdateUI = true;
            }

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

        if (gameObject.tag == "Player")
        {
            player = true;
            doUpdateUI = true;
            stats = GetComponent<Stats>();
        }
        else
        {
            player = false;
            doUpdateUI = false;
        }

        if (boss)
        {
            bossMaxHealth = currentHealth;
        }
    }

    private void OnGUI()
    {
        if (doUpdateUI)
        {
            UpdateUI();
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
                gameManager.EnemyKilled();
                GameObject.Destroy(gameObject);
            }
        }
    }

    private void UpdateUI()
    {
        if (boss)
        {
            healthText.text = currentHealth.ToString("0") + "/" + bossMaxHealth;
            healthSlider.value = currentHealth / bossMaxHealth;
        }
        else
        {
            healthText.text = currentHealth.ToString("0") + "/" + stats.maxHealth;
            healthSlider.value = currentHealth / stats.maxHealth;
        }
    }

    private IEnumerator InvincibiltyFrame()
    {
        yield return new WaitForSeconds(invincibilityFrameTime);
        isInvincibilityFrame = false;
    }
}
