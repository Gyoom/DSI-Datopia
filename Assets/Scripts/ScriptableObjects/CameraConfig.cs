using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CameraConfig", order = 1)]
public class CameraConfig : ScriptableObject
{
    public float timeOffset = 0.05f;
    public Vector3 posOffset = new Vector3(0f, 4f, -10f);
}
