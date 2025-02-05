using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Splines;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [SerializeField] private GameObject doubleBlockPrefab;
    [Header("Inputs")]
    [SerializeField] private float fingerDelay = 0.1f;
    [SerializeField] private float fingerSwipDistMin = 20f;

    [Header("SideMode")]
    [SerializeField] private Way currentSide = Way.Mid;
    [SerializeField] private float sideMoveOffset = 5f;
    [SerializeField] private float sideMoveDelay = 0.2f;
    public bool inMove;
    [SerializeField] private float xCenterValue = 0f;
    float newPos = 0f;

    [Header("JunctionMove")]
    [SerializeField] private float junctionMoveDelay = 0.5f;
    private float junctionMoveTimer = 0f;
    private SplineContainer spline;


    [Header("JumpMove")]
    [SerializeField] private float jumpStrength = 8f;
    [SerializeField] private float jumpDelay = 0.5f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private Transform splashPos;
    [HideInInspector] public bool canJump = true; 

    [Header("Trail")]
    [SerializeField] private bool trailActive;
    [SerializeField] private Transform trails;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private float trailInstantiateDelay = 0.1f;
    private float trailInstantiateTimer;
    [SerializeField] private float trailDuration = 1f;

    [Header("Impact")]
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private float impactScale;


    RaycastHit[] fingerHits;
    private bool fingerActive;
    private float fingerTimer = 0f;
    private Vector2 MouseStartPos;
    private Vector2 MouseEndPos;

    private LayerMask obstacleLayer;

    private void Awake()
    {
        Instance = this;
    }

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

            var v3 = Input.mousePosition;
            v3.z = 1f;
            Vector3 dir = (Camera.main.ScreenToWorldPoint(v3) - Camera.main.transform.position).normalized;
            fingerHits = Physics.RaycastAll(Camera.main.transform.position, dir, 1000f);
            
            fingerActive = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            MouseEndPos = Input.mousePosition;
            Vector3 dist = MouseEndPos - MouseStartPos;
            // Swipe
            if (dist.magnitude > fingerSwipDistMin && fingerTimer > fingerDelay)
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

    private void FixedUpdate()
    {
        // Junction Move
        if (spline)
        {
            JunctionMove();
        }
    }

    private IEnumerator TrailLife() {
        GameObject trail = Instantiate(trailPrefab, trails.position, Quaternion.identity, ScrollingManager.instance.propsParent);
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

            AudioManager.Instance.PlaySFX("Swipe");

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
        if (inMove || !canJump)
            yield break;

        inMove = true;
        trailActive = false;

        // est ce qu'on peut changer de voies en saut ?
        transform.DOMoveY(transform.position.y + jumpStrength, jumpDelay).SetEase(jumpCurve);
        AudioManager.Instance.PlaySFX("Saut");
        yield return new WaitForSeconds(jumpDelay);
        GameObject splash = Instantiate(splashPrefab, splashPos.position, Quaternion.identity, ScrollingManager.instance.gameObject.transform.GetChild(0));
        trailActive = true;
        inMove = false;
        AudioManager.Instance.PlaySFX("Atterrissage");
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
                        StartCoroutine(hit.transform.GetComponent<SinkObstacle>().ClickObstacle());
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

    public void JunctionChoice(JunctionPlayerDetect jpd) {
        
        float offSet = ScrollingManager.instance.xValueOffset;

        switch (currentSide) { 
            case Way.Left:
                inMove = true;
                spline = jpd.SplineLeft;
                junctionMoveTimer = 0f;

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
                inMove = true;
                spline = jpd.SplineRight;
                junctionMoveTimer = 0f;

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

    private void JunctionMove() {
        Vector3 newPlayerPos = transform.position;
        Vector3 playerDir = transform.forward;
        Quaternion playerRot = Quaternion.identity;

        float progressValue = junctionMoveTimer / junctionMoveDelay;

        // Pos
        newPlayerPos.x = spline.EvaluatePosition(progressValue).x;
        transform.position = newPlayerPos;

        // Dir
        playerDir = spline.EvaluateTangent(progressValue);
        if (Vector3.Magnitude(playerDir) <= Mathf.Epsilon)
        {
            if (progressValue < 1f)
                playerDir = spline.EvaluateTangent(Mathf.Min(1f, progressValue + 0.01f));
            else
                playerDir = spline.EvaluateTangent(progressValue - 0.01f);
        }
        playerDir.Normalize();

        // Rot
        playerRot = Quaternion.LookRotation(playerDir, Vector3.up);
        transform.rotation = playerRot;

        junctionMoveTimer += Time.deltaTime;

        // endMove
        if (junctionMoveTimer >= junctionMoveDelay)
        {
            junctionMoveTimer = 0;
            spline = null;
            inMove = false;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            // UI Update
            UIManager.Instance.UpdateJunctionText();
        
            int doubleBlockCount = 0;
            foreach (var block in ScrollingManager.instance.blocks) {
                if (block.name == doubleBlockPrefab.name + "(Clone)") { 
                    doubleBlockCount++;
                }
            }

            UIManager.Instance.distanceBeforeNextChoice = ( 
                ScrollingManager.instance.blocksCountBeforeJunction + doubleBlockCount
            ) * ScrollingManager.instance.BlockLength + transform.position.z;

            UIManager.Instance.traveledDistance = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player collision : " + collision.gameObject.ToString());

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
