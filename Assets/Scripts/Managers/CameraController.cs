using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 posOffset = new Vector3(0, 29, -25);
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 currentVelocity = Vector3.zero;

    private void Start()
    {
        if (target)
        {
            // all directions camera follow
            Vector3 newPos = target.position + posOffset;
            transform.position = newPos;

        }
    }

    void FixedUpdate()
    {
        if (target)
        {
            // all directions camera follow
            Vector3 newPos = target.position + posOffset;
            newPos.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, newPos, smoothTime * Time.deltaTime);
        }
    }
}
