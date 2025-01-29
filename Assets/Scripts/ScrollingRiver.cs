using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingRiver : MonoBehaviour
{
    [SerializeField]
    private bool backgroundScrolling = true;
    [SerializeField]
    private float backgroundSpeed = 0.1f;
    [SerializeField]
    private Transform blocksParent; 
    [SerializeField]
    private List<GameObject> blocks = new List<GameObject>();
    [SerializeField]
    private float BlockLength = 100f;
    [SerializeField]
    private float blockOffset = 20f;
    [SerializeField]
    public float xValue = 0f;
    [SerializeField]
    public float xValueOffset = 65f;
    [SerializeField]
    private Vector2Int RangeBlocksCountBeforeNextJunction = new Vector2Int(1, 1);
    [SerializeField] private int nextBlockDecount = 0;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject right;
    [SerializeField]
    private GameObject junction;
    [SerializeField]
    private GameObject postJunction;

    [Header("Comeback")]
    [SerializeField] private float comebackAmount = 20f;
    [SerializeField] private float comebackDelay = 0.5f;

    private float moveCount = 0f;
    [HideInInspector] public bool junctionPending;
    private int blockCount = 0;


    private void Start()
    {
        SetBlockDecount();
    }

    void Update()
    {   
        if (!backgroundScrolling)
            return;

        float move = Time.deltaTime * backgroundSpeed;
        moveCount += move;
        foreach (var block in blocks) {
            block.transform.position += Vector3.back * move;   
        }

        if (blocks.Count > 0 && moveCount >= BlockLength) {
            GameObject toDelete = blocks[0];
            blocks.RemoveAt(0);
            Destroy(toDelete);

            GameObject newBlockPrefab = GetNextBlock();
            GameObject temp = Instantiate(newBlockPrefab, blocksParent);
            
            Vector3 tempPos = new Vector3(xValue, 0, blockOffset + (BlockLength * blocks.Count));
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
            SetBlockDecount();
            junctionPending = true;
        }
        else if (junctionPending) { 
            gm = postJunction;
        } else {
            gm = right;
            nextBlockDecount--;
        }

        return gm;
    }

    private void SetBlockDecount() {
        int min = RangeBlocksCountBeforeNextJunction.x;
        int max = RangeBlocksCountBeforeNextJunction.y;
        nextBlockDecount = Random.Range(min, max + 1);
    }

    public IEnumerator ComeBack()
    {
        backgroundScrolling = false;
        moveCount -= comebackAmount;
        transform.DOMoveZ(transform.position.z + comebackAmount, comebackDelay);
        yield return new WaitForSeconds(comebackDelay);
        backgroundScrolling = true;

    } 
}
