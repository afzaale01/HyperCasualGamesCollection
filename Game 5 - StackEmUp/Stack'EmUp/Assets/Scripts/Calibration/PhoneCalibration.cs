using System.Collections;
using System.Collections.Generic;
using Lewis.TouchExtensions;
using UnityEngine;

/// <summary>
/// Class for calibrating the phones gyro
/// Passes the phone input its offset values to use when
/// detecting input
/// </summary>
public class PhoneCalibration : MonoBehaviour
{

    //Store the current calibration value
    public Quaternion lastCalibrationValues;

    /// <summary>
    /// Calibrates the gyroscope. Taking the current values and giving
    /// them to the phone input manager to use as offsets
    /// Called from button on Calibrate UI
    /// </summary>
    public void CalibrateGyro()
    {
        //Get current phone rotation and store it so we can apply it i
        //if the player chooses
        lastCalibrationValues = Quaternion.Euler(PhoneInput.GetDeviceRotation());
    }
}
