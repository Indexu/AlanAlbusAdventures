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
    public GameObject damageText;
    public bool boss;
    public bool player;
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
    private const float damageTextDuration = 0.6f;
    private const float damageTextSpeed = 3f;

    public void Damage(float amount, bool isMagical)
    {
        if (!isInvincible && !isInvincibilityFrame && !isDead)
        {
            amount /= (isMagical ? magicResistance : physicalResistance);

            currentHealth -= amount;

            doUpdateUI = true;
            damageAmount = amount;

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
                healthSlider.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
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
                DisplayDamageText();
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
                DisplayDamageText();
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
            Vector2 screenPos = GameManager.instance.PositionToUI(transform.position);
            screenPos.y += bounds.extents.y * healthBarOffset;

            healthBar.anchoredPosition = screenPos;
        }
    }

    private void DisplayDamageText()
    {
        var damageTextInstance = Instantiate(damageText, Vector3.zero, Quaternion.identity, GameManager.instance.canvas.transform);

        Vector2 screenPos = GameManager.instance.PositionToUI(transform.position);
        screenPos.y += bounds.extents.y * healthBarOffset;

        var rt = damageTextInstance.GetComponent<RectTransform>();
        rt.GetComponent<RectTransform>().anchoredPosition = screenPos;
        damageTextInstance.GetComponent<Text>().text = damageAmount.ToString();

        StartCoroutine(AnimateDamageText(rt));
    }

    private IEnumerator InvincibiltyFrame()
    {
        yield return new WaitForSeconds(invincibilityFrameTime);
        isInvincibilityFrame = false;
    }

    private IEnumerator AnimateDamageText(RectTransform rt)
    {
        var seconds = 0f;
        var vector = new Vector2(0f, damageTextSpeed);
        var text = rt.gameObject.GetComponent<Text>();
        var color = text.color;

        while (seconds < damageTextDuration)
        {
            seconds += Time.deltaTime;
            rt.anchoredPosition += vector;
            color.a -= Time.deltaTime / damageTextDuration;
            text.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        GameObject.Destroy(rt.gameObject);
    }
}
