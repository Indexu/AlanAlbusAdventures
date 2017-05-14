using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimationController : MonoBehaviour
{
    public Sprite openedChest;
    public Sprite closedChest;
    public bool opened;
    public AudioClip openSound;

    private SpriteRenderer spriteRenderer;
    private Behaviour halo;
    private int playersAdjacent;
    private GameObject button;
    private const float yOffset = -150f;

    public void OpenChest()
    {
        if (!opened)
        {
            spriteRenderer.sprite = openedChest;
            opened = true;
            halo.enabled = false;

            if (button != null)
            {
                GameObject.Destroy(button);
            }

            GameManager.instance.DropLoot(Random.Range(1, 4), true);
            GameManager.instance.DropHealthPotion(transform.position, true);
            SoundManager.instance.PlaySounds(openSound);
        }
    }

    private void Start()
    {
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedChest;
        halo = (Behaviour)transform.parent.GetComponent("Halo");
        halo.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" && !opened)
        {
            playersAdjacent++;
            halo.enabled = true;

            if (button != null)
            {
                GameObject.Destroy(button);
            }

            var ps4 = collider.GetComponent<PlayerController>().playstationController;

            button = UIManager.instance.CreateAndShowButton(transform.position, yOffset, Direction.down, ps4);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            playersAdjacent--;
            if (playersAdjacent == 0)
            {
                halo.enabled = false;
                GameObject.Destroy(button);
            }
        }
    }
}
