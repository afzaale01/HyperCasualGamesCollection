using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.MathUtils
{
    /// <summary>
    /// Custom Math Utility Class
    /// </summary>
    public static class MathUtils
    {

        /// <summary>
        /// Clamps a vector 3 between 2 other vector 3s
        /// </summary>
        /// <param name="value">Value to Clamp</param>
        /// <param name="min">Min X/Y/Z Values</param>
        /// <param name="max">Max X/Y/Z Values</param>
        /// <returns></returns>
        public static Vector2 ClampVec3(Vector2 value, Vector2 min, Vector2 max)
        {
            float x = Mathf.Clamp(value.x, min.x, max.x);
            float y = Mathf.Clamp(value.y, min.y, max.y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Clamps a vector 3 between 2 other vector 3s
        /// </summary>
        /// <param name="value">Value to Clamp</param>
        /// <param name="min">Min X/Y/Z Values</param>
        /// <param name="max">Max X/Y/Z Values</param>
        /// <returns></returns>
        public static Vector3 ClampVec3(Vector3 value, Vector3 min, Vector3 max)
        {
            float x = Mathf.Clamp(value.x, min.x, max.x);
            float y = Mathf.Clamp(value.y, min.y, max.y);
            float z = Mathf.Clamp(value.z, min.z, max.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Get the absolute values of a vector 2
        /// </summary>
        /// <param name="value">Value to Abs</param>
        public static Vector2 Abs(Vector2 value){
            float absX = Mathf.Abs(value.x);
            float absY = Mathf.Abs(value.y);

            return new Vector2(absX, absY);
        }

        /// <summary>
        /// Get the absolute values of a vector 3
        /// </summary>
        /// <param name="value">Value to Abs</param>
        public static Vector3 Abs(Vector3 value)
        {
            float absX = Mathf.Abs(value.x);
            float absY = Mathf.Abs(value.y);
            float absZ = Mathf.Abs(value.z);

            return new Vector3(absX, absY, absZ);
        }

        /// <summary>
        /// Get the max value from a vector 3
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float MaxVecValue(Vector3 value)
        {
            return Mathf.Max(value.x, value.y, value.z);
        }

    }
}
