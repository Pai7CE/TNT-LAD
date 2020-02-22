﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  public float walkSpeed;
  private Vector2 axis;

  void Update()
  {
    MovePlayer();
  }

  void MovePlayer()
  {
    Vector3 movement = new Vector3(axis.x, 0, axis.y) * walkSpeed;
    transform.Translate(movement * Time.deltaTime, Space.World);

    Vector3 newDirection = Vector3.RotateTowards(transform.forward, new Vector3(axis.x, 0, axis.y), Time.deltaTime * 20f, 0.0f);
    transform.rotation = Quaternion.LookRotation(newDirection * Time.deltaTime);
  }

  void OnMovement(InputValue inputValue)
  {
    axis = inputValue.Get<Vector2>();
  }
}
