using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimationController : MonoBehaviour {
	public Sprite openedChest;
	public Sprite closedChest;
	private SpriteRenderer spriteRenderer; 

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = closedChest;
	}
	public void OpenChest()
	{
		if (spriteRenderer.sprite == closedChest)
		{
			spriteRenderer.sprite = openedChest;
		}
		else
		{
			spriteRenderer.sprite = closedChest;
		}
	}
}
