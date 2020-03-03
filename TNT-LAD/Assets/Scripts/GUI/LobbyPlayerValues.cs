﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyPlayerValues : MonoBehaviour
{
  private int playerNumber { get; set; }

  private bool isSelectedByPlayer { get; set; }

  private InputDevice inputDevice { get; set; }

  private void Start()
  {
    SetIsSelected(false);
  }

  public bool getIsSelected()
  {
    return isSelectedByPlayer;
  }

  public void SetIsSelected(bool isSelectedByPlayer)
  {
    this.isSelectedByPlayer = isSelectedByPlayer;
  }

  public InputDevice GetInputDevice()
  {
    return inputDevice;
  }

  public void SetInputDevice(InputDevice inputDevice)
  {
    this.inputDevice = inputDevice;
  }

  public void SetPlayerNumber(int playerNumber)
  {
    this.playerNumber = playerNumber;
  }

  public int GetPlayerNumber()
  {
    return playerNumber;
  }
}