using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float xCenterValue = 0f;
    [Header("SideMode")]
    [SerializeField] private Side currentSide = Side.Mid;
    [SerializeField] private float sideMoveOffset = 5f;
    [SerializeField] private float sideMoveDelay = 0.2f;
    [SerializeField] private bool inMove;

    [Header("JunctionMove")]
    [SerializeField] private ScrollingRiver bgScrolling;
    [SerializeField] private float junctionMoveDelay = 0.5f;

    float newPos = 0f;

    void Update()
    {
        bool doMove = false;

        if (!inMove && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))) {
            if (currentSide == Side.Mid)
            {
                newPos = xCenterValue - sideMoveOffset;
                inMove = true;
                doMove = true;
                currentSide = Side.Left;
            }
            else if (currentSide == Side.Right)
            {
                newPos = xCenterValue;
                inMove = true;
                doMove = true;
                currentSide = Side.Mid;
            }
        }
        if (!inMove && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))) {
            if (currentSide == Side.Mid)
            {
                newPos = xCenterValue + sideMoveOffset;
                inMove = true;
                doMove = true;
                currentSide = Side.Right;
            }
            else if (currentSide == Side.Left)
            {
                newPos = xCenterValue;
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

    public void JunctionMove() {
        
        float offSet = bgScrolling.xValueOffset;

        switch (currentSide) { 
            case Side.Left:
                transform.DOMoveX(transform.position.x - offSet, junctionMoveDelay);
                bgScrolling.xValue -= offSet;
                xCenterValue -= offSet;
                bgScrolling.junctionPending = false;
                break;
            case Side.Mid:
                StartCoroutine(bgScrolling.ComeBack());
                break;
            case Side.Right:
                transform.DOMoveX(transform.position.x + offSet, junctionMoveDelay);
                bgScrolling.xValue += offSet;
                xCenterValue += offSet;
                bgScrolling.junctionPending = false;
                break;
            default:
                break;
        }
    }
}

public enum Side { Left, Mid, Right }
