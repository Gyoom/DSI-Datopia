using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleConstructor : MonoBehaviour
{
    [SerializeField] private Transform slot1;
    [SerializeField] private Transform slot2;

    public void Instantiate(GameObject straightBlock)
    {
        for (int i = 0; i < slot1.childCount; i++)
        {
            Destroy(slot1.GetChild(i).gameObject);
        }
        for (int i = 0; i < slot2.childCount; i++)
        {
            Destroy(slot2.GetChild(i).gameObject);
        }
        GameObject temp = Instantiate(straightBlock, slot1);
        temp.transform.localPosition = Vector3.zero;
        temp = Instantiate(straightBlock, slot2);
        temp.transform.localPosition = Vector3.zero;

    }
}
