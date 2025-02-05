using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class ScrollingManager : MonoBehaviour
{
    
    public static ScrollingManager instance;

    [Header("Refs")]
    [SerializeField] private GameObject player;

    [Header("Prefabs")]
    [SerializeField]
    private List<GameObject> rights = new List<GameObject>();
    [SerializeField]
    private GameObject junction;
    [SerializeField]
    private GameObject postJunction;

    [Header("Scrolling")]
    public bool isScrolling = true;
    [SerializeField]
    private float backgroundSpeed = 0.1f;
    [SerializeField] private Transform blocksParent;
    public Transform propsParent;
    public  List<GameObject> blocks = new List<GameObject>();
    public float BlockLength = 100f;
    [SerializeField]
    private float blockOffset = 20f;
    [SerializeField]
    public float xValueOffset = 65f;
    [SerializeField]
    private Vector2Int startBlocksCountRange = new Vector2Int(0, 0);
    [SerializeField]
    private Vector2Int junctionBlocksCountRange = new Vector2Int(1, 3);

    [Header("Comeback")]
    [SerializeField] private float comebackAmount = 50f;
    [SerializeField] private float comebackDelay = 0.5f;

    [Header("Values Ingame")]
    public float xValue = 0f;
    [SerializeField] private int nextBlockDecount = 0;

    private float moveCount = 0f;
    [HideInInspector] public bool junctionPending;
    [HideInInspector] public int blocksCountBeforeJunction = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetBlockDecount(true);
        UIManager.Instance.UpdateJunctionText();
        UIManager.Instance.distanceBeforeNextChoice = (blocks.Count + blocksCountBeforeJunction) * BlockLength;

    }

    void FixedUpdate()
    {   
        if (!isScrolling && blocks.Count > 0)
            return;

        // move amount
        float move = Time.deltaTime * backgroundSpeed;
        moveCount += move;

        // apply move
        List<GameObject> blocksToDelete = new List<GameObject>();
        foreach (var block in blocks) {
            block.transform.position += Vector3.back * move;
            if (block.transform.position.z < player.transform.position.z - BlockLength * 2f) { 
                blocksToDelete.Add(block);
            }
        }
        for (int i = 0; i < propsParent.childCount; i++)
        {
            propsParent.GetChild(i).position += Vector3.back * move;
        }
        UIManager.Instance.Travel(move);

        // delete
        foreach (var block in blocksToDelete)
        {
          Destroy(block);
        }
        blocks.RemoveAll((gm) => blocksToDelete.Contains(gm));

        // add new
        if (moveCount >= BlockLength) {
            GameObject newBlockPrefab = GetNextBlock();
            GameObject temp = Instantiate(newBlockPrefab, blocksParent);
            if (temp.GetComponent<DoubleConstructor>()) {
                temp.GetComponent<DoubleConstructor>().Instantiate(rights[Random.Range(0, rights.Count)]);
            }

            Vector3 tempPos = new Vector3(xValue, 0, blockOffset + blocks[blocks.Count - 1].transform.position.z + BlockLength);
            temp.transform.localPosition = tempPos;
            
            blocks.Add(temp);
            moveCount = 0;  
        }
    
    }

    private GameObject GetNextBlock() {
        GameObject gm;
        if (!junctionPending && nextBlockDecount == 0)
        {
            gm = junction;
            SetBlockDecount(false);
            junctionPending = true;
        }
        else if (junctionPending) { 
            gm = postJunction;
        } else {
            gm = rights[Random.Range(0, rights.Count)];
            nextBlockDecount--;
        }

        return gm;
    }

    private void SetBlockDecount(bool start) {
        int min = 0;
        int max = 0;

        if (start)
        {
            min = startBlocksCountRange.x;
            max = startBlocksCountRange.y;
        }
        else {
            min = junctionBlocksCountRange.x;
            max = junctionBlocksCountRange.y;
        }
        nextBlockDecount = Random.Range(min, max + 1);
        blocksCountBeforeJunction = nextBlockDecount;
    }

    public IEnumerator ComeBack(float hitRecoil)
    {
        PlayerController.Instance.canJump = false;
        if (hitRecoil == -1f)
            hitRecoil = comebackAmount;

        isScrolling = false;
        moveCount -= hitRecoil;
        foreach (var block in blocks)
        {
            block.transform.DOMoveZ(block.transform.position.z + hitRecoil, comebackDelay);
        }
        propsParent.DOMoveZ(propsParent.position.z + hitRecoil, comebackDelay);

        yield return new WaitForSeconds(comebackDelay);

        PlayerController.Instance.canJump = true;
        UIManager.Instance.Travel(-hitRecoil);
        isScrolling = true;
    }
}
