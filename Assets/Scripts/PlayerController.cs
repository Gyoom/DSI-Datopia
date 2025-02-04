using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEditor;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private float fingerDelay = 0.1f;
   
    [Header("SideMode")]
    [SerializeField] private Way currentSide = Way.Mid;
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
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private Transform splashPos;

    [Header("Trail")]
    [SerializeField] private bool trailActive;
    [SerializeField] private Transform trails;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private float trailInstantiateDelay = 0.1f;
    private float trailInstantiateTimer;
    [SerializeField] private float trailDuration = 1f;

    [Header("Trail")]
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private float impactScale;


    RaycastHit[] fingerHits;
    private bool fingerActive;
    private float fingerTimer = 0f;
    private Vector2 MouseStartPos;
    private Vector2 MouseEndPos;

    private LayerMask obstacleLayer;

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Obstacles");
    }

    void Update()
    {
        // Inputs ---------------------------------------------------------------------------------------------
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

        // Trail --------------------------------------------------------------------------------------

        // new trail
        if (trailActive)
        {
            trailInstantiateTimer += Time.deltaTime;

            if (trailInstantiateTimer >= trailInstantiateDelay)
            {
                StartCoroutine(TrailLife());
                trailInstantiateTimer = 0;
            }
        }
    }

    private IEnumerator TrailLife() {
        GameObject trail = Instantiate(trailPrefab, trails.position, Quaternion.identity, ScrollingManager.instance.gameObject.transform.GetChild(0));
        float time = 0;
        float localDuration = trailDuration;
        Vector3 newScale = trail.transform.localScale;

        while (time < trailDuration)
        {
            if (!trail.gameObject)
                break;

            time += Time.deltaTime;

            float newScaleValue = Mathf.Lerp(1f, 2f, time / trailDuration);
            newScale.x = newScaleValue;
            newScale.z = newScaleValue;

            trail.transform.localScale = newScale;

            yield return null;
        }

        if (trail.gameObject)
            Destroy(trail);
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
                    case "SwipableObstacle":
                        find = true;
    
                        Vector2 SwipeDir = GetSwipDir();
                        if (SwipeDir.x > 0f)
                            hit.transform.parent.GetComponent<SwipableObstacle>().WaysMove(true);

                        if (SwipeDir.x < 0f)
                            hit.transform.parent.GetComponent<SwipableObstacle>().WaysMove(false);

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
            if (currentSide == Way.Mid)
            {
                newPos = xCenterValue - sideMoveOffset;
                inMove = true;
                doMove = true;
                currentSide = Way.Left;
            }
            else if (currentSide == Way.Right)
            {
                newPos = xCenterValue;
                inMove = true;
                doMove = true;
                currentSide = Way.Mid;
            }
        }
        if (!inMove && !left)
        {
            if (currentSide == Way.Mid)
            {
                newPos = xCenterValue + sideMoveOffset;
                inMove = true;
                doMove = true;
                currentSide = Way.Right;
            }
            else if (currentSide == Way.Left)
            {
                newPos = xCenterValue;
                inMove = true;
                doMove = true;
                currentSide = Way.Mid;
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
        if (inMove)
            yield break;

        inMove = true;
        trailActive = false;

        // est ce qu'on peut changer de voies en saut ?
        transform.DOMoveY(transform.position.y + jumpStrength, jumpDelay).SetEase(jumpCurve);
        yield return new WaitForSeconds(jumpDelay);
        GameObject splash = Instantiate(splashPrefab, splashPos.position, Quaternion.identity, ScrollingManager.instance.gameObject.transform.GetChild(0));
        trailActive = true;
        inMove = false;
        yield return new WaitForSeconds(splashPrefab.GetComponent<ParticleSystem>().main.duration);
        Destroy(splash);
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

    IEnumerator CanMoveAgain() { 
        yield return new WaitForSeconds(sideMoveDelay);
        inMove = false;
    }

    public void JunctionMove(JunctionPlayerDetect jpd) {
        
        float offSet = ScrollingManager.instance.xValueOffset;

        switch (currentSide) { 
            case Way.Left:
                Debug.Log("Left");
                float xDestLeft = transform.position.x - offSet + (xCenterValue - transform.position.x);
                //transform.DOMoveX(xDestLeft, junctionMoveDelay);
                /*GetComponent<SplineAnimate>().Container = jpd.SplineLeft;
                GetComponent<SplineAnimate>().Duration = junctionMoveDelay;
                GetComponent<SplineAnimate>().enabled = true;
                GetComponent <SplineAnimate>().Play();*/

                StartCoroutine(EndJunctionChoice(jpd.SplineLeft));

                ScrollingManager.instance.xValue -= offSet;
                xCenterValue -= offSet;
                ScrollingManager.instance.junctionPending = false;
                currentSide = Way.Mid;
                UIManager.Instance.EmptyJonctionText(-1);
                break;

            case Way.Mid:
                StartCoroutine(ScrollingManager.instance.ComeBack(-1f));
                break;

            case Way.Right:
                float xDestRight = transform.position.x + offSet + (xCenterValue - transform.position.x);
                transform.DOMoveX(xDestRight, junctionMoveDelay);
                StartCoroutine(EndJunctionChoice(jpd.SplineRight));

                ScrollingManager.instance.xValue += offSet;
                xCenterValue += offSet;
                ScrollingManager.instance.junctionPending = false;
                currentSide = Way.Mid;
                UIManager.Instance.EmptyJonctionText(1);
                break;

            default:
                break;
        }
    }

    private IEnumerator EndJunctionChoice(SplineContainer spline) {
        // Fade backaground
        float time = 0;

        Vector3 newPlayerPos = transform.position;
        Vector3 playerDir = transform.forward;
        Quaternion playerRot = Quaternion.identity;

        float progressValue = 0f;
        float Duration = junctionMoveDelay;


        var forward = Vector3.forward;
        var up = Vector3.up;


        while (time < Duration)
        {
            progressValue = time / Duration;
 
            // Pos
            newPlayerPos.x = spline.EvaluatePosition(progressValue).x;
            transform.position = newPlayerPos;

            // Dir
            playerDir = spline.EvaluateTangent(progressValue);
            if (Vector3.Magnitude(forward) <= Mathf.Epsilon)
            {
                if (progressValue < 1f)
                    forward = spline.EvaluateTangent(Mathf.Min(1f, progressValue + 0.01f));
                else
                    forward = spline.EvaluateTangent(progressValue - 0.01f);
            }
            forward.Normalize();


            // Rot
            playerRot = Quaternion.LookRotation(playerDir, up);
            transform.rotation = playerRot;

            time += Time.deltaTime;

            yield return null;
        }

        
        UIManager.Instance.UpdateJunctionText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.ToString());

        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0) {
            StartCoroutine(ImpactVfx(collision.contacts.First().point));
            StartCoroutine(ScrollingManager.instance.ComeBack(collision.gameObject.GetComponent<Obstacle>().hitRecoil));
        }
    }

    private IEnumerator ImpactVfx(Vector3 pos) {
        GameObject impact = Instantiate(impactPrefab, pos, Quaternion.identity, ScrollingManager.instance.gameObject.transform.GetChild(0));
        for (int i = 0; i < impact.transform.childCount; i++)
        {
            impact.transform.GetChild(i).localScale = new Vector3(impactScale, impactScale, impactScale);
        }

        yield return new WaitForSeconds(impactPrefab.GetComponent<ParticleSystem>().main.duration);
        Destroy(impact);
    }
}

public enum Way { Left, Mid, Right }
