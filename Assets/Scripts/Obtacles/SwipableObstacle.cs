using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwipableObstacle : Obstacle
{
    [SerializeField] private Way currentWay = Way.Mid;
    [SerializeField] private float sideMoveDelay = 0.5f;
    [SerializeField] private float sideMoveOffset = 7f;

    private LayerMask obstacleLayer;
    private bool inMove;
    private float newPos;

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Obstacles", "Player");
    }

    public void WaysMove(bool left)
    {
        bool doMove = false;
        newPos = transform.position.x;
        
        // check obstacle
        RaycastHit hit;
        Physics.Raycast(transform.position, left ? Vector3.left : Vector3.right, out hit, sideMoveOffset + 1f);
        if (hit.transform && obstacleLayer.value != hit.transform.gameObject.layer) {
            Debug.Log(hit.transform.ToString());
            return;
        }

        if (!inMove && left)
        {
            if (currentWay == Way.Mid)
            {
                newPos -= sideMoveOffset;
                inMove = true;
                doMove = true;
                currentWay = Way.Left;
            }
            else if (currentWay == Way.Right)
            {
                newPos -= sideMoveOffset;
                inMove = true;
                doMove = true;
                currentWay = Way.Mid;
            }
        }
        if (!inMove && !left)
        {
            if (currentWay == Way.Mid)
            {
                newPos += sideMoveOffset;
                inMove = true;
                doMove = true;
                currentWay = Way.Right;
            }
            else if (currentWay == Way.Left)
            {
                newPos += sideMoveOffset;
                inMove = true;
                doMove = true;
                currentWay = Way.Mid;
            }
        }

        if (doMove)
        {
            transform.DOMoveX(newPos, sideMoveDelay);
            StartCoroutine(CanMoveAgain());
        }
    }

    private IEnumerator CanMoveAgain() { 
        yield return new WaitForSeconds(sideMoveDelay);
        inMove = false;
    }
}
