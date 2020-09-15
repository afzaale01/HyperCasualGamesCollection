using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelButton : MonoBehaviour
{
    /// <summary>
    /// Restart the Game
    /// </summary>
    public void RestartGame()
    {
        //Call the game state manager restart
        GameStateManager.RestartGame();
    }

}
