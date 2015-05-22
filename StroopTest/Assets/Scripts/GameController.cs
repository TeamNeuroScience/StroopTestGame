using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct Trial
{
    float reactionTime;
    int trialNumber;
    bool isSameWord; //Indicates whether word matches the color
}

public class GameController : MonoBehaviour {

    public string[] colors;
    public int totalTrials;
    private Trial[] trials;
    private int currentTrial;
    private Text gameText;
    private System.DateTime trialStartTime; //Start time for a given trial. Used to measure reaction time

    private GameObject menuCanvas, gameCanvas;

    private enum GameState
    {
        WAITING_FOR_COLOR, WAITING_NEXT_TRIAL, TEST_COMPLETE, INACTIVE
    }

    GameState _gameState;

    void Awake()
    {
        
    }

	// Use this for initialization
	void Start () {
        gameText = GameObject.Find("GameText").GetComponent<Text>();
        trials = new Trial[totalTrials];
        menuCanvas = GameObject.Find("MainMenuCanvas");
        menuCanvas.SetActive(true);
        gameCanvas = GameObject.Find("GameCanvas");
        gameCanvas.SetActive(false);
        _gameState = GameState.INACTIVE;
    }
	
	// Update is called once per frame
	void Update () {
        switch (_gameState)
        {
            case GameState.WAITING_FOR_COLOR:
                string inputColor = "";
                if (Input.GetButton("red"))
                {
                    inputColor = "red";
                } else if (Input.GetButton("green"))
                {
                    inputColor = "green";
                }
                 else if (Input.GetButton("blue"))
                {
                    inputColor = "blue";
                }
                if (inputColor != "")
                {
                    if (inputColor == gameText.color.ToString())
                    {
                        gameText.text = "Correct!\n";
                        gameText.color = Color.green;
                    }
                    else
                    {
                        gameText.text = "Incorrect!\n";
                        gameText.color = Color.red;
                    }
                    gameText.text += "RectionTime:" + (trialStartTime - System.DateTime.Now).Milliseconds.ToString();
                    _gameState = GameState.WAITING_NEXT_TRIAL;
                }
                break;
            case GameState.WAITING_NEXT_TRIAL:
                if (currentTrial > totalTrials)
                {
                    _gameState = GameState.TEST_COMPLETE;
                    break;
                }
                if (Input.GetButton("NextTrial")) {
                    gameText.text = colors[Random.Range(0, colors.Length)];
                    string colorStr = colors[Random.Range(0, colors.Length)];
                    gameText.color = parseColor(colorStr);
                    _gameState = GameState.WAITING_FOR_COLOR;
                    trialStartTime = System.DateTime.Now;
                    break;
                }
                break;
            case GameState.TEST_COMPLETE:
                break;
            case GameState.INACTIVE:
                break;
            default:
                break;
        }
	
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
        _gameState = GameState.WAITING_FOR_COLOR;
        currentTrial = 0;
    }

    public void onClickNewTest()
    {
        StartCoroutine(fadeIn(gameCanvas, 2.0f));
        StartCoroutine(fadeOut(menuCanvas, 2.0f));
        _gameState = GameState.WAITING_NEXT_TRIAL;
    }

	//Functions to fade user interface in and out.
	IEnumerator fadeIn(GameObject obj, float speed) {
		float increment;
		obj.SetActive(true);
		CanvasGroup cv = obj.GetComponent<CanvasGroup>();
		while (cv.alpha < 1) {
			increment = speed * Time.deltaTime;
			if (cv.alpha + increment > 1) cv.alpha = 1;
			else cv.alpha += speed * Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator fadeOut(GameObject obj, float speed) {
		float increment;
		CanvasGroup cv = obj.GetComponent<CanvasGroup>();
		while (cv.alpha > 0) {
			increment = speed * Time.deltaTime;
			if (cv.alpha - increment < 0) cv.alpha = 0;
			else cv.alpha -= speed * Time.deltaTime;
			yield return null;
		}
		obj.SetActive(false);
	}
}
