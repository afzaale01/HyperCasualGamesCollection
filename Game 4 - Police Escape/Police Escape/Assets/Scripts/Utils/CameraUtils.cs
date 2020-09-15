using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.Camera
{
    public static class CameraUtils
    {
        /// <summary>
        /// Gets if a point is within the cameras view frustum
        /// </summary>
        /// <param name="point">Point to Check</param>
        /// <param name="cam">Camera to check</param>
        public static bool IsPointWithinCamView(Vector3 point, UnityEngine.Camera cam)
        {
            //Convert the world point to screen point if the point is not >0 and <1 then
            //it is outside the cameras view
            if (cam == null)
            {
                return false;
            }

            Vector3 camViewportPos = cam.WorldToViewportPoint(point);
            if (camViewportPos.x < 0 || camViewportPos.x > 1 ||
                camViewportPos.y < 0 || camViewportPos.y > 1 ||
                camViewportPos.z < 0)
            {
                return false;
            }

            //Do Raycast
            Vector3 directionToPoint = (point - cam.gameObject.transform.position).normalized;
            if (!Physics.Raycast(cam.gameObject.transform.position, directionToPoint))
            {
                return false;
            }

            //Point is view
            return true;
        }
    }
}
