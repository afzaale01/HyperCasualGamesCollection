using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : GameMenu
{
    public void ResumeGame()
    {
        GameStateManager.ResumeGame();
    }
}
