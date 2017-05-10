using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public int playerID; // ReWired player ID
    public ProjectileDirection projectileDirection;
    public bool attackButton;

    private Rigidbody2D rb2d;
    private VitalityController vc;
    private DoorController door;
    private ChestAnimationController chest;
    private ReviveController reviveController;
    private GameManager gameManager;
    private Stats stats;
    private Player player;
    private Vector2 moveVector;
    private Vector2 rotationVector;
    private bool doAttack;
    private bool inStatsScreen;
    private float currentReviveTime;
    private const float reviveTime = 2.5f;
    private bool canNavigateStats;
    private float joystickStatsDelay = 0.15f;
    private const float joystickDeadzone = 0.4f;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        vc = GetComponent<VitalityController>();
        stats = GetComponent<Stats>();
        rotationVector = Vector2.up;
        inStatsScreen = false;
        canNavigateStats = true;
        currentReviveTime = 0f;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!vc.isDead)
        {
            CheckInput();
        }
        else
        {
            if (player.GetButtonUp("Confirm"))
            {
                gameManager.DeadReset();
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!vc.isDead && !inStatsScreen)
        {
            Rotation();
            Movement();

            if (doAttack)
            {
                if (playerID == 0)
                {
                    projectileDirection.Slash(stats, rotationVector);
                }
                else
                {
                    projectileDirection.Shoot(stats, rotationVector);
                }

                doAttack = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Door")
        {
            door = collider.gameObject.GetComponent<DoorController>();
        }
        else if (collider.gameObject.layer == LayerMask.NameToLayer("ReviveTriggers"))
        {
            reviveController = collider.gameObject.GetComponent<ReviveController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Door")
        {
            door = null;
        }
        else if (collider.gameObject.layer == LayerMask.NameToLayer("ReviveTriggers"))
        {
            reviveController = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Chest")
        {
            chest = collider.gameObject.GetComponent<ChestAnimationController>();
        }
    }

    private void CheckInput()
    {
        if (gameManager.isPaused)
        {
            if (player.GetButtonUp("Pause"))
            {
                gameManager.Unpause();
            }
            if (player.GetButtonUp("Confirm"))
            {
                gameManager.Reset();
            }
        }
        else if (inStatsScreen)
        {
            var analogStick = player.GetAxis("Move Vertical");

            if (player.GetButtonUp("Stats Screen"))
            {
                inStatsScreen = false;
                stats.HideStats();
            }
            if (player.GetButtonUp("DPad Up") || (0 < analogStick && canNavigateStats))
            {
                stats.Up();
            }
            if (player.GetButtonUp("DPad Down") || (analogStick < 0 && canNavigateStats))
            {
                stats.Down();
            }
            if (player.GetButtonUp("Confirm"))
            {
                stats.UpgradeStat();
            }

            if (analogStick != 0 && canNavigateStats)
            {
                canNavigateStats = false;
                StartCoroutine(DelayJoystick());
            }
        }
        else
        {
            if (player.GetButton("Attack") && attackButton)
            {
                doAttack = true;
            }
            if (player.GetButtonUp("Stats Screen"))
            {
                inStatsScreen = true;
                stats.ShowStats();
            }

            if (player.GetButtonUp("Confirm"))
            {
                if (currentReviveTime != 0f)
                {
                    currentReviveTime = 0f;
                }
                else if (chest != null)
                {
                    chest.OpenChest();
                }
                else if (door != null && !gameManager.changingRooms)
                {
                    door.GoThrough();
                }
            }
            else if (player.GetButtonDown("Confirm") || currentReviveTime != 0f)
            {
                if (reviveController != null && reviveController.vc.isDead)
                {
                    currentReviveTime += Time.deltaTime;
                    if (reviveTime < currentReviveTime)
                    {
                        reviveController.Revive();
                        currentReviveTime = 0f;
                    }
                }
            }
            if (player.GetButtonUp("Pause"))
            {
                gameManager.Pause();
            }
        }
    }

    private void Rotation()
    {
        float x = player.GetAxis("Look Horizontal");
        float y = player.GetAxis("Look Vertical");

        if (joystickDeadzone < Mathf.Abs(x) || joystickDeadzone < Mathf.Abs(y))
        {
            rotationVector.x = x;
            rotationVector.y = y;
            projectileDirection.SetCrosshair(rotationVector);

            if (!attackButton)
            {
                doAttack = true;
            }
        }
    }

    private void Movement()
    {
        moveVector.x = player.GetAxis("Move Horizontal");
        moveVector.y = player.GetAxis("Move Vertical");

        rb2d.AddForce(moveVector * stats.movementSpeed, ForceMode2D.Impulse);
    }

    public IEnumerator DelayJoystick()
    {
        yield return new WaitForSeconds(joystickStatsDelay);
        canNavigateStats = true;
    }
}
