using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for particles that destory after they are finished
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticles : MonoBehaviour
{
    //Particle System to Destroy after it is done
    private ParticleSystem partSys;

    // Start is called before the first frame update
    void Awake()
    {
        //Get the particle system
        partSys = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        //Check for the end of particle system
        if (!partSys.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
