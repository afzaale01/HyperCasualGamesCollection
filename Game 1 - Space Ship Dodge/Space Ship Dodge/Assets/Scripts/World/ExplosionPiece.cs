using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPiece : MonoBehaviour
{
    //Events so that we can add explosion piece to the world mover
    public delegate void ExplosionPieceEvent(GameObject obj);
    public static event ExplosionPieceEvent PieceCreated;
    public static event ExplosionPieceEvent PieceDestroyed;

    //Time before this obj is destoryed
    private float lifeTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Register created obj
        PieceCreated?.Invoke(gameObject);
        Destroy(gameObject, lifeTime);
    }

    private void OnDestroy()
    {
        //Register destroyed obj
        PieceDestroyed?.Invoke(gameObject);
    }
}
