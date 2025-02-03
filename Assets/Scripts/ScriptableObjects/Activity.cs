using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Activity", order = 1)]
public class Activity : JunctionOption
{
    public List<Theme> associatedThemes;
}