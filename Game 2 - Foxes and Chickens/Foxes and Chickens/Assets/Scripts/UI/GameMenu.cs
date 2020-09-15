using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Menu - shared functions for all game menus
/// </summary>
public class GameMenu : Menu
{
    /// <summary>
    /// Restart the level
    /// </summary>
    public void RestartLevel()
    {
        //Call Game state manager to trigger game restart
        GameStateManager.RestartGame();
    }
}
