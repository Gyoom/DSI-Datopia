using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("SideMode")]
    [SerializeField] private Side currentSide = Side.Mid;
    [SerializeField] private float sideMoveValue = 10f;
    [SerializeField] private float sideMoveDelay = 0.2f;
    [SerializeField] private bool inMove;
    Rigidbody2D rb;

    float newPos = 0f;

    void Update()
    {
        bool doMove = false;

        if (!inMove && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))) {
            if (currentSide == Side.Mid)
            {
                newPos = -sideMoveValue;
                inMove = true;
                doMove = true;
                currentSide = Side.Left;
            }
            else if (currentSide == Side.Right)
            {
                newPos = 0f;
                inMove = true;
                doMove = true;
                currentSide = Side.Mid;
            }
        }
        if (!inMove && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))) {
            if (currentSide == Side.Mid)
            {
                newPos = sideMoveValue;
                inMove = true;
                doMove = true;
                currentSide = Side.Right;
            }
            else if (currentSide == Side.Left)
            {
                newPos = 0f;
                inMove = true;
                doMove = true;
                currentSide = Side.Mid;
            }
        }

        if (doMove) {
            Vector3 dest = transform.position + Vector3.zero;
            dest.x = newPos;
            transform.DOMove(dest, sideMoveDelay);
            StartCoroutine(CanMoveAgain());
        }
    }

    IEnumerator CanMoveAgain() { 
        yield return new WaitForSeconds(sideMoveDelay);
        inMove = false;
    }
}

public enum Side { Left, Mid, Right }
