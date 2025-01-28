using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Side currentSide = Side.Mid;
    [SerializeField] private bool SwipeLeft;
    [SerializeField] private bool SwipeRight;

    Rigidbody2D rb;

    float newPos = 0f;

    
    void Start()
    {
        rb.velocity = Vector3.zero;

    }

    void Update()
    {
        SwipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    }
}

public enum Side { Left, Mid, Right }
