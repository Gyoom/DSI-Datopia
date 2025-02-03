using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameStep gameStep = GameStep.Choices;

    [Header("Choices")]
    [SerializeField] private List<ThemesByTier> ThemesTiers = new List<ThemesByTier>();
    [SerializeField][Range(1, 4)] private int nbrUseByTier = 1;
    private int currentTierIndex = 0;
    private int currentTierUseCount = 0;
    private JunctionOption currentLeft;
    private JunctionOption currentRight;
    private List<ThemePair> usedPairs = new List<ThemePair>();
    [HideInInspector] public List<JunctionOption> previousChoices = new List<JunctionOption>();

    [Header("Dates")]
    [SerializeField] private List<Activity> Activities = new List<Activity>();

    [Header("End")]
    [SerializeField] private float fadeDuration = 2.0f;
    [SerializeField] private CanvasGroup blackScreenUIElement;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Button replayButton;

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

    private void DisplayDates()
    {
        List<Activity> ActivityPair = GetActivityPair();

        int ran = Random.Range(0, 2);
        if (ran == 0) {
            textLeft.text = ActivityPair[0].Description;
            textRight.text = ActivityPair[1].Description;
            currentLeft = ActivityPair[0];
            currentRight = ActivityPair[1];
        } else {
            textLeft.text = ActivityPair[1].Description;
            textRight.text = ActivityPair[0].Description;
            currentLeft = ActivityPair[1];
            currentRight = ActivityPair[0];
        }
    }

    private List<Activity> GetActivityPair() {
        List<Activity> MatchedActivities = new List<Activity>();

        foreach (var item in Activities)
        {
            if (item.associatedThemes.Contains((Theme) previousChoices[0]) &&
                item.associatedThemes.Contains((Theme) previousChoices[1]) &&
                item.associatedThemes.Contains((Theme) previousChoices[2])
            ) { 
                MatchedActivities.Add(item);
            }
        }

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
            int ran = Random.Range(0, Activities.Count);
            pair.Add(Activities[ran]);
            Activities.RemoveAt(ran);

            ran = Random.Range(0, Activities.Count);
            pair.Add(Activities[ran]);
        }

        return pair;
    }

    public void EmptyJonctionText(int choice) {

       
        if (choice == -1)
            previousChoices.Add(currentLeft);
        if (choice == 1)
            previousChoices.Add(currentRight);

        textLeft.text = "";
        textRight.text = "";
    }

    private IEnumerator DisplayEnd() {
        endText.gameObject.SetActive(true);
        endText.text = " You have chosen : " + previousChoices[3].Name; 
        replayButton.gameObject.SetActive(true);

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

            time += Time.deltaTime;
            yield return null;
        }
        blackScreenUIElement.alpha = 1;

        ScrollingManager.instance.isScrolling = false;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}

public enum GameStep { Choices, Dates, End }
