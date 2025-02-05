using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SinkObstacle : MonoBehaviour {

    [SerializeField] private float delayMove = 0.3f;
    [SerializeField] private float delayBerforeUp = 3f;
    [SerializeField] private float downAmount;
    [SerializeField] private Material deadCroco;
    [SerializeField] private Material aliveCroco;
    private MeshRenderer mr;
    [SerializeField] private AudioClip deadCrocoSound;

    private BoxCollider Collider; 

    private void Start()
    {
        Collider = transform.parent.GetComponent<BoxCollider>();
        mr = GetComponent<MeshRenderer>();
    }

    public IEnumerator ClickObstacle() { 
        Collider.enabled = false;
        transform.parent.DOMoveY(transform.parent.position.y - downAmount, delayMove);
        mr.material = deadCroco;
        AudioManager.Instance.sfxSource.PlayOneShot(deadCrocoSound);
        
        yield return new WaitForSeconds(delayMove + delayBerforeUp);

        Collider.enabled = true;
        transform.parent.DOMoveY(transform.parent.position.y + downAmount, delayMove);
        mr.material = aliveCroco;

    }
}
