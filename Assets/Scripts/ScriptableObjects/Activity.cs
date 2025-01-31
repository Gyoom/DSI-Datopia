using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Activity", order = 1)]
public class Activity : ScriptableObject
{
    public string AcivityName;
    public string Description;
    public List<Theme> associatedThemes;
}