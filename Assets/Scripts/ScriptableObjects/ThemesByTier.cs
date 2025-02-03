using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Themes Tier", order = 1)]
public class ThemesByTier : ScriptableObject
{
    public Tier tier;
    public List<ThemePair> choiceList;
}