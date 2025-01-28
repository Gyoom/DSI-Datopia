using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingRiver : MonoBehaviour
{
    [SerializeField]
    private float backgroundSpeed = 0.1f;
    [SerializeField]
    private MeshFilter riverRenderer;

    Vector3 startPosition;
    float spriteSize;


    void Start()
    {
        startPosition = transform.position;
        spriteSize = riverRenderer.mesh.bounds.size.z * riverRenderer.transform.localScale.z;
    }

    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * backgroundSpeed, spriteSize);
        transform.position = startPosition + Vector3.back * newPos;
    }
}
