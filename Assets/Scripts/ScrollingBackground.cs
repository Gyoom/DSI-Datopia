using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField]
    private float backgroundSpeed = 0.1f;
    [SerializeField]
    private SpriteRenderer backgroundRenderer;

    Vector3 startPosition;
    float spriteSize;


    void Start()
    {
        startPosition = transform.position;
        spriteSize  = backgroundRenderer.sprite.bounds.size.y * backgroundRenderer.transform.localScale.y;
    }

    void Update()
    {
           float newPos = Mathf.Repeat(Time.time * backgroundSpeed, spriteSize);
            transform.position = startPosition + Vector3.down * newPos;
    }
}
