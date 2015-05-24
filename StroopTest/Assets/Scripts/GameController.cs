using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public struct Trial
{
    public int reactionTime; //Reaction time in miliseconds
    public int trialNumber;
    public bool isCongruent; //Indicates whether word matches the color
    public bool isCorrect;
}

public class GameController : MonoBehaviour
{

    private static GameController _instance;

    public string[] colors;
    public int totalTrials;
    public float trialDelay = 0.5f; //Delay in seconds from pressing space to the next trial
    private float trialDelayStart;
    private Trial[] trials;
    private int currentTrial;
    private Text gameText, averageResultsText, exportStatusText;
    private System.DateTime trialStartTime; //Start time for a given trial. Used to measure reaction time

    private GameObject menuCanvas, gameCanvas, resultsCanvas;

    private enum GameState
    {
        WAITING_FOR_ANSWER, WAITING_NEXT_TRIAL, WAITING_TRIAL_DELAY, TEST_BEGINS, TEST_COMPLETE, INACTIVE
    }

    GameState _gameState;

    void Awake()
    {
        _instance = this;
    }

    public GameController getInstance()
    {
        return _instance ? _instance : new GameController();
    }

    // Use this for initialization
    void Start()
    {
        gameText = GameObject.Find("GameText").GetComponent<Text>();
        averageResultsText = GameObject.Find("AverageResultsText").GetComponent<Text>();
        exportStatusText = GameObject.Find("ExportStatus").GetComponent<Text>();
        trials = new Trial[totalTrials];
        menuCanvas = GameObject.Find("MainMenuCanvas");
        menuCanvas.SetActive(true);
        gameCanvas = GameObject.Find("GameCanvas");
        gameCanvas.SetActive(false);
        resultsCanvas = GameObject.Find("ResultsCanvas");
        resultsCanvas.SetActive(false);
        _gameState = GameState.INACTIVE;

    }

    // Update is called once per frame
    void Update()
    {
        switch (_gameState)
        {
            case GameState.WAITING_FOR_ANSWER:
                bool isCorrect = false;
                bool receivedInput = false;
                if (Input.GetButton("red"))
                {
                    receivedInput = true;
                    isCorrect = gameText.color == Color.red;
                }
                else if (Input.GetButton("black"))
                {
                    receivedInput = true;
                    isCorrect = gameText.color == Color.green;
                }
                else if (Input.GetButton("blue"))
                {
                    receivedInput = true;
                    isCorrect = gameText.color == Color.blue;
                }
                if (receivedInput)
                {
                    if (isCorrect)
                    {
                        gameText.text = "Correct!\n";
                        gameText.color = Color.green;
                    }
                    else
                    {
                        gameText.text = "Incorrect!\n";
                        gameText.color = Color.red;
                    }
                    //Show reaction time as an integer of miliseconds that passed since the beginning of the trial
                    int reactionTime = (int)(System.DateTime.Now - trialStartTime).TotalMilliseconds;
                    gameText.text += "RectionTime:" + reactionTime.ToString() + " ms";
                    _gameState = GameState.WAITING_NEXT_TRIAL;

                    //Record the information about a given trial
                    trials[currentTrial].trialNumber = currentTrial;
                    trials[currentTrial].reactionTime = reactionTime;
                    trials[currentTrial].isCorrect = isCorrect;
                    currentTrial++;
                }
                break;


            case GameState.WAITING_NEXT_TRIAL:
                if (currentTrial + 1 > totalTrials)
                {
                    _gameState = GameState.TEST_COMPLETE;
                    break;
                }
                if (Input.GetButton("NextTrial"))
                {
                    trialDelayStart = Time.time;
                    _gameState = GameState.WAITING_TRIAL_DELAY;
                    gameText.text = "";
                    break;
                }
                break;

            case GameState.WAITING_TRIAL_DELAY:
                if (Time.time - trialDelayStart > trialDelay)
                {
                    gameText.text = colors[Random.Range(0, colors.Length)];
                    string colorStr = colors[Random.Range(0, colors.Length)];
                    gameText.color = parseColor(colorStr);
                    _gameState = GameState.WAITING_FOR_ANSWER;
                    trialStartTime = System.DateTime.Now;
                    trials[currentTrial].isCongruent = gameText.text == colorStr;

                }
                break;
            //When the game starts, set up initial game text 
            case GameState.TEST_BEGINS:
                gameText.text = "Press space to begin!";
                _gameState = GameState.WAITING_NEXT_TRIAL;
                break;

            //When the test is complete, move onto the results canvas
            case GameState.TEST_COMPLETE:
                StartCoroutine(fadeOut(gameCanvas, 2.0f));
                StartCoroutine(fadeIn(resultsCanvas, 2.0f));
                averageResultsText.text = "Correctness percentage: " + GetCorrectnessPercentage().ToString("0.00") + "%\n" +
                    "Average reaction time: " + GetAverageReaction().ToString() + "ms";
                _gameState = GameState.INACTIVE;
                break;


            case GameState.INACTIVE:
                break;


            default:
                break;
        }

    }

