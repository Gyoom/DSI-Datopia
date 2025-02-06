using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameStep gameStep = GameStep.Choices;
  
    [Header("Choices")]
    [SerializeField] private TextMeshProUGUI textLeft;
    [SerializeField] private TextMeshProUGUI textRight;
    [SerializeField] private List<ThemesByTier> ThemesTiers = new List<ThemesByTier>();
    [SerializeField][Range(1, 4)] private int nbrUseByTier = 1;
    private int currentTierIndex = 0;
    private int currentTierUseCount = 0;
    private JunctionOption currentLeft;
    private JunctionOption currentRight;
    private List<ThemePair> usedPairs = new List<ThemePair>();
    [HideInInspector] public List<JunctionOption> previousChoices = new List<JunctionOption>();

    [Header("Dates")]
    [SerializeField] private List<Activity> defaultActivities;

    [Header("End")]
    [SerializeField] private float fadeDuration = 2.0f;
    [SerializeField] private CanvasGroup blackScreenUIElement;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private TextMeshProUGUI placesText;
    [SerializeField] private Button replayButton;

    [Header("Progress")]
    [SerializeField] private Scrollbar progressSlider;
    [HideInInspector] public float distanceBeforeNextChoice;
    [HideInInspector] public float traveledDistance;
    [SerializeField] private Transform goal;
    [SerializeField] private Vector2 scaleRange = new Vector2(1.5f, 2.5f);
    [SerializeField] private float scaleDuration = 1f;

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

    // Dates
    private List<Activity> AllActivities = new List<Activity>();

    private void Awake()
    {
        Application.targetFrameRate = Mathf.FloorToInt((float)Screen.currentResolution.refreshRateRatio.value);
        Instance = this;
    }

    private void Start()
    {
        // progress bar animation
        goal.DOScale(scaleRange.y, scaleDuration).From(scaleRange.x).SetLoops(-1, LoopType.Yoyo);
       
        // data storing
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Datas/Activities" });
      
        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var newActivity = AssetDatabase.LoadAssetAtPath<Activity>(SOpath);
            AllActivities.Add(newActivity);
        }
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

    public void UpdateJunctionText()
    {
        switch (gameStep) { 
            case GameStep.Choices:
                DisplayChoice();
                break;
            case GameStep.Dates:
                DisplayDates();
                gameStep = GameStep.End;
                break;
            case GameStep.End:
                StartCoroutine(DisplayEnd());
                break;
            default:
                break;
        }
    }

    private void DisplayChoice() {
        ThemePair tp = GetNextThemePair();

        int ran = Random.Range(0, 2);
        if (ran == 0)
        {
            textLeft.text = tp.first.Description;
            textRight.text = tp.second.Description;
            currentLeft = tp.first;
            currentRight = tp.second;
        }
        else
        {
            textLeft.text = tp.second.Description;
            textRight.text = tp.first.Description;
            currentLeft = tp.second;
            currentRight = tp.first;
        }
        textLeft.transform.parent.gameObject.SetActive(true);
        textRight.transform.parent.gameObject.SetActive(true);

        AudioManager.Instance.PlaySFX("Panneaux");
    }

    private void DisplayDates()
    {
        List<Activity> ActivityPair = GetActivityPair();

        int ran = Random.Range(0, 2);
        if (ran == 0)
        {
            textLeft.text = ActivityPair[0].Description;
            textRight.text = ActivityPair[1].Description;
            currentLeft = ActivityPair[0];
            currentRight = ActivityPair[1];
        }
        else
        {
            textLeft.text = ActivityPair[1].Description;
            textRight.text = ActivityPair[0].Description;
            currentLeft = ActivityPair[1];
            currentRight = ActivityPair[0];
        }
        textLeft.transform.parent.gameObject.SetActive(true);
        textRight.transform.parent.gameObject.SetActive(true);
    }

    private IEnumerator DisplayEnd()
    {
        AudioManager.Instance.PlaySFX("Fin");
        endText.gameObject.SetActive(true);
        endText.text = " You have chosen : " + previousChoices[3].Name;
        replayButton.gameObject.SetActive(true);
        placesText.gameObject.SetActive(true);

        // Fade backaground
        float time = 0;
        float startValueAlphaScreen = blackScreenUIElement.alpha;

        float halfDuration = fadeDuration / 2;

        while (time < halfDuration)
        {
            blackScreenUIElement.alpha = Mathf.Lerp(startValueAlphaScreen, 1, time / halfDuration);

            time += Time.deltaTime;
            yield return null;
        }
        blackScreenUIElement.alpha = 1;

        // Display Elements
        time = 0;
        startValueAlphaScreen = endText.GetComponent<CanvasGroup>().alpha;
        float newAlpha = 0;

        while (time < halfDuration)
        {
            newAlpha = Mathf.Lerp(startValueAlphaScreen, 1, time / halfDuration);
            endText.GetComponent<CanvasGroup>().alpha = newAlpha;
            replayButton.GetComponent<CanvasGroup>().alpha = newAlpha;
            placesText.GetComponent<CanvasGroup>().alpha = newAlpha;

            time += Time.deltaTime;
            yield return null;
        }
        blackScreenUIElement.alpha = 1;

        ScrollingManager.instance.isScrolling = false;
    }

    public void EmptyJonctionText(int choice)
    {


        if (choice == -1)
            previousChoices.Add(currentLeft);
        if (choice == 1)
            previousChoices.Add(currentRight);

        textLeft.transform.parent.gameObject.SetActive(false);
        textRight.transform.parent.gameObject.SetActive(false);
    }

    private ThemePair GetNextThemePair()
    {
        ThemesByTier currentTier = ThemesTiers[currentTierIndex];
        List<ThemePair> tempList = new List<ThemePair>();

        foreach (var item in currentTier.choiceList)
        {
            if (!usedPairs.Contains(item))
                tempList.Add(item);
        }

        if (tempList.Count == 0)
            return null;
        else
        {
            ThemePair currentPair = tempList[Random.Range(0, tempList.Count)];
            usedPairs.Add(currentPair);
            
            currentTierUseCount++;
            if (currentTierUseCount == nbrUseByTier)
            {
                currentTierUseCount = 0;
                currentTierIndex++;
                if (currentTierIndex >= ThemesTiers.Count)
                    gameStep = GameStep.Dates;
            }

            return currentPair;
        }
    }

    private List<Activity> GetActivityPair() {
        List<Activity> validActivities = new List<Activity>(AllActivities);

        // remove opposed activities
        for (int i = 0; i < previousChoices.Count; i++)
        {
            //Find bad Theme
            ThemePair currentPair = usedPairs[i];
            Theme badTheme;

            if (currentPair.first == previousChoices[i])
            {
                badTheme = currentPair.second;
            } else { 
                badTheme = currentPair.first;
            }
            
            // remove activity with bad theme
            List<Activity> ActivitiesToRemove = new List<Activity>();
            
            foreach (var activity in validActivities)
            {
                if (activity.associatedThemes.Contains(badTheme)) { 
                    ActivitiesToRemove.Add(activity);
                }
            }

            foreach (var activity in ActivitiesToRemove)
            {
                validActivities.Remove(activity);
            }

        }

        List<Activity> MatchedActivities = new List<Activity>();

        if (validActivities.Count < 2)
        {
            Debug.Log("Error : no valid activity found");
            MatchedActivities = new List<Activity>(defaultActivities);
        }
        else
        {
            for (int i = previousChoices.Count; i > 0; i--) // nbr match required
            {
                foreach (var item in validActivities) // check every activity
                {
                    bool match = true;
                    for (int j = 0; j < i; j++)
                    { // check if current activity have the required match count
                        if (!item.associatedThemes.Contains((Theme)previousChoices[j]))
                            match = false;
                    }

                    if (match)
                        MatchedActivities.Add(item);
                }

                if (MatchedActivities.Count >= 2)
                    break;
                else
                    MatchedActivities.Clear();
            }

            // Debug info
            if (MatchedActivities.Count < 2)
            {
                Debug.Log("Error : no matching activity found");
                MatchedActivities = new List<Activity>(defaultActivities);
            }

        }

        // Get Random activity in matched pool
        List<Activity> pair = new List<Activity>();

        if (MatchedActivities.Count >= 2)
        {
            int ran = Random.Range(0, MatchedActivities.Count);
            pair.Add(MatchedActivities[ran]);
            MatchedActivities.RemoveAt(ran);

            ran = Random.Range(0, MatchedActivities.Count);
            pair.Add(MatchedActivities[ran]);
        }
        else {
            int ran = Random.Range(0, AllActivities.Count);
            pair.Add(AllActivities[ran]);
            AllActivities.RemoveAt(ran);

            ran = Random.Range(0, AllActivities.Count);
            pair.Add(AllActivities[ran]);
        }

        return pair;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Travel(float distance) {
        traveledDistance += distance;
        progressSlider.value = traveledDistance / distanceBeforeNextChoice;
    }
}

public enum GameStep { Choices, Dates, End }
