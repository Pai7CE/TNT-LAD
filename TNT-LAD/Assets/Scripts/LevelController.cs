﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//[ExecuteInEditMode]
public class LevelController : MonoBehaviour
{
  [Header("Parameters")]
  public int blockSize;
  public float spacing;
  public Vector3 startPos;

  [Header("Grid")]
  public int gridX;
  public int gridZ;

  [Header("Floor")]
  public GameObject floor;

  [Header("Block")]
  public GameObject defaultBlock;
  public GameObject destructibleBlock;
  public float blockHeight;

  [Header("Outer Wall")]
  public GameObject outerWall;

  private GameObject[,] floorMap;
  private GameObject[,] blockMap;

  private GameObject floorBlocks;
  private GameObject levelBlocks;

  private HandleLevelFile handleLevelFile;

  public enum blockType {DESTRUCTIBLE, DEFAULT};

  // Start is called before the first frame update
  void Start()
  {
    handleLevelFile = gameObject.GetComponent<HandleLevelFile>();

    CreateSceneStructure();
    Construct();
  }

  //creating scene structure
  private void CreateSceneStructure()
  {
    GameObject levelObjects = new GameObject("Level Objects");
    floorBlocks = new GameObject("Floor Blocks");
    floorBlocks.transform.parent = levelObjects.transform;
    levelBlocks = new GameObject("Level Blocks");
    levelBlocks.transform.parent = levelObjects.transform;
  }

  //constructs the map
  public void Construct()
  {
    if (File.Exists(handleLevelFile.GetFilePath()))
    {
      updateMapParams();
      blockMap = new GameObject[gridX, gridZ];
      floorMap = new GameObject[gridX, gridZ];

      PlaceLevelBlocks();
    }
    else
    {
      blockMap = new GameObject[gridX, gridZ];
      floorMap = new GameObject[gridX, gridZ];
    }
    GenerateFloor();
    GenerateWall();
  }

  private void updateMapParams()
  {
    string[] blockLine = handleLevelFile.GetFileData();
    gridX = blockLine.Length;
    gridZ = blockLine[0].ToCharArray().Length - 1; //whitespace is also part of the array
  }

  //Generates outer wall of play area
  private void GenerateWall()
  {
    //for (int x = -1; x < gridX+1; x++)
    //{
    //  Vector3 pos = new Vector3(startPos.x + x * spacing, 0, startPos.z - spacing);
    //  Instantiate(outerWall, pos, Quaternion.identity);
    //}
  }

  //generates the Floor of the Gamefield
  private void GenerateFloor()
  {
    for (int x = 0; x < gridX; x++)
    {
      for (int z = 0; z < gridZ; z++)
      {
        Vector3 pos = new Vector3(startPos.x + x * spacing, 0, startPos.z + z * spacing);
        floorMap[x, z] = Instantiate(floor, pos, Quaternion.identity) as GameObject;
        floorMap[x, z].transform.parent = floorBlocks.transform;
      }
    }
  }

  //sets the block at specified location
  public void SetBlock(int x, int z, LevelController.blockType blockType)
  {
    //out of bounds check
    if(x >= gridX || z >= gridZ)
    {
      Debug.LogError("Out of bounds! x:" + x + " z: " + z);
      return;
    }
    //placing block only of there is space available
    if(blockMap[x, z] == null)
    {
      Vector3 pos = new Vector3(startPos.x + x * spacing, blockHeight / 2, startPos.z + z * spacing);
      switch (blockType)
      {
        case blockType.DEFAULT:
          blockMap[x, z] = Instantiate(defaultBlock, pos, Quaternion.identity) as GameObject;
          break;
        case blockType.DESTRUCTIBLE:
          blockMap[x, z] = Instantiate(destructibleBlock, pos, Quaternion.identity) as GameObject;
          break;
        default:

          break;
      }
      blockMap[x, z].transform.parent = levelBlocks.transform;
    }
    else
    {
      Debug.LogError("No Space available");
    }
  }

  //places the blocks in the according .txt file
  private void PlaceLevelBlocks()
  {

    string[] blockLine = handleLevelFile.GetFileData();

    for (int x = 0; x < blockLine.Length; x++)
    {
      char[] blocks = blockLine[x].ToCharArray();
      for (int z = 0; z < blocks.Length; z++)
      {
        if (blocks[z] == handleLevelFile.GetCharDestructible())
        {
          SetBlock(x, z, blockType.DESTRUCTIBLE);
        }
        if (blocks[z] == handleLevelFile.GetCharDefault())
        {
          SetBlock(x, z, blockType.DEFAULT);
        }
      }
    }
  }

  public void deleteBlock(int x, int z)
  {
    if(blockMap[x, z] != null)
    {
      Destroy(blockMap[x, z]);
    }
    else
    {
      Debug.LogError("No block at those coordinates");
    }
  }

  private void ClearBlocks(bool clearAll)
  {
    if (blockMap != null)
    {
      for (int x = 0; x < blockMap.GetLength(0); x++)
      {
        for (int z = 0; z < blockMap.GetLength(1); z++)
        {
          Destroy(blockMap[x, z]);
        }
      }
    }
    if (clearAll)
    {
      if (floorMap != null)
      {
        for (int x = 0; x < floorMap.GetLength(0); x++)
        {
          for (int z = 0; z < floorMap.GetLength(1); z++)
          {
            Destroy(floorMap[x, z]);
          }
        }
      }
    }
  }



  [CustomEditor(typeof(LevelController))]
  class DecalMeshHelperEditor : Editor
  {
    LevelController levelController;
    HandleLevelFile levelFile;
    int xBlock = 0;
    int zBlock = 0;

    private void OnEnable()
    {
      levelController = (LevelController)target;
      levelFile = (HandleLevelFile)target;
    }
    
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();
      if (GUILayout.Button("Reconstruct"))
      {
        if (Application.isPlaying)
        {
          levelController.Construct();
        }
      }

      //GUILayout.BeginHorizontal();
      xBlock = EditorGUILayout.IntField("X", xBlock, GUILayout.ExpandWidth(false));
      zBlock = EditorGUILayout.IntField("Z", zBlock, GUILayout.ExpandWidth(false));
      //GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Default")) 
      {
        if (Application.isPlaying)
        {
          levelController.SetBlock(xBlock, zBlock, blockType.DEFAULT);
        }
      }
      if (GUILayout.Button("Destructible"))
      {
        if (Application.isPlaying)
        {
          levelController.SetBlock(xBlock, zBlock, blockType.DESTRUCTIBLE);
        }
      }
      if (GUILayout.Button("Delete"))
      {
        if (Application.isPlaying)
        {
          levelController.deleteBlock(xBlock, zBlock);
        }
      }
      GUILayout.EndHorizontal();
      if(GUILayout.Button("Clear Blocks"))
      {
        if (Application.isPlaying)
        {
          levelController.ClearBlocks(false);
        }
      }
      if (GUILayout.Button("Clear All"))
      {
        if (Application.isPlaying)
        {
          levelController.ClearBlocks(true);
        }
      }
      if (GUILayout.Button("Save Map"))
      {
        if (Application.isPlaying)
        {
          
        }
      }
    }
  }

}
