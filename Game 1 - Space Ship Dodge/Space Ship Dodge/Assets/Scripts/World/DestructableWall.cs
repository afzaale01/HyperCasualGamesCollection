using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableWall : MonoBehaviour
{
    //Settings for the explosion of this wall
    [Min(1f)]
    protected float explosionCubeSize = 1;
    protected Vector3 explosionSpeed = new Vector3(0,0,0);

    //All of the colliders this object has
    protected List<Collider> wallColliders;

    //Physics Layer that the exploded pieces should be
    //on so they don't collide with themselves
    private int explosionCollisionLayer = 10;

    //Explosion Audio
    [Header("Audio")] 
    [SerializeField] private SFXAudioSource explosionSource;
    [SerializeField] private AudioClip explosionSound;

    private void Awake()
    { 
        //Add Audio Soruce. we need a global player because
        if (explosionSource == null)
        {
            explosionSource = gameObject.AddComponent<SFXAudioSource>();
        }
    }

    public void Start()
    {
        //Get all of the colliders for this obj and possible colliders in children
        wallColliders = new List<Collider>();
        wallColliders.AddRange(GetComponentsInChildren<Collider>());
    }

    /// <summary>
    /// Explode this wall in to sub cubes
    /// </summary>
    public void ExplodeWall()
    {
        if (wallColliders != null)
        {
            //Create cubes within each collider
            foreach (Collider collider in wallColliders)
            {
                //Get the colliders size
                Vector3 colliderSize = collider.bounds.size;

                //Work out the top left of the cube
                Vector3 cubeOrigin = transform.position - (colliderSize / 2);

                for (float x = 0; x < colliderSize.x; x += explosionCubeSize)
                {
                    for (float y = 0; y < colliderSize.y; y += explosionCubeSize)
                    {
                        for (float z = 0; z < colliderSize.z; z += explosionCubeSize)
                        {
                            GameObject piece = CreateExplosionPiece(cubeOrigin + new Vector3(x, y, z), Vector3.one);
                            piece.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(5, 20), 0, 0);
                            //Set to have the same material as the wall
                            piece.GetComponent<Renderer>().material = gameObject.GetComponent<Renderer>().material;
                        }
                    }
                }

                //Play Explosion Sound
                if (explosionSource && explosionSound)
                {
                    explosionSource.PlaySFX(explosionSound);
                }

                //Destroy Collider and renderer
                GetComponent<Collider>().enabled = false;
                GetComponent<Renderer>().enabled = false;
                //Set to Destroy object after audio has played
                Destroy(gameObject, explosionSound.length);
            }
        }
 
    }

    /// <summary>
    /// Create a piece of of the explosion
    /// </summary>
    /// <param name="localPos">Local Position to create peice at</param>
    /// <param name="scale">Scale of te piece</param>
    private GameObject CreateExplosionPiece(Vector3 localPos, Vector3 scale)
    {
        GameObject cube;
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //Set pos, size and rotation
        cube.transform.localScale = scale;
        cube.transform.position = localPos;
        cube.transform.rotation = transform.rotation;
        cube.transform.parent = transform.parent;

        //Add components
        cube.AddComponent<Rigidbody>();
        cube.AddComponent<ExplosionPiece>();

        //Set layer so that it doesn't collide with other
        //explosion pieces
        cube.layer = explosionCollisionLayer;

        return cube;

    }

    /// <summary>
    /// Revert the wall to it's original state
    /// </summary>
    public void ReconstructWall()
    {

    }
}
