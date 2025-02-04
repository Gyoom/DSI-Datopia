using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 posOffset = new Vector3(0, 29, -25);

    [SerializeField] private float timeLerpPos = 0.1f;

    private Vector3 currentVelocity;

    private void Start()
    {
        if (player)
        {
            // all directions camera follow
            Vector3 target = player.transform.position;
            target.z += posOffset.z;
            target.y += posOffset.y;
            target.x += posOffset.x;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, timeLerpPos);

        }
    }

    void LateUpdate()
    {
        if (player)
        {
            // all directions camera follow
            Vector3 target = player.transform.position;
            target.z += posOffset.z;
            target.y = transform.position.y;
            target.x += posOffset.x;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, timeLerpPos);

        }
    }
}
