using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a level based on an input array of data
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    //Single Instance of the level generator
    private static LevelGenerator instance;

    //Location that the level starts from
    private readonly Vector3 levelStartPos = new Vector3(0, 0, 0);

    //Prefabs for walkable and non-walkable tiles
    [SerializeField]
    private GameObject walkableTilePrefab, impassableTilePrefab, playerStartTilePrefab, playerEndTilePrefab, foxStartTilePrefab;

    //Infomation about the level
    public Vector2Int LevelSize { get; private set; }
    public int[,] LevelData { get; private set; }
    public GameObject[,] LevelCubes { get; private set; }

    //Enum for Types of tile
    public enum TileType
    {
        TILE_WALKABLE = 0,
        TILE_IMPASSABLE = 1,
        TILE_FOX_START = 2,
        TILE_PLAYER_START = 3,
        TILE_PLAYER_FINISH = 4
    }

    //Constants for which value in array corrisponds to which type of cube
    //Dictonary <arrayValue, prefabToInstantiate>
    public Dictionary<TileType, GameObject> arrayTilesPairs = new Dictionary<TileType, GameObject>();

    //Level data used to spawn the level
    [SerializeField]
    private LevelData createLevelData;
   
    /// <summary>
    /// Gets the singleton instance of hte level Generator
    /// </summary>
    /// <returns></returns>
    public static LevelGenerator GetInstance()
    {
        if(instance == null)
        {
            GameObject newGO = new GameObject();
            instance = newGO.AddComponent<LevelGenerator>();
        }

        return instance;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        //If an instance of the levelgenerator already exists then destroy this object
        if (instance != null && instance != this)
        {
            //Instance of the level generator already
            //exists - delete this one and exit
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        //Setup Dictonary for array value pairs
        arrayTilesPairs = new Dictionary<TileType, GameObject>()
        {
            {TileType.TILE_WALKABLE, walkableTilePrefab },       //Walkable Tile is 0
            {TileType.TILE_IMPASSABLE, impassableTilePrefab },     //Impassable Tile is 1
            {TileType.TILE_FOX_START, foxStartTilePrefab },       //Fox Start Tile is 2
            {TileType.TILE_PLAYER_START, playerStartTilePrefab },    //Player Start Tile is 3
            {TileType.TILE_PLAYER_FINISH, playerEndTilePrefab },      //Player End Tile is 4
        };
    }

    /// <summary>
    /// Start the level
    /// </summary>
    private void StartLevel()
    {
        GenerateLevel(createLevelData.LevelArray);
    }

    /// <summary>
    /// Generate a level in the world based on an input array of data
    /// </summary>
    /// <param name="levelArray"></param>
    private void GenerateLevel(int[,] levelArray)
    {
        //Set the size of the level
        LevelSize = new Vector2Int(levelArray.GetLength(0), levelArray.GetLength(1));

        //Initalise the cubes array
        LevelCubes = new GameObject[LevelSize.x, LevelSize.y];
        //Set the level data to the newly received level array
        LevelData = levelArray;

        //Loop all cubes in the level array and set passable/inpassable cubes in the 
        //world as object
        for(int x = 0; x < LevelSize.x; x++)
        {
            for(int y = 0; y < LevelSize.y; y++)
            {
                //Generate either passable or impassable cube at given position
                Vector3 createPos = levelStartPos + new Vector3(x, 0f, y);

                //Create a new tile and decide what type it is based on the valie within the array
                GameObject newTile = Instantiate(arrayTilesPairs[(TileType)LevelData[x,y]], gameObject.transform, true);

                //Set the new tiles position to the correct create pos
                newTile.transform.position = createPos;

                //Add new cube to the cubes array
                LevelCubes[x, y] = newTile;
            }
        }
    }

    /// <summary>
    /// Gets the center point of the level in world space
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLevelCenter() {

        //Get half of the level size
        Vector3 halfLevelSize = new Vector3((LevelSize.x * 0.5f) - (GameStateManager.tileMoveAmount * 0.5f), 0.0f, (LevelSize.y * 0.5f) - (GameStateManager.tileMoveAmount * 0.5f));

        //Convert this to world space and return
        Vector3 levelCenter = levelStartPos + (halfLevelSize * GameStateManager.tileMoveAmount);
        return levelCenter;

    }

    /// <summary>
    /// Checks if all of the level cubes are visible from a camera
    /// </summary>
    /// <returns></returns>
    public bool AllCubesVisible(Camera cam)
    {
        //Calculate the camera view frustrum
        Plane[] camViewPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        //Get all of the Renderers from the cubes and check if they are within the 
        //cameras view frustrum
        foreach(GameObject cube in LevelCubes)
        {
            Renderer cubeRenderer = cube.GetComponentInChildren<Renderer>();

            //If a single cube is not visible return false
            if(!GeometryUtility.TestPlanesAABB(camViewPlanes, cubeRenderer.bounds))
            {
                return false;
            }
        }

        //All cubes were visible
        return true;

    }

    #region Event Subs/Unsubs
    private void OnEnable()
    {
        GameStateManager.LevelStarted += StartLevel;
    }
    private void OnDisable()
    {
        GameStateManager.LevelStarted -= StartLevel;
    }
    #endregion
}
