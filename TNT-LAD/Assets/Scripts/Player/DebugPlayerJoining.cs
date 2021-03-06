﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class DebugPlayerJoining : MonoBehaviour
{
  [HideInInspector]
  public InputDevice[] inputDevices;
  [HideInInspector]
  public Player[] players = new Player[]{
    new Player(0),
    new Player(1),
    new Player(2),
    new Player(3),
  };

  void Start()
  {
    inputDevices = InputDevice.all.ToArray();
  }

  void Update()
  {
    foreach(var player in players)
    {
      if(player.isActive && player.isNewPlayer)
      {
        player.isNewPlayer = false;
        JoinPlayer(player.playerPrefab, inputDevices[player.deviceIndex]);
      }
    }
  }

  private void JoinPlayer(GameObject player, InputDevice device)
  {
    string controlScheme = (device is Gamepad) ? "Gamepad" : "Keyboard";
    PlayerInput playerInput = PlayerInput.Instantiate(player, controlScheme: controlScheme, pairWithDevice: device);
    playerInput.SwitchCurrentControlScheme(controlScheme, new InputDevice[] { device }); //control scheme has to be set twice?
  }
}

#region For Debugging porposes. Will eventually be replaced by a Player/Character selection menu

[Serializable]
public class Player
{
  public int playerIndex;
  [SerializeField]
  public bool isActive;
  [SerializeField]
  public bool isNewPlayer;
  [SerializeField]
  public GameObject playerPrefab;
  [SerializeField]
  public int deviceIndex;

  public Player(int index) 
  { 
    playerIndex = index;
    isNewPlayer = true;
  }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DebugPlayerJoining))]
public class CustomInspector : Editor
{
  public override void OnInspectorGUI()
  {
    var playerJoining = target as DebugPlayerJoining;
    string[] inputDeviceStrings;

    DrawDefaultInspector();

    foreach(var player in playerJoining.players)
    {
      EditorGUILayout.LabelField($"Player{player.playerIndex} -------------------------");
      player.playerPrefab = (GameObject)EditorGUILayout.ObjectField("PlayerPrefab", player.playerPrefab, typeof(GameObject), false);
      if (EditorApplication.isPlaying)
      {
        inputDeviceStrings = Array.ConvertAll(playerJoining.inputDevices, x => x.ToString());
        player.isActive = EditorGUILayout.Toggle("isActive", player.isActive);
        player.deviceIndex = EditorGUILayout.Popup("InputDevice", player.deviceIndex, inputDeviceStrings);
      }
    }
  }
}
#endif
#endregion