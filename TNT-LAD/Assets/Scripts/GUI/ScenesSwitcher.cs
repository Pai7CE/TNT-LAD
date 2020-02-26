﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesSwitcher : MonoBehaviour
{
  public void SwitchToGame()
  {
    SceneManager.LoadScene("Level 1");
  }

  public void SwitchToMainMenu()
  {
    SceneManager.LoadScene("MainMenu");
  }
}