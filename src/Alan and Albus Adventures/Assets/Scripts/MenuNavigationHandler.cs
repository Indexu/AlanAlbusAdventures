using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;

public class MenuNavigationHandler : MonoBehaviour {
	//private IList<Player> player;
	public EventSystem eventSystem;
	public GameObject selectedObject;
	public bool selectedItem;
	public void Awake()
	{
		//player = ReInput.players.AllPlayers;
        ReInput.ControllerConnectedEvent += OnControllerConnected;
	}

	private void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
		//playstationController = args.name.Contains("Sony");
    }

	private void OnDestroy()
    {
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
    }

	private void onDisable()
	{
		Debug.Log("bye");
		selectedItem = false;
	}
	private void Update()
	{
		checkInput();
	}

	private void checkInput()
	{
		var analogStick = Rewired.ReInput.players.GetPlayer(0).GetAxis("Move Vertical");
		if(analogStick < 0)
		{
			Debug.Log("down");
			eventSystem.SetSelectedGameObject(selectedObject);
			selectedItem = true;
        }
		if(analogStick > 0)
		{
			Debug.Log("up");
			eventSystem.SetSelectedGameObject(selectedObject);
			selectedItem = true;
		}

	}

}
