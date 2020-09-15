using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Menu
{
    /// <summary>
    /// Resume the Game
    /// </summary>
    public void ResumeGame()
    {
        GameStateManager.ResumeGame();
    }
}
