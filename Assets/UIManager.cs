using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private TextMeshProUGUI textLeft;
    [SerializeField] private TextMeshProUGUI textRight;


    private void Start()
    {
         Instance = this;
    }

    public void DisplayJonctionText(ThemePair tp)
    {
        
        int ran = Random.Range(0, 2);
        if (ran == 0)
        {
            textLeft.text = tp.first.Description;
            textRight.text = tp.second.Description;
        }
        else { 
            textLeft.text = tp.second.Description;
            textRight.text = tp.first.Description;
        }
    }

    public void EmptyJonctionText() {
        Debug.Log("Empty");
        textLeft.text = "";
        textRight.text = "";
    }

}
