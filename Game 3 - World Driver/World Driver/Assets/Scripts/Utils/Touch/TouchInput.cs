using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.TouchExtensions
{
    /// <summary>
    /// Class that extends the inbuit unity touch inputs with a number of 
    /// helper functions
    /// </summary>
    public static class TouchInput
    {
        public enum SwipeAxis
        {
            X,
            Y
        }

        /// <summary>
        /// Checks that the touch that we are trying to get is valid (i.e that
        /// the touch id we are trying to get actaully exists)
        /// </summary>
        /// <param name="touchID">ID of Touch to check</param>
        /// <returns>If touch is valid</returns>
        public static bool TouchIsValid(int touchID)
        {
            //Check that our touch count is greater than the touch that we 
            //are trying to do
            if (Input.touchCount > touchID)
            {
                //Check touch was not cancled
                if(Input.GetTouch(touchID).phase != TouchPhase.Canceled)
                {
                    return true;
                }
            }

            //Invalid Touch
            return false;
        }

        /// <summary>
        /// Gets the delta movement of a touch (i.e the movement * delta time)
        /// </summary>
        /// <param name="touch">Touch to get movement of</param>
        /// <returns>Movement since last touch update</returns>
        public static Vector2 GetDeltaTouchMovement(Touch touch)
        {
            return touch.deltaPosition * touch.deltaTime;
        }

        /// <summary>
        /// Gets the the amount of movement in a given direction specificed by the given swipe axis
        /// (either X or Y)
        /// Inputs 
        /// </summary>
        /// <param name="touch">Touch to get movement from</param>
        /// <param name="direction">Direction to return movement from</param>
        /// <returns></returns>
        public static float GetTouchMovementInDirection(Touch touch , SwipeAxis direction)
        {
            switch (direction)
            {
                case SwipeAxis.X:
                    return GetDeltaTouchMovement(touch).x;
                case SwipeAxis.Y:
                    return GetDeltaTouchMovement(touch).y;
                default:
                    //Invalid - return 0 movement
                    return 0.0f;
            }
        }

    }
}
