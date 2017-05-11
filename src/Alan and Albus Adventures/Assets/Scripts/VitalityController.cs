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
    public GameObject hitParticle;
    public GameObject critParticle;
    public GameObject deathParticle;
    public GameObject reviveParticle;
    public bool boss;
    public bool player;
    public List<AudioClip> blobDeathSound;
    public List<AudioClip> blobDamageSound;
    public List<AudioClip> playerDamageSound;
    public List<AudioClip> playerDeathSound;
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
    private SpriteRenderer spriteRenderer;
    private bool lowHealth;
    private const float lowHealthThreshold = 0.2f;
    private const float minColor = 190f / 255f;
    private const float maxColor = 255f / 255f;
    private const float colorSpeed = 200f / 255f;
    private bool colorUp;

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
        if (currentHealth > stats.maxHealth)
        {
            currentHealth = stats.maxHealth;
        }
        doUpdateUI = true;
    }

    public void Revive()
    {
        isDead = false;
        lowHealth = false;
        currentHealth += stats.maxHealth / 2;
        spriteRenderer.color = Color.white;
        doUpdateUI = true;
        isInvincibilityFrame = true;
        StartCoroutine(InvincibiltyFrame());

        Instantiate(reviveParticle, transform.position, Quaternion.identity);
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
        spriteRenderer = GetComponent<SpriteRenderer>();

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

    private void Update()
    {
        if (player && lowHealth && !isDead)
        {
            var color = spriteRenderer.color;
            var increase = colorSpeed * Time.deltaTime;

            if (colorUp)
            {
                if (color.g + increase < maxColor)
                {
                    color.g += increase;
                    color.b += increase;
                }
                else
                {
                    color.g = maxColor;
                    color.b = maxColor;
                    colorUp = false;
                }
            }
            else
            {
                if (minColor < color.g + increase)
                {
                    color.g -= increase;
                    color.b -= increase;
                }
                else
                {
                    color.g = minColor;
                    color.b = minColor;
                    colorUp = true;
                }
            }

            spriteRenderer.color = color;
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
                currentHealth = 0;
                spriteRenderer.color = Color.gray;
                SoundManager.instance.PlaySounds(playerDeathSound);
                GameManager.instance.PlayerKilled();         
            }
            else
            {
                var xp = GetComponent<Enemy>().experienceValue;
                GameManager.instance.EnemyKilled(xp);
                SoundManager.instance.PlaySounds(blobDeathSound);
                healthSlider.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }

            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(selectedParticle, transform.position, Quaternion.identity);
            if(tag == "enemy")
            {
                SoundManager.instance.PlaySounds(blobDamageSound);
            }
            else
            {
                SoundManager.instance.PlaySounds(playerDamageSound);
            }
        }

        lowHealth = player && ((float)currentHealth / (float)stats.maxHealth < lowHealthThreshold);
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
