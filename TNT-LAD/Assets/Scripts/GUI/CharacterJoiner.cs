﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterJoiner : MonoBehaviour
{
  private Dictionary<int, LobbyPlayerValues> playerDict;

  private int playerCount = 1;

  private void Start()
  {
    playerDict = new Dictionary<int, LobbyPlayerValues>();
  }

  // Update is called once per frame
  void Update()
  {
    if (IsConnectionButtonIsPressed())
    {
      LobbyPlayerValues newPlayer = GetNewPlayerValues();
      if (newPlayer != null) {
        playerDict.Add(playerCount, newPlayer);
        
      }
    }
  }

  private LobbyPlayerValues GetNewPlayerValues()
  {
    foreach (GameObject curPlayerSelection in GameObject.FindGameObjectsWithTag("GUIPlayer")) 
    {
      LobbyPlayerValues valuesOfCurPlayer = curPlayerSelection.GetComponent<LobbyPlayerValues>();
      if (!valuesOfCurPlayer.getIsSelected())
      {
        valuesOfCurPlayer.SetIsSelected(true);
        curPlayerSelection.transform.GetChild(0).GetComponent<Text>().text = "Player " + playerCount;
        playerCount++;
        curPlayerSelection.transform.GetChild(1).GetComponent<Image>().enabled = false;
        return valuesOfCurPlayer;
      }
    }
    return null;
  }

  private bool IsConnectionButtonIsPressed()
  {
    foreach (InputDevice curInputDevice in InputDevice.all) {
      if(curInputDevice is Keyboard) {
        if (((Keyboard)curInputDevice).spaceKey.wasPressedThisFrame) {
          return true;
        }
      }
      else if (curInputDevice is Gamepad) {
        if (((Gamepad)curInputDevice).startButton.wasPressedThisFrame) {
          return true;
        }
      }
    }
    return false;
  } 
}
