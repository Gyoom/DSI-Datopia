using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Left-Right")]
    [SerializeField] private TextMeshProUGUI textLeft;
    [SerializeField] private TextMeshProUGUI textRight;

    [Header("FPS")]
    [SerializeField] bool FPSActive;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Color badColor = Color.red;
    [SerializeField] Color neutralColor = Color.yellow;
    [SerializeField] Color goodColor = Color.cyan;

    [SerializeField] float badValue = 50;
    [SerializeField] float neutralValue = 60;

    [SerializeField] float fps;

    const float updateInterval = .1f;
    float accum;
    float frames;
    float timeLeft;

    private void Awake()
    {
        Application.targetFrameRate = Mathf.FloorToInt((float)Screen.currentResolution.refreshRateRatio.value);
    }


    private void Start()
    {
         Instance = this;
    }


    private void Update()
    {
        if (FPSActive) {
            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeLeft <= 0)
            {
                fps = accum / frames;

                if (fps < badValue)
                {
                    text.color = badColor;
                }
                else if (fps < neutralValue)
                {
                    text.color = neutralColor;
                }
                else
                {
                    text.color = goodColor;
                }

                text.text = fps.ToString("f1");
                timeLeft = updateInterval;
                accum = 0;
                frames = 0;
            }
        }
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
