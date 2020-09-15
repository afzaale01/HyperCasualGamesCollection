using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor script to save the current level to data
/// </summary>
public class LevelSaver : MonoBehaviour
{
    private const string playerStartTileTag = "PlayerStart";
    private const string playerEndTileTag = "PlayerEnd";
    private const string foxStartTileTag = "FoxStart";

    private const string levelSavePath = "Assets/Levels/";

    //Size of the sphere to use when we sphere cast to check if a position if free
    private const float sphereCastRadius = 0.1f;
    private const float levelTileSize = 1f;


    [MenuItem("Level/Save Current Level")]
    private static void SaveOpenLevel()
    {
        Debug.Log("[LEVEL SAVE] Starting Level Save");


        //Find the bottom left corner
        Vector2 bottomLeftCorner = Vector2.zero;
        bool foundBottomLeftCorner = FindBottomLeftCorner(ref bottomLeftCorner);
        if (!foundBottomLeftCorner)
        {
            return;
        }

        //Convert to a vector 3 so we can use it in 3D sphere casting
        Vector3 levelStartPos = new Vector3(bottomLeftCorner.x, 0f, bottomLeftCorner.y);

        //Then go up and right to find the level size
        Vector2Int levelSize = GetLevelGridSize(levelStartPos);

        //Then for the level size fill in all of the tile types
        int[,] levelTiles = CaptureLevelCubes(levelSize, levelStartPos);
        Debug.Log("[LEVEL SAVING] Level Generated");

        //Save to a scripable object
        SaveLevelToSO(levelTiles, levelSize);

        Debug.Log("[LEVEL SAVING] Save Complete");
    }

    /// <summary>
    /// Saves the data to a scriptable object
    /// </summary>
    private static void SaveLevelToSO(int[,] levelData, Vector2Int levelSize) 
    {
        Debug.Log("[LEVEL SAVING] Saving Level...");

        //Create a scriptable object and assign data
        LevelData saveObject = ScriptableObject.CreateInstance<LevelData>();
        saveObject.levelName = "Untitled Level";
        saveObject.LevelArray = levelData;
        saveObject.levelSize = levelSize;
        //Set the file name to be unique by current date/time
        saveObject.name = "GeneratedLevel_" + EditorSceneManager.GetActiveScene().name; //SO name

        //Save asset to asset data base (i.e our project window)
        AssetDatabase.CreateAsset(saveObject, levelSavePath + saveObject.name + ".asset");
        AssetDatabase.SaveAssets();

        //Show our new asset in the project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = saveObject;

        Debug.Log("[LEVEL SAVING] Level Saved to: " + levelSavePath + saveObject.name + ".asset");
    }

    /// <summary>
    /// Captures all of the level cubes and saves them to any array of ints
    /// </summary>
    /// <returns></returns>
    private static int[,] CaptureLevelCubes(Vector2Int levelSize, Vector3 levelStartPos)
    {
        int[,] levelGrid = new int[levelSize.x, levelSize.y];

        for (int x = 0; x < levelSize.x; ++x)
        {
            for (int y = 0; y < levelSize.y; ++y)
            {
                //Position to check at
                Vector3 checkPos = levelStartPos + new Vector3(x, 0, y);

                //Add the gameobject at that position to the cube array
                var collision = Physics.OverlapSphere(checkPos, sphereCastRadius);
                if (collision != null)
                {
                    //Set the cube in the cube grid, pass the parent because we always hit the
                    //floor (child) object
                    levelGrid[x, y] = (int)GetTileType(collision[0].transform.parent.gameObject);
                }
            }
        }

        //Return the array
        return levelGrid;
    }


    /// <summary>
    /// Gets the type of tile based on the game object
    /// </summary>
    /// <param name="tileGO">Tile Object to test</param>
    /// <returns></returns>
    private static LevelGenerator.TileType GetTileType(GameObject tileGO)
    {

        //Check if it is any of the start tiles, these don't have scripts
        //atached so we must compare the tags
        switch (tileGO.tag)
        {
            case (playerStartTileTag):
                return LevelGenerator.TileType.TILE_PLAYER_START;
            case (playerEndTileTag):
                return LevelGenerator.TileType.TILE_PLAYER_FINISH;
            case (foxStartTileTag):
                return LevelGenerator.TileType.TILE_FOX_START;
            default:
                //Do Nothing
                break;
        }

        //Check if it is a walkable tile
        if (tileGO.GetComponentInChildren<WalkableTile>())
        {
            return LevelGenerator.TileType.TILE_WALKABLE;
        }

        //Is not any specified type, thus must be impassable
        return LevelGenerator.TileType.TILE_IMPASSABLE;
    }

    /// <summary>
    /// Gets the bottom left corner of the level
    /// </summary>
    /// <param name="bottomLeft">(REF) Vector2 to fill with position</param>
    /// <returns></returns>
    private static bool FindBottomLeftCorner(ref Vector2 bottomLeft)
    {
        //Find the start tile in the level
        GameObject startTile = GameObject.FindGameObjectWithTag(playerStartTileTag);
        //If there is no start tile then error out
        if (!startTile)
        {
            Debug.LogError("[LEVEL SAVE - FAILED] Could not find start tile");
            return false;
        }

        Vector3 startTilePos = startTile.transform.position;

        //Move as far down as we can go from the start tile
        Vector2 currentPos = new Vector2(startTile.transform.position.x, startTile.transform.position.z);
        while (IsTileAtPos(new Vector3(currentPos.x, 0.5f, currentPos.y - levelTileSize)))
        {
            //Decrease our y pos so we keep moving down
            currentPos.y -= levelTileSize;
        }

        //Move as far left as we can
        while (IsTileAtPos(new Vector3(currentPos.x - levelTileSize, 0.5f, currentPos.y)))
        {
            //Decrease our y pos so we keep moving down
            currentPos.x -= levelTileSize;
        }

        //We are now at the bottom left corner return it
        bottomLeft = currentPos;
        return true;
    }

    /// <summary>
    /// Gets the size of the level grid
    /// </summary>
    /// <param name="levelStartPos">Bottom Left Square of the level to start at</param>
    private static Vector2Int GetLevelGridSize(Vector3 levelStartPos)
    {
        Vector2Int levelSize = Vector2Int.zero;

        //Go along the x/y axis and sphere case to see if there is an
        //object at that posiion, once we have found a blank space then
        //we have got the x/y size
        while (Physics.CheckSphere(new Vector3(levelSize.x, 0, 0) + levelStartPos, sphereCastRadius))
        {
            levelSize.x++;
        }

        while (Physics.CheckSphere(new Vector3(0, 0, levelSize.y) + levelStartPos, sphereCastRadius))
        {
            levelSize.y++;
        }

        return levelSize;
    }

    /// <summary>
    /// Checks if a tile exists at the given position
    /// </summary>
    /// <param name="pos">Position to Check</param>
    private static bool IsTileAtPos(Vector3 pos)
    {
        //If we have have a collision with an object then we are hitting a tile
        Collider[] hitColliders = Physics.OverlapSphere(pos, sphereCastRadius);

        //Return if we hit something
        return hitColliders.Length > 0;
    }

}