    //Get average reaction time. Precondition: all tests have been complete
    private float GetAverageReaction()
    {
        int totalReaction = 0;
        foreach (Trial trial in trials)
        {
            if (trial.reactionTime > 0)
            {
                totalReaction += trial.reactionTime;
            }
        }
        return totalReaction / totalTrials;
    }

    private float GetCorrectnessPercentage()
    {
        int correctCount = 0;
        foreach (Trial trial in trials)
        {
            if (trial.isCorrect) correctCount++;
        }
        return correctCount * 100 / totalTrials;
    }

    private Color parseColor(string color)
    {
        if (color == "red") { return Color.red; }
        else if (color == "green") { return Color.green; }
        return Color.blue;
    }

    private void setUpGame()
    {
        System.Array.Clear(trials, 0, totalTrials);
        gameText.GetComponent<Text>().text = "";
        _gameState = GameState.TEST_BEGINS;
        currentTrial = 0;
    }

    private void ExportCVS()
    {
        string path = @"E:\test.csv";
        string output = "Average rection time:," + GetAverageReaction() + "ms\n" +
            "Correctness Perfentage:," + GetCorrectnessPercentage().ToString("0.00") + "%\n"
        + "Trial#,congruency,correctness,reactiontime\n";
        foreach (Trial trial in trials)
        {
            output += trial.trialNumber.ToString() + ",";
            output += trial.isCongruent ? "congruent," : "incongruent,";
            output += trial.isCorrect ? "correct," : "incorrect,";
            output += trial.reactionTime.ToString() + "ms\n";
        }
        try {
            File.WriteAllText(path, output);
            exportStatusText.text = "Export successful!";
        }
        catch (System.Exception e)
        {
            exportStatusText.text = "Export FAILED!\n";
            exportStatusText.text += e.Message;
        }
    }
    public void onClickExportResults()
    {
        ExportCVS();
    }

    public void onClickNewTest()
    {
        StartCoroutine(fadeIn(gameCanvas, 2.0f));
        StartCoroutine(fadeOut(menuCanvas, 2.0f));
        setUpGame();
    }

    public void onClickMainMenu()
    {
        StartCoroutine(fadeOut(resultsCanvas, 2.0f));
        StartCoroutine(fadeIn(menuCanvas, 2.0f));
    }

    //Functions to fade user interface in and out.
    IEnumerator fadeIn(GameObject obj, float speed)
    {
        float increment;
        obj.SetActive(true);
        CanvasGroup cv = obj.GetComponent<CanvasGroup>();
        while (cv.alpha < 1)
        {
            increment = speed * Time.deltaTime;
            if (cv.alpha + increment > 1) cv.alpha = 1;
            else cv.alpha += speed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator fadeOut(GameObject obj, float speed)
    {
        float increment;
        CanvasGroup cv = obj.GetComponent<CanvasGroup>();
        while (cv.alpha > 0)
        {
            increment = speed * Time.deltaTime;
            if (cv.alpha - increment < 0) cv.alpha = 0;
            else cv.alpha -= speed * Time.deltaTime;
            yield return null;
        }
        obj.SetActive(false);
    }
}
