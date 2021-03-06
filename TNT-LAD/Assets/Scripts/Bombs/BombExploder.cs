﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExploder : MonoBehaviour
{
  public float fuseTimeSeconds;
  public int explosionRange;
  public GameObject explosion;
  [HideInInspector] public List<GameObject> playersInsideBomb;
  private BoxCollider playerExitTrigger;
  private BoxCollider bombCollider;

  void Start()
  {
    bombCollider = gameObject.GetComponent<BoxCollider>();
    bombCollider.enabled = true;

    DisableCollisionsForPlayersInsideBomb();

    //Additional trigger just to detect when player leaves bomb collider
    playerExitTrigger = (BoxCollider)gameObject.AddComponent(typeof(BoxCollider));
    playerExitTrigger.isTrigger = true;
    playerExitTrigger.center = bombCollider.center;
    playerExitTrigger.size = bombCollider.size;

    Invoke("ExplodeBomb", fuseTimeSeconds);
  }

  private Collider[] GetObjectsCollidingWithBomb()
  {
    return Physics.OverlapBox(gameObject.transform.position + bombCollider.center, bombCollider.size / 2);
  }

  void DisableCollisionsForPlayersInsideBomb()
  {
    foreach (var col in GetObjectsCollidingWithBomb())
    {
      if (col.gameObject.tag == "Player")
      {
        Physics.IgnoreCollision(bombCollider, col.GetComponent<CharacterController>(), true);
        playersInsideBomb.Add(col.gameObject);
      }
    }
  }

  void OnTriggerExit(Collider col)
  { 
    if(playersInsideBomb.Contains(col.gameObject))
    {
      Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), col.GetComponent<CharacterController>(), false);
      playersInsideBomb.Remove(col.gameObject);
    }
    if(playersInsideBomb.Count == 0)
    {
      Destroy(playerExitTrigger);
    }
  }

  public void ExplodeBomb()
  {
    foreach(Vector3 pos in GetExplosionPositions())
    {
      var explosionObj = Instantiate(explosion, pos, Quaternion.identity);
      explosionObj.GetComponent<ExplosionBehaviour>().originalBombPosition = gameObject.transform.position;
    }
    SoundManager.PlaySound(SoundManager.Sound.Explosion);
    Destroy(gameObject);
  }

  private List<Vector3> GetExplosionPositions()
  {
    var explosionPositions = new List<Vector3>();
    explosionPositions.Add(gameObject.transform.position);
    //for each direction (n, s, e, w)
    for (int dir = 0; dir < 4; dir++) 
    {
      bool positionValid = true;
      //go through ranges
      for (int curRange = 1; curRange < explosionRange; curRange++)
      {
        Vector3 position = gameObject.transform.position + GetExplosionPointAddition(dir, curRange);
        foreach (var collider in Physics.OverlapSphere(position, 0.01f))
        {
          if(collider.gameObject.TryGetComponent(out BlockData blockData))
          {
            if(blockData.blockType == BlockData.BlockType.DESTRUCTIBLE)
            {
              explosionPositions.Add(position);
            }
            positionValid = false;
          }
        }
        if (positionValid)
        {
          explosionPositions.Add(position);
        }
        else
        {
          break;
        }
      }
    }
    return explosionPositions;
  }

  private Vector3 GetExplosionPointAddition(int direction, int range)
  {
    Vector3 pointAddition = new Vector3();
    switch (direction)
    {
      case 0:
        pointAddition = new Vector3(0, 0, range);
        break;
      case 1:
        pointAddition = new Vector3(range, 0, 0);
        break;
      case 2:
        pointAddition = new Vector3(0, 0, -range);
        break;
      case 3:
        pointAddition = new Vector3(-range, 0, 0);
        break;
      default:
        Debug.LogError("Invalid directional parameter");
        break;
    }
    return pointAddition;
  }

}
