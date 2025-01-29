using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Themes Tier", order = 1)]
public class ThemeTier : ScriptableObject
{
    public Tier tier;
    public List<ThemePair> choiceList;
}

/*public enum options { 
    Classique, Insolite, Dynamique, Calme, Coop�ration, Comp�tition, Instructif, amusant,
    Interieur, Ext�rieur, Publique, Intime, Urbain, Rural, Chic, D�contract�,
    Repas, PasDeRepas, Journ�e, Soir�e, Economique, Couteux, Sortie, Domicile, Activit�, Discussion, Sportif, Culturel  
}
*/