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
    Classique, Insolite, Dynamique, Calme, Coopération, Compétition, Instructif, amusant,
    Interieur, Extérieur, Publique, Intime, Urbain, Rural, Chic, Décontracté,
    Repas, PasDeRepas, Journée, Soirée, Economique, Couteux, Sortie, Domicile, Activité, Discussion, Sportif, Culturel  
}
*/