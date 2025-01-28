using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string theme;
    [Range(1f, 10f)] 
    public string difficulty;
    
    public List<string> themes;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }
}
