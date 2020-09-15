using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lewis.MathUtils;

namespace Lewis.TouchExtensions
{
    /// <summary>
    /// Class that extends the inbuit unity touch inputs with a number of 
    /// helper functions
    /// </summary>
    public static class TouchInput
    {

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
        /// Gets the main direction of a touch, this is returned as a vector 2 of only the 
        /// main swipe direction (i.e a mainly left swipe will return (-1,0))
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMainSwipeDirection(Touch touch)
        {
            //Get the abs of the touch movement so we know which direction has had
            //the most movement
            Vector2 touchMovement = touch.deltaPosition;
            Vector2 absTouchMovement = MathUtils.MathUtils.Abs(touchMovement);

            if(absTouchMovement.x > absTouchMovement.y)
            {
                //X > Y work out which direction in the Y we have dragged
                return touchMovement.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
            }
            else if(absTouchMovement.y > absTouchMovement.x)
            {
                //Y > X work out which direction in the Y we have dragged
                return touchMovement.y > 0 ? new Vector2(0, 1) : new Vector2(0, -1);
            }
            else
            {
                //Both values are 0 return empty vector
                return new Vector2(0, 0);
            }

        }

    }
}
