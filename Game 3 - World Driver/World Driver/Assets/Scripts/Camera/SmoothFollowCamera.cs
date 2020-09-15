using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    //Target to follow
    [SerializeField]
    private Transform target;

    //Smoothnesses
    [SerializeField]
    private float followSmoothness, rotationSmoothness;

    //Offset from the target to follow at
    [SerializeField]
    private Vector3 followOffset;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    private void Start()
    {
        //Teleport to start position
        transform.position = target.TransformDirection(followOffset);
        transform.rotation = Quaternion.LookRotation(-transform.position.normalized, target.up);
    }

    // Update is called once per frame
    private void Update()
    {
        //Null check target, if we have
        //no target then we have nothing to follow
        if(target == null)
        {
            return;
        }

        //Calculate new target position and smooth to that position
        Vector3 targetPos = target.TransformDirection(followOffset);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothness);

        //Calculate new rotation and smooth to that rotation
        Quaternion targetRot = Quaternion.LookRotation(-transform.position.normalized, target.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothness);
    }
}
