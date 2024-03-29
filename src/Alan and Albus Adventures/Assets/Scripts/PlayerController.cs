﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    public int playerID; // ReWired player ID
    public ProjectileDirection projectileDirection;
    public bool attackButton;
    public bool playstationController;
    public DoorController door;
    public float destroyTime;
    public bool inStatsScreen;

    private Rigidbody2D rb2d;
    private VitalityController vc;
    private ChestAnimationController chest;
    private ReviveController reviveController;
    private GameManager gameManager;
    private Stats stats;
    private Player player;
    private Inventory inventory;
    private GameObject healthPotion;
    private Vector2 moveVector;
    private Vector2 rotationVector;
    private bool doAttack;
    private float currentReviveTime;
    private const float reviveTime = 2.5f;
    private bool canNavigateStats;
    private float joystickStatsDelay = 0.15f;
    private const float joystickDeadzone = 0.4f;
    private bool showingPassives;
    private int viewItem;
    private float currentDestroyTime;
    private bool destroyingItem;
    private GameObject hole;
    private GameObject alex;
    private List<GameObject> itemList;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerID);

        if (playerID == 0)
        {
            playstationController = ControllerSubscriber.AlanPS4;
        }
        else
        {
            playstationController = ControllerSubscriber.AlbusPS4;
        }
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
        viewItem = -1;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        inventory = GetComponent<Inventory>();
        hole = null;
        alex = null;

        UIManager.instance.ApplySpritesToButtons(playerID, playstationController);

        if (playerID == 0)
        {
            itemList = GameManager.instance.AlanItemList;
        }
        else
        {
            itemList = GameManager.instance.AlbusItemList;
        }
    }

    private void Update()
    {
        if (!vc.isDead && !GameManager.instance.changingRooms)
        {
            CheckInput();
        }
    }

    private void FixedUpdate()
    {
        if (!vc.isDead && !inStatsScreen && !GameManager.instance.changingRooms)
        {
            Rotation();
            Movement();

            if (doAttack)
            {
                if (playerID == 0)
                {
                    projectileDirection.Slash(rotationVector);
                }
                else
                {
                    projectileDirection.Shoot(rotationVector);
                }

                doAttack = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("ReviveTriggers"))
        {
            reviveController = collider.gameObject.GetComponent<ReviveController>();

            if (reviveController.vc.isDead)
            {
                UIManager.instance.ShowReviveUI(collider.transform.parent, playstationController);
            }
        }
        else if (collider.tag == "Item")
        {
            itemList.Add(collider.gameObject);
        }
        else if (collider.tag == "HealthPotion")
        {
            healthPotion = collider.gameObject;
        }
        else if (collider.tag == "Chest")
        {
            var c = collider.GetComponent<ChestAnimationController>();

            if (!c.opened)
            {
                chest = c;
            }
        }
        else if (collider.tag == "Door")
        {
            door = collider.gameObject.GetComponent<DoorController>();
            door.EnterRange();
            UIManager.instance.ShowDoorButton(collider.transform.position, door.direction, playstationController);
        }
        else if (collider.tag == "Hole")
        {
            hole = collider.gameObject;
        }
        else if (collider.tag == "Alex")
        {
            alex = collider.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("ReviveTriggers"))
        {
            if (reviveController.vc.isDead)
            {
                UIManager.instance.HideReviveUI();
            }

            reviveController = null;
        }
        else if (collider.tag == "Item")
        {
            itemList.Remove(collider.gameObject);
        }
        else if (collider.tag == "HealthPotion")
        {
            healthPotion = null;
        }
        else if (collider.tag == "Chest")
        {
            chest = null;
        }
        else if (collider.tag == "Door")
        {
            if (door != null)
            {
                door.ExitRange();
                UIManager.instance.HideDoorButton(door.direction);
                door = null;
            }
        }
        else if (collider.tag == "Hole")
        {
            hole = null;
        }
        else if (collider.tag == "Alex")
        {
            alex = null;
        }
    }

    private void CheckInput()
    {
        if (gameManager.isPaused)
        {
            if (player.GetButtonUp("Start"))
            {
                gameManager.Unpause();
            }
        }
        else if (inStatsScreen)
        {
            var analogStick = player.GetAxis("Move Vertical");

            if (player.GetButtonUp("Triangle") || player.GetButtonUp("Circle"))
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
            if (player.GetButtonUp("Cross"))
            {
                stats.UpgradeStat();
            }

            if (player.GetButtonDown("Square"))
            {
                stats.UnmodifiedStats(true);
            }
            if (player.GetButtonUp("Square"))
            {
                stats.UnmodifiedStats(false);
            }

            if (analogStick != 0 && canNavigateStats)
            {
                canNavigateStats = false;
                StartCoroutine(DelayJoystick());
            }
        }
        else if (showingPassives)
        {
            if (player.GetButtonUp("L2"))
            {
                showingPassives = false;
                UIManager.instance.SetPassiveItemSlots(playerID, false);
                UIManager.instance.HideInventoryTooltip(playerID);
            }
            if ((playerID == 0 && GameManager.instance.AlanItemList.Count != 0) || (playerID == 1 && GameManager.instance.AlbusItemList.Count != 0))
            {
                AttemptToPickUpItem();
            }
            else if (player.GetButtonUp("Square"))
            {
                viewItem = 0;
                ViewItem();
            }
            else if (player.GetButtonUp("Cross"))
            {
                viewItem = 1;
                ViewItem();
            }
            else if (player.GetButtonUp("Circle"))
            {
                viewItem = 2;
                ViewItem();
            }
            else if (player.GetButtonUp("Triangle"))
            {
                viewItem = 3;
                ViewItem();
            }
            else if ((player.GetButtonDown("Square") || destroyingItem) && viewItem == 0)
            {
                DestroyItem();
            }
            else if ((player.GetButtonDown("Cross") || destroyingItem) && viewItem == 1)
            {
                DestroyItem();
            }
            else if ((player.GetButtonDown("Circle") || destroyingItem) && viewItem == 2)
            {
                DestroyItem();
            }
            else if ((player.GetButtonDown("Triangle") || destroyingItem) && viewItem == 3)
            {
                DestroyItem();
            }
        }
        else
        {
            if (player.GetButtonDown("L2"))
            {
                showingPassives = true;
                UIManager.instance.SetPassiveItemSlots(playerID, true);
            }
            if (player.GetButton("R2") && attackButton)
            {
                doAttack = true;
            }
            if (player.GetButtonUp("Triangle") && !GameManager.instance.inCombat)
            {
                inStatsScreen = true;
                stats.ShowStats();
            }

            if (player.GetButtonUp("Cross"))
            {
                if (currentReviveTime != 0f)
                {
                    currentReviveTime = 0f;
                    UIManager.instance.UpdateReviveSlider(0f);
                }
                else if (chest != null)
                {
                    chest.OpenChest();
                }
                else if (healthPotion != null)
                {
                    PickUpHealthPotion();
                }
                else if (door != null && !gameManager.changingRooms)
                {
                    door.GoThrough();
                }
                else if (hole != null && !gameManager.changingRooms)
                {
                    GameManager.instance.NextFloor();
                }
                else if (alex != null && !gameManager.changingRooms)
                {
                    GameManager.instance.StoryModeWin();
                }
            }
            else if (player.GetButtonDown("Cross") || currentReviveTime != 0f)
            {
                if (reviveController != null && reviveController.vc.isDead)
                {
                    currentReviveTime += Time.deltaTime;
                    UIManager.instance.UpdateReviveSlider(currentReviveTime / reviveTime);

                    if (reviveTime < currentReviveTime)
                    {
                        UIManager.instance.HideReviveUI();
                        reviveController.Revive();
                        currentReviveTime = 0f;
                    }
                }
            }
            if (player.GetButtonUp("Start"))
            {
                gameManager.Pause();
            }
            if (player.GetButtonDown("R1"))
            {
                inventory.UseHealthPotion();
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

        rb2d.AddForce(moveVector * stats.movementSpeed * inventory.GetStatBonus(Property.MOVEMENTSPEED), ForceMode2D.Impulse);
    }

    private void AttemptToPickUpItem()
    {
        var item = itemList[itemList.Count - 1];

        if (item == null)
        {
            itemList.RemoveAt(itemList.Count - 1);
            return;
        }

        var itemToAdd = item.GetComponent<Item>();

        if (player.GetButtonUp("Square") && inventory.AddItem(itemToAdd, 0))
        {
            UIManager.instance.SetPassiveItemLeft(playerID, (Texture)itemToAdd.GetComponent<SpriteRenderer>().sprite.texture);
            GameManager.instance.RemoveFromItemLists(itemToAdd.gameObject);
            itemToAdd.PickedUp();
        }
        else if (player.GetButtonUp("Cross") && inventory.AddItem(itemToAdd, 1))
        {
            UIManager.instance.SetPassiveItemDown(playerID, (Texture)itemToAdd.GetComponent<SpriteRenderer>().sprite.texture);
            GameManager.instance.RemoveFromItemLists(itemToAdd.gameObject);
            itemToAdd.PickedUp();
        }
        else if (player.GetButtonUp("Circle") && inventory.AddItem(itemToAdd, 2))
        {
            UIManager.instance.SetPassiveItemRight(playerID, (Texture)itemToAdd.GetComponent<SpriteRenderer>().sprite.texture);
            GameManager.instance.RemoveFromItemLists(itemToAdd.gameObject);
            itemToAdd.PickedUp();
        }
        else if (player.GetButtonUp("Triangle") && inventory.AddItem(itemToAdd, 3))
        {
            UIManager.instance.SetPassiveItemUp(playerID, (Texture)itemToAdd.GetComponent<SpriteRenderer>().sprite.texture);
            GameManager.instance.RemoveFromItemLists(itemToAdd.gameObject);
            itemToAdd.PickedUp();
        }
    }

    private void PickUpHealthPotion()
    {
        var hp = healthPotion.GetComponent<HealthPotion>();
        inventory.AddHealthPotion(hp.charges);
        hp.PickedUp();
        healthPotion = null;
    }

    private void ViewItem()
    {
        inventory.ViewItem(viewItem);
        destroyingItem = false;
        currentDestroyTime = 0f;
    }

    private void DestroyItem()
    {
        destroyingItem = true;
        currentDestroyTime += Time.deltaTime;
        inventory.DestroyItem(viewItem, currentDestroyTime / destroyTime);

        if (destroyTime <= currentDestroyTime)
        {
            destroyingItem = false;
            currentDestroyTime = 0f;
            viewItem = -1;
        }
    }

    public IEnumerator DelayJoystick()
    {
        yield return new WaitForSeconds(joystickStatsDelay);
        canNavigateStats = true;
    }
}
