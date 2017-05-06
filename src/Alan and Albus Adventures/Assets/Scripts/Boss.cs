using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Boss : Enemy 
{
	public string BossName;
	public Text nameText;

	protected override void Start()
	{
		base.Start();

		nameText.text = name;
	}
}
