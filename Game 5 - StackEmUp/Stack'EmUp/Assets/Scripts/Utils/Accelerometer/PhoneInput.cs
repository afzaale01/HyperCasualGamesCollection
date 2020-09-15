using System.Collections;
using System.Collections.Generic;
using Lewis.GameOptions;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Lewis.TouchExtensions
{

    /// <summary>
    /// Class to deal with phone inputs that are not the touchscreen
    /// </summary>
    public static class PhoneInput
    {

        //Static Constructor to subscribe to event
        static PhoneInput()
        {
            //Subscribe to event to update gyro offset 
            GameOptionsManager.GameOptionsUpdated += SetDeviceGyroOffset;

            //Enable gyro
            Input.gyro.enabled = true;

            //Update the gyro offset on creation
            SetDeviceGyroOffset();
        }

        //Offset from our pecived rotation to the real rotation of the phone
        private static Quaternion phoneRotationOffset = Quaternion.identity;

        /// <summary>
        /// Sets the phones gyro offset. Used for calibration
        /// of the device so we detect up as the same
        /// as the players phone
        /// </summary>
        public static void SetDeviceGyroOffset()
        {
            phoneRotationOffset =  GyroToUnity( Quaternion.Euler(GameOptionsManager.GetCurrentGameOptions().phoneRotOffset));
            Debug.Log("Set Phone Rotation:" + phoneRotationOffset.eulerAngles);
        }

        /// <summary>
        /// Gets the device rotation the in X/Y/Z axies in 
        /// </summary>
        /// <returns>Devices current rotation</returns>
        public static Vector3 GetDeviceRotation()
        {
            //Check we have a gyro
            if (Input.gyro == null)
            {
                return Vector3.zero;
            }

            Vector3 rawRotation = Input.gyro.attitude.eulerAngles;
            Debug.Log("Get Rotation: " + rawRotation + "\nWith Offset: " + (rawRotation - phoneRotationOffset.eulerAngles));


            //Get the rotation and convert to a Unity format
            return rawRotation - phoneRotationOffset.eulerAngles;
        }

        /// <summary>
        /// Gets the devices rotation rate, which is the speed of rotation in each axis in the last frame
        /// </summary>
        /// <returns>Rate of Phone rotation in the X/Y/Z axis in degrees</returns>
        public static Vector3 GetDeviceRotationRate()
        {
            //Check we have a gyro
            if (Input.gyro == null)
            {
                return Vector3.zero;
            }
            //Make sure that gyro is enabled
            Input.gyro.enabled = true;

            return Input.gyro.rotationRate * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Converts the Rotation from the gyro to a Unity format
        /// </summary>
        /// <param name="rotation">Rotation to convert</param>
        /// <returns></returns>
        private static Quaternion GyroToUnity(Quaternion rotation)
        {
            return new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w);
        }
    }
}