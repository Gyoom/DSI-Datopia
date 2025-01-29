using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionPlayerDetect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().JunctionMove();      
        }
    }
}
