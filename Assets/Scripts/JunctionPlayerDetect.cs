using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class JunctionPlayerDetect : MonoBehaviour
{
    public SplineContainer SplineLeft;
    public SplineContainer SplineRight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().JunctionChoice(this);      
        }
    }
}
