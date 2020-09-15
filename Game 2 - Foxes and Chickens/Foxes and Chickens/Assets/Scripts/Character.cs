using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class that is used for shared functions between the
/// AI and Player Characters
/// </summary>
public abstract class Character : MonoBehaviour
{
    //Event for when the characters turn ends
    public delegate void CharacterTurnEvent(GameStateManager.GameTurn endingTurn);
    public static event CharacterTurnEvent EndTurn;


    //Amount of time for a turn from starting
    //move to allowing the next turn to start
    protected static float turnDuration = 0.4f; /*(seconds)*/

    //Bool for if we have made a move and are just wating for our movements to
    //process
    protected bool TurnInteractionEnded { get; private set; } = false;

    /// <summary>
    /// End a turn after a number of seconds has passed
    /// </summary>
    /// <param name="turnToEnd">Turn To End (Player or AI)</param>
    /// <param name="time">Time (seconds) to wait</param>
    /// <returns></returns>
    protected IEnumerator EndTurnAfterTime(GameStateManager.GameTurn turnToEnd, float time)
    {
        //Wait for time
        TurnInteractionEnded = true;
        yield return new WaitForSecondsRealtime(time);

        //End Turn
        EndTurn?.Invoke(turnToEnd);
        TurnInteractionEnded = false;
    }

    /// <summary>
    /// Move to a position over time
    /// </summary>
    /// <param name="target">Target Position</param>
    /// <param name="time">Time to Move</param>
    protected IEnumerator MoveOverTime(Vector3 target, float time)
    {
        //Record our start pos so we can lerp
        Vector3 startPos = transform.position;

        //Setup the amount of time it has already taken
        //for us to move
        float elapsedTime = 0;
        float ratio = elapsedTime / time;

        //Move at a speed that means we will be at our target destination
        //when our time is up
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / time;
            transform.position = Vector3.Lerp(startPos, target, ratio);
            yield return null;
        }
    }
}
