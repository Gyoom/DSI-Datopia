using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ThemePair", order = 1)]
public class ThemePair : ScriptableObject
{
    public Tier tier;
    public Theme first;
    public Theme second; 
}

public enum Tier { Tier1, Tier2, Tier3 }

/*public enum options { 
    Classique, Insolite, Dynamique, Calme, Coop�ration, Comp�tition, Instructif, amusant,
    Interieur, Ext�rieur, Publique, Intime, Urbain, Rural, Chic, D�contract�,
    Repas, PasDeRepas, Journ�e, Soir�e, Economique, Couteux, Sortie, Domicile, Activit�, Discussion, Sportif, Culturel  
}
*/