using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelButton : MonoBehaviour
{
    /// <summary>
    /// Trigger the game restart
    /// Callled by button press assignment in the inspector
    /// </summary>
    public void TriggerRestartLevel()
    {
        GameStateManager.RestartGame();
    }
}
