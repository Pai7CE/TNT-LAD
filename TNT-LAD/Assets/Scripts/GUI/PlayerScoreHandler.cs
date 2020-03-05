﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreHandler : MonoBehaviour
{

  private int playerCount;

  void Start()
  {
    StaticPlayers.staticPlayers.Add(new StaticPlayerData(1, 2));
    StaticPlayers.staticPlayers.Add(new StaticPlayerData(2, 0));

    playerCount = 2;//GUIValues.PlayerList.Count;
    for (int i = 0; i < playerCount; i++) {
      gameObject.transform.GetChild(i).gameObject.SetActive(true);
    }
    foreach (StaticPlayerData staticPlayer in StaticPlayers.staticPlayers) {
      Debug.Log("Player " + staticPlayer.PlayerIndex + " has " + staticPlayer.PlayerScore + " points");
    }
  }

  void Update()
  {

  }
}