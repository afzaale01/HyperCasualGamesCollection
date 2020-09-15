using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathfinding : MonoBehaviour
{
    //Grid of Cubes and data used for pathfinding
    private GameObject[,] levelCubes;
    private int[,] levelData;
    private Vector2Int levelSize;
    //Distances from the starting node for each cube
    //Coordinates match cubes array
    private float[,] cubeDistances;

    //List of tenative cubes
    private HashSet<GameObject> tentativesList;
    //List of unvisited cubes
    private HashSet<GameObject> unvisitedCubes;

    //Grid Start Cube, this cube should be at the bottom left (0,0) point of the level
    private Vector3 levelStartPos = new Vector3(0, 0, 0);

    //The weighting that the heuristic analysis should have when finding a path
    private float heuristicMutiplyer = 1.0f;


    private void Start()
    {
        //Get the data about the current level
        GetLevelData();
    }

    private void GetLevelData()
    {
        //Initalise Cube Grid as this won't change when finding
        //a new path
        if (levelCubes == null)
        {
            LevelGenerator levelGen = LevelGenerator.GetInstance();
            if (levelGen != null)
            {
                levelCubes = levelGen.LevelCubes;
                levelData = levelGen.LevelData;
                levelSize = levelGen.LevelSize;
            }
        }
    }

    /// <summary>
    /// Get an A* Path between 2 points on the level grid
    /// </summary>
    /// <param name="startPos">Start Position of the Pathfind</param>
    /// <param name="endPos">End Position of the Pathfind</param>
    /// <returns></returns>
    public List<GameObject> GetAStarPath(Vector3 startPos, Vector3 endPos)
    {

        //Null Level Data
        if (levelCubes == null && levelData == null)
        {
            GetLevelData();
        }

        //Initalise Data Structures
        tentativesList = new HashSet<GameObject>(); //Only needs to be init-ed as an empty hashmap - no need for function
        InitUnvisitedSet();
        InitImpassableCubes();
        InitCubeDistances();

        //Get the start and end position within the array
        Vector2Int startPosInArray = GetWorldPosInArray(new Vector2(startPos.x, startPos.z));
        Vector2Int endPosInArray = GetWorldPosInArray(new Vector2(endPos.x, endPos.z));

        //Check that positions are valid - If any positions inside the array 
        if (!PositionsWithinArrayBounds(startPosInArray, endPosInArray))
        {
            //Return an empty list - no path can be found between points 
            //outside the array 
            return new List<GameObject>();
        }

        //Get the start and end cubes
        GameObject startCube = levelCubes[startPosInArray.x, startPosInArray.y];
        GameObject endCube = levelCubes[endPosInArray.x, endPosInArray.y];

        //Check that start and end cubes are valid
        if(!startCube || !endCube)
        {
            //Return an empty listStart/End Cubes are not valid
            return new List<GameObject>();
        }
        
        //Check that start and end cubes are in the unvisited set
        //and thus can be visited
        if(!unvisitedCubes.Contains(startCube) || !unvisitedCubes.Contains(endCube))
        {
            return new List<GameObject>();
        }

        //Run Pathfind Function - attempt to find a path
        AStarPathfind(startCube, endCube);

        //If our pathfinding found the end cube in its search then
        //trace pack the best route - then return it
        return TraceBackAStarPath(startCube, endCube);

    }

    /// <summary>
    /// Run the A* Pathfinding algorithm, working through tiles,
    /// until we find the destination or visit all cubes
    /// </summary>
    /// <param name="startCube"></param>
    /// <param name="endCube"></param>
    private void AStarPathfind(GameObject startCube, GameObject endCube)
    {
        //Init Values
        bool finished = false; //Stores if we have reached the end cube or visited everycube
        GameObject currentCube = startCube; //The current cube we are assessing

        //Set the distance of the start cube to 0 as it will always be 0
        Vector2Int startCubePosInArray = GetCubePosInArray(startCube);
        cubeDistances[startCubePosInArray.x, startCubePosInArray.y] = 0;

        //Loop until we have exausted our options
        while (!finished)
        {

            //Get the distance value from current cube
            Vector2Int currentCubePosInArray = GetCubePosInArray(currentCube);
            float currentCubeDist = cubeDistances[currentCubePosInArray.x, currentCubePosInArray.y] + GetHeuristicDistance(currentCube, endCube);

            //Get the neighbours of the current cube
            List<GameObject> currentNeighbours = GetNeighbours(currentCube);
            //Add all neighbours to the tentative list
            foreach (GameObject neighbour in currentNeighbours) { 
                tentativesList.Add(neighbour);
            }

            //Update the distances of all of the tentatives in the tentative list
            foreach(GameObject tentative in tentativesList)
            {
                Vector2Int tentativePosInArray = GetCubePosInArray(tentative);
                float tentativeDist = cubeDistances[tentativePosInArray.x, tentativePosInArray.y];
                //If the currentCube distance + 1 is greater than the tentative distance, then keep the tentative
                //distance the same, else update the tentative distance to the the current cube distance + 1
                tentativeDist = currentCubeDist + 1 > tentativeDist ? tentativeDist : currentCubeDist + 1;
                cubeDistances[tentativePosInArray.x, tentativePosInArray.y] = tentativeDist;
            }

            //Remove the current cube from the unvisited and tentative list
            float lowestDistance = Mathf.Infinity;
            unvisitedCubes.Remove(currentCube);
            tentativesList.Remove(currentCube);

            foreach(GameObject tentative in tentativesList)
            {
                //Update the distances again
                Vector2Int tentativePosInArray = GetCubePosInArray(tentative);
                float tentativeDist = cubeDistances[tentativePosInArray.x, tentativePosInArray.y];
                //If the currentCube distance + 1 is greater than the tentative distance, then keep the tentative
                //distance the same, else update the tentative distance to the the current cube distance + 1
                tentativeDist = currentCubeDist + 1 > tentativeDist ? tentativeDist : currentCubeDist + 1;
                cubeDistances[tentativePosInArray.x, tentativePosInArray.y] = tentativeDist;

                //Find the tentative with the lowest distance and assign it to as the 
                //next cube to process
                if(tentativeDist < lowestDistance)
                {
                    lowestDistance = tentativeDist;
                    currentCube = tentative;
                }
            }

            //Check if we are finished
            if(tentativesList.Count == 0){
                finished = true;
            }
        }
    }

    /// <summary>
    /// Retreives the best path after the pathfinding has been completed
    /// </summary>
    /// <param name="startCube">Start Cube of the pathfind</param>
    /// <param name="endCube">End cube of the pathfind</param>
    /// <param name="includeStartCube">Whether the start cube should be included in the returned path</param>
    /// <returns>The best path from A* Pathfinding</returns>
    private List<GameObject> TraceBackAStarPath(GameObject startCube, GameObject endCube, bool includeStartCube = false)
    {
        //Create list to add to and return path
        List<GameObject> path = new List<GameObject>();

        //Work from the end cube back to the start cube
        GameObject currentCube = endCube;

        //Loop until we get back to the start cube
        while(currentCube != startCube)
        {
            //Null Check the current cube
            if (!currentCube)
            {
                return new List<GameObject>();
            }

            //Add the current cube to the list
            path.Add(currentCube);

            //Get all the neighbours of the currrent cube - ignoring the unvisited set because
            //all tiles have been visited as a result of the AStartPathFind()
            List<GameObject> neighbours = GetNeighbours(currentCube, ignoreUnvisitedSet: true);

            /*
            Find the best (shortest distance) neighbour of the neighbours
            of this cube and assign it to the current cube so that we can loop
            around and add it to the list
            */
            float lowestDistance = float.PositiveInfinity;
            GameObject bestNeighbour = null;
            foreach(GameObject neighbour in neighbours)
            {
                //Check if the neighbours distance is the lowest we have
                //found so far
                Vector2Int neighbourPosInArray = GetCubePosInArray(neighbour);
                float neighbourDist = cubeDistances[neighbourPosInArray.x, neighbourPosInArray.y];
                if (neighbourDist < lowestDistance)
                {
                    //Assign this as the new best cube
                    lowestDistance = neighbourDist;
                    bestNeighbour = neighbour;
                }
            }

            //Assign current cube to best neighbour
            currentCube = bestNeighbour;
        }

        //Check if we should include the start cube
        if (includeStartCube)
        {
            path.Add(startCube);
        }

        //Reverse the route we found (because we started at the end point and worked back)
        //and return it
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Calculates the heuristic distance from the supplied node to the goal
    /// </summary>
    /// <param name="cube">Supplied node</param>
    /// <returns>Heuristic representing estimated distance from supplied node to goal </returns>
    private float GetHeuristicDistance(GameObject currentCube, GameObject endCube)
    {
        float xDistance = Mathf.Abs((int)currentCube.transform.position.x - (int)endCube.transform.position.x);
        float yDistance = Mathf.Abs((int)currentCube.transform.position.y - (int)endCube.transform.position.y);

        return heuristicMutiplyer * (xDistance + yDistance);
    }

    /// <summary>
    /// Gets the neighbours of the given cube
    /// Calls GetNeighbours(int arrayX, int arrayY, bool ignoreUnvisitedSet = false) after converting the cubes
    /// position in to it's position within the array
    /// </summary>
    /// <param name="cube">Cube to get neighbours for</param>
    /// <param name="ignoreUnvisitedSet">Include cubes outside the unvisited set</param>
    /// <returns></returns>
    private List<GameObject> GetNeighbours(GameObject cube, bool ignoreUnvisitedSet = false)
    {
        Vector2Int cubePosInArray = GetCubePosInArray(cube);
        return GetNeighbours(cubePosInArray.x, cubePosInArray.y, ignoreUnvisitedSet);
    }

    /// <summary>
    /// Gets the neighbouring cubes given a position within the cubes array 
    /// </summary>
    /// <param name="arrayX">X Position within the array</param>
    /// <param name="arrayY">Y Position within the array</param>
    /// <param name="ignoreUnvisitedSet"></param>
    /// <returns></returns>
    private List<GameObject> GetNeighbours(int arrayX, int arrayY, bool ignoreUnvisitedSet = false)
    {
        List<GameObject> neighboursList = new List<GameObject>();

        //Check to the left/right/up/down of the current array pos
        //and add a cube to the list if it is in a valid position
        //LEFT
        if(PositionWithinArrayBounds(arrayX + 1, arrayY))
        {
            if(unvisitedCubes.Contains(levelCubes[arrayX +1, arrayY]) || ignoreUnvisitedSet){
                neighboursList.Add(levelCubes[arrayX + 1, arrayY]);
            }
        }
        //RIGHT
        if (PositionWithinArrayBounds(arrayX - 1, arrayY))
        {
            if (unvisitedCubes.Contains(levelCubes[arrayX - 1, arrayY]) || ignoreUnvisitedSet)
            {
                neighboursList.Add(levelCubes[arrayX - 1, arrayY]);
            }
        }
        //UP
        if (PositionWithinArrayBounds(arrayX, arrayY + 1))
        {
            if (unvisitedCubes.Contains(levelCubes[arrayX, arrayY + 1]) || ignoreUnvisitedSet)
            {
                neighboursList.Add(levelCubes[arrayX, arrayY + 1]);
            }
        }
        //DOWN
        if (PositionWithinArrayBounds(arrayX, arrayY - 1))
        {
            if (unvisitedCubes.Contains(levelCubes[arrayX, arrayY - 1]) || ignoreUnvisitedSet)
            {
                neighboursList.Add(levelCubes[arrayX, arrayY - 1]);
            }
        }

        //Return all the neighbours that we got
        return neighboursList;
    }

    /// <summary>
    /// Remove all Impassable cubes from the unvisited hashset
    /// </summary>
    private void InitImpassableCubes()
    {
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                //Check if the tile we are checking has it's data
                //value to be impassable
                if (levelData[x, y] == (int)LevelGenerator.TileType.TILE_IMPASSABLE) //Check that a fox is not already in a tile
                {
                    unvisitedCubes.Remove(levelCubes[x, y]);
                }
                else
                {
                    /*
                    if(levelCubes[x, y].GetComponentInChildren<WalkableTile>()?.Occipier?.GetComponent<FoxAI>())
                    {
                        unvisitedCubes.Remove(levelCubes[x, y]);
                    }*/
                }
            }
        }
    }

    /// <summary>
    /// Initalise all cubes in to a hashmap of unvisited cubes
    /// </summary>
    private void InitUnvisitedSet()
    {
        unvisitedCubes = new HashSet<GameObject>();

        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                unvisitedCubes.Add(levelCubes[x, y]);
            }
        }
    }

    /// <summary>
    /// Initalise the cube distances array by setting
    /// all distances to Positive Infinify
    /// </summary>
    private void InitCubeDistances()
    {
        cubeDistances = new float[levelSize.x, levelSize.y];

        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                //Set all distances that we haven't calculated yet to infinity
                cubeDistances[x, y] = float.PositiveInfinity;
            }
        }
    }

    /// <summary>
    /// Checks if the start and end positions are within the bounds
    /// of the cube array
    /// </summary>
    /// <returns>If Positions are within array bounds</returns>
    private bool PositionsWithinArrayBounds(Vector2Int startPosInArray, Vector2Int endPosInArray)
    {
        return (PositionWithinArrayBounds(startPosInArray.x, startPosInArray.y) 
            && PositionWithinArrayBounds(endPosInArray.x, endPosInArray.y));
    }

    /// <summary>
    /// Checks if a given position is within the bounds of the level cubes
    /// array
    /// </summary>
    /// <returns>If Position is within array bounds</returns>
    private bool PositionWithinArrayBounds(int x, int y)
    {
        //Check that positions are not less than 0
        if (x < 0 || y < 0)
        {
            return false;
        }

        //Check that values are not greater than array size
        if (x >= levelCubes.GetLength(0) || y >= levelCubes.GetLength(1)){
            return false;
        }

        //Both checks passed return true
        //as we are within bounds
        return true;
    }


    /// <summary>
    /// Gets the position of a cube within the cubes/distance array
    /// Always good to check this position is valid with PositionsWithinArrayBounds()
    /// </summary>
    /// <param name="cube">Cube to get the position of</param>
    /// <returns>Cube Position with array</returns>
    private Vector2Int GetCubePosInArray(GameObject cube)
    {
        return GetWorldPosInArray(new Vector2(cube.transform.position.x, cube.transform.position.z));
    }

    /// <summary>
    /// Gets the position of a world position with the cubes/distance array
    /// Always good to check this position is valid with PositionsWithinArrayBounds()
    /// </summary>
    /// <param name="pos">Position to get world position of</param>
    /// <returns></returns>
    private Vector2Int GetWorldPosInArray(Vector2 pos)
    {
        //Take its world position away from the level start postion to get its 
        //pos within the array
        int xPosInArray = (int)pos.x - (int)levelStartPos.x;
        int yPosInArray = (int)pos.y - (int)levelStartPos.z;

        return new Vector2Int(xPosInArray, yPosInArray);
    }

}
