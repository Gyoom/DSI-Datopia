using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private float fingerDelay = 0.1f;
   
    [Header("SideMode")]
    [SerializeField] private Side currentSide = Side.Mid;
    [SerializeField] private float sideMoveOffset = 5f;
    [SerializeField] private float sideMoveDelay = 0.2f;
    [SerializeField] private bool inMove;
    [SerializeField] private float xCenterValue = 0f;
    float newPos = 0f;

    [Header("JunctionMove")]
    [SerializeField] private float junctionMoveDelay = 0.5f;

    
    
    [Header("JumpMove")]
    [SerializeField] private float jumpStrength = 8f;
    [SerializeField] private float jumpDelay = 0.5f;
    [SerializeField] private AnimationCurve jumpCurve;


    RaycastHit[] fingerHits;
    private bool fingerActive;
    private float fingerTimer = 0f;
    private Vector2 MouseStartPos;
    private Vector2 MouseEndPos;



    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            fingerTimer = 0;
            MouseStartPos = Input.mousePosition;
            //Array.Clear(fingerHits, 0, fingerHits.Length);

            var v3 = Input.mousePosition;
            v3.z = 1f;
            Vector3 dir = (Camera.main.ScreenToWorldPoint(v3) - Camera.main.transform.position).normalized;
            fingerHits = Physics.RaycastAll(Camera.main.transform.position, dir, 1000f);
            
            fingerActive = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            MouseEndPos = Input.mousePosition;
            // Swipe
            if (fingerTimer > fingerDelay)
            {
                SwipeInputs();
            }
            // Click
            else {
  
                ClickInputs();
            }
            fingerTimer = 0f;
            fingerActive = false;
        }

        if (fingerActive) {
            fingerTimer += Time.deltaTime;
        }
    }

    private void SwipeInputs()
    {
        bool find = false;
        foreach (RaycastHit hit in fingerHits)
        {
            if (hit.transform)
            {
                switch (hit.transform.gameObject.tag)
                {
                    case "Swipable":
                        find = true;
                        break;
                    default:
                        break;
                }
            }
        }

        // player swip
        if (!find) {
            Vector2 SwipeDir = GetSwipDir();
   

            if (SwipeDir.x > 0f)
                WaysInputs(true);
            else if (SwipeDir.x < 0f)
                WaysInputs(false);
            else if (SwipeDir.y > 0f)
                StartCoroutine(JumpMove());
        }
    }

    private Vector2 GetSwipDir()
    {
        Vector2 swipDir = Vector2.zero;

        float xVal = MouseEndPos.x - MouseStartPos.x;
        float yVal = MouseEndPos.y - MouseStartPos.y;

        if (Mathf.Abs(xVal) > Mathf.Abs(yVal))
            swipDir.x = Mathf.Sign(MouseStartPos.x - MouseEndPos.x);
        else
            swipDir.y = Mathf.Sign(MouseEndPos.y - MouseStartPos.y);

        return swipDir;
    }

    private void WaysInputs(bool left)
    {
        bool doMove = false;

        if (!inMove && left)
        {
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
        if (!inMove && !left)
        {
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

        if (doMove)
        {
            Vector3 dest = transform.position + Vector3.zero;
            dest.x = newPos;
            transform.DOMove(dest, sideMoveDelay);
            StartCoroutine(CanMoveAgain());
        }

    }

    private IEnumerator JumpMove()
    {
        Debug.Log("Jump");
        if (inMove)
            yield break;

        Debug.Log("Jump");

        // est ce qu'on peut changer de voies en saut ?
        transform.DOMoveY(transform.position.y + jumpStrength, jumpDelay).SetEase(jumpCurve);
        yield return new WaitForSeconds(jumpDelay);
    }

    private void ClickInputs() {
        foreach (RaycastHit hit in fingerHits)
        {
            if (hit.transform)
            {
                switch (hit.transform.gameObject.tag)
                {

                    case "BreakableObstacle":
                        Destroy(hit.transform.parent.gameObject);
                        break;

                    default:
                        break;
                }
            }
        }         
    }

    /*void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GizmosPos, 1);
    }*/


    IEnumerator CanMoveAgain() { 
        yield return new WaitForSeconds(sideMoveDelay);
        inMove = false;
    }

    public void JunctionMove() {
        
        float offSet = ScrollingManager.instance.xValueOffset;

        switch (currentSide) { 
            case Side.Left:
                float xDestLeft = transform.position.x - offSet + (xCenterValue - transform.position.x);
                transform.DOMoveX(xDestLeft, junctionMoveDelay);
                ScrollingManager.instance.xValue -= offSet;
                xCenterValue -= offSet;
                ScrollingManager.instance.junctionPending = false;
                currentSide = Side.Mid;
                UIManager.Instance.EmptyJonctionText();
                break;

            case Side.Mid:
                StartCoroutine(ScrollingManager.instance.ComeBack());
                break;

            case Side.Right:
                float xDestRight = transform.position.x + offSet + (xCenterValue - transform.position.x);
                transform.DOMoveX(xDestRight, junctionMoveDelay);
                ScrollingManager.instance.xValue += offSet;
                xCenterValue += offSet;
                ScrollingManager.instance.junctionPending = false;
                currentSide = Side.Mid;
                UIManager.Instance.EmptyJonctionText();
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Aie");
        if (collision.gameObject.CompareTag("Obstacle")) {
            StartCoroutine(ScrollingManager.instance.ComeBack());
        }
    }
}

public enum Side { Left, Mid, Right }
