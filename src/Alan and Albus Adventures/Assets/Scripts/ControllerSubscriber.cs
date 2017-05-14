using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ControllerSubscriber : MonoBehaviour
{
    public static bool AlanPS4;
    public static bool AlbusPS4;

    private void Awake()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    private void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerId == 0)
        {
            AlanPS4 = args.name.Contains("Sony");
        }
        else
        {
            AlbusPS4 = args.name.Contains("Sony");
        }
    }
}
