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
    private float enemyMaxHealth;
    private bool knockback;
    private Vector2 knockbackVector;
    private float knockbackForce;
    private RectTransform healthBar;
    private Enemy enemyComponent;
    private const float healthBarOffset = 133.7f;

    public void Damage(float amount, bool isMagical)
    {
        if (!isInvincible && !isInvincibilityFrame && !isDead)
        {
            amount /= (isMagical ? magicResistance : physicalResistance);

            currentHealth -= amount;

            doUpdateUI = true;

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
                gameManager.EnemyKilled();
                GameObject.Destroy(healthSlider.gameObject);
                GameObject.Destroy(gameObject);
            }
        }
    }

    private void UpdateUI()
    {
        if (boss)
        {
            healthText.text = currentHealth.ToString("0") + "/" + enemyMaxHealth;
            healthSlider.value = currentHealth / enemyMaxHealth;
        }
        else if (player)
        {
            healthText.text = currentHealth.ToString("0") + "/" + stats.maxHealth;
            healthSlider.value = currentHealth / stats.maxHealth;
        }
        else
        {
            healthSlider.value = currentHealth / enemyMaxHealth;
        }
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
            Vector2 viewportPos = GameManager.instance.mainCamera.WorldToViewportPoint(transform.position);
            Vector2 screenPos = new Vector2(
            viewportPos.x * GameManager.instance.canvasRect.sizeDelta.x,
            viewportPos.y * GameManager.instance.canvasRect.sizeDelta.y);

            screenPos -= GameManager.instance.canvasRect.sizeDelta * 0.5f;
            screenPos.y += GetComponent<Renderer>().bounds.extents.y * healthBarOffset;

            healthBar.anchoredPosition = screenPos;
        }
    }

    private IEnumerator InvincibiltyFrame()
    {
        yield return new WaitForSeconds(invincibilityFrameTime);
        isInvincibilityFrame = false;
    }
}
