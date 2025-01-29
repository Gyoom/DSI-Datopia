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
    Classique, Insolite, Dynamique, Calme, Coopération, Compétition, Instructif, amusant,
    Interieur, Extérieur, Publique, Intime, Urbain, Rural, Chic, Décontracté,
    Repas, PasDeRepas, Journée, Soirée, Economique, Couteux, Sortie, Domicile, Activité, Discussion, Sportif, Culturel  
}
*/