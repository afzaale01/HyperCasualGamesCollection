using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceLightsController : MonoBehaviour
{
    //Light Groups are groups of lights that should be on or off together
    [SerializeField]
    private List<LightGroup> lightGroups;

    [Header("Light Timing")]
    [SerializeField]
    private float lightSwitchTime = 2f; //Time to wait before switching the active light group
    private float currentLightTime; //Time since we last changed light group

    private int activeLightGroupIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Init active group and current time lights have been active
        activeLightGroupIndex = 0;
        currentLightTime = 0f;

        //Set the 1st light group active
        UpdateActiveLightGroup();

    }

    // Update is called once per frame
    void Update()
    {
        currentLightTime += Time.deltaTime;

        //Switch active light group after the given time
        if (currentLightTime >= lightSwitchTime)
        {
            if (++activeLightGroupIndex >= lightGroups.Count)
            {
                activeLightGroupIndex = 0;
            }

            UpdateActiveLightGroup();

            currentLightTime = 0f;
        }
    }

    /// <summary>
    /// Updates the active light group based on the active light group index
    /// </summary>
    private void UpdateActiveLightGroup()
    {
        //Set the current light group to be active, all others to be
        //inactive
        for (int i = 0; i < lightGroups.Count; i++)
        {
            if (i == activeLightGroupIndex)
            {
                lightGroups[i].SetGroupActive(true);
            }
            else
            {
                lightGroups[i].SetGroupActive(false);
            }
        }
    }

    /// <summary>
    /// Class to store all a group of lights and toggle their status
    /// </summary>
    [System.Serializable]
    public class LightGroup
    {
        [SerializeField]
        private List<Light> lights;

        /// <summary>
        /// Sets all of the lights in this group either active
        /// or inactive based on the flag set
        /// </summary>
        /// <param name="shouldBeActive">If lights should be active</param>
        public void SetGroupActive(bool shouldBeActive)
        {
            foreach (Light currentLight in lights)
            {
                currentLight.enabled = shouldBeActive;
            }
        }
    }

}
