using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SinkObstacle : MonoBehaviour {

    [SerializeField] private float delayMove = 0.3f;
    [SerializeField] private float delayBerforeUp = 3f;
    [SerializeField] private float downAmount = 5f;

    private BoxCollider Collider; 

    private void Start()
    {
        Collider = transform.parent.GetComponent<BoxCollider>();
    }

    public IEnumerator ClickObstacle() { 
        Collider.enabled = false;
        transform.parent.DOMoveY(transform.parent.position.y - downAmount, delayMove);
        
        yield return new WaitForSeconds(delayMove + delayBerforeUp);

        Collider.enabled = true;
        transform.parent.DOMoveY(transform.parent.position.y + downAmount, delayMove);

    }
}
