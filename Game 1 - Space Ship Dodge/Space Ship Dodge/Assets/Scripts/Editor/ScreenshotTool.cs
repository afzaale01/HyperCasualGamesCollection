using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor Script that is used to create screenshots
/// </summary>
public class ScreenshotTool : MonoBehaviour
{
    //File type of our screenshots
    private const string screenshotFileType = ".png";

    /// <summary>
    /// Takes a screenshot of the playing game and saves it 
    /// </summary>
    [MenuItem("Game IO/Screenshots/Take screenshot")]
    private static void TakeAndSaveScreenshot()
    {
        //If screenshot directory does not exist then create it
        if (!Directory.Exists(GetScreenshotsDirectory()))
        {
            //Open directory in windows explorer
            CreateScreenshotsDirectory();
        }

        ScreenCapture.CaptureScreenshot(GetScreenshotsDirectory() + GetScreenshotName());
    }

    /// <summary>
    /// Gets the screenshot name for a new screenshot
    /// </summary>
    /// <returns>Screenshot Name</returns>
    private static string GetScreenshotName()
    {
        //Format the current date time in to a nice format
        DateTime currentDateTime = DateTime.Now;
        string screenShotDateString = currentDateTime.ToString("dd_mm_yyyy-HH_mm_ss");

        //Generate a screenshot name based off the current date and time
        return screenShotDateString + screenshotFileType;
    }

    /// <summary>
    /// Gets the screenshots directory 
    /// </summary>
    private static string GetScreenshotsDirectory()
    {
        return Directory.GetCurrentDirectory() + "/Screenshots/";
    }

    /// <summary>
    /// Create the screenshots directory
    /// </summary>
    private static void CreateScreenshotsDirectory()
    {
        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Screenshots/");
    }

    #if UNITY_EDITOR_WIN //Only on windows as mac/linux do not have explorer.exe

    /// <summary>
    /// Opens the screenshot file location in explorer
    /// </summary>
    [MenuItem("Game IO/Screenshots/Open Screenshots Path")]
    private static void OpenScreenshotDirectory()
    {
        //If screenshot directory does not exist then create it
        if (!Directory.Exists(GetScreenshotsDirectory()))
        {
            //Open directory in windows explorer
            CreateScreenshotsDirectory();
        }

        //Open the screenshots directory
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = GetScreenshotsDirectory(),
            UseShellExecute = true,
            Verb = "open"
        });

    }

    #endif

}
