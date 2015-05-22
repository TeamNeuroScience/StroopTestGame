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
    private GameObject gameText;

    private enum GameState
    {
        WAITING_FOR_COLOR, WAITING_NEXT_TRIAL, TEST_COMPLETE
    }

    GameState _gameState;

	// Use this for initialization
	void Start () {
        gameText = GameObject.Find("GameText");
        trials = new Trial[totalTrials];
	}
	
	// Update is called once per frame
	void Update () {
        switch (_gameState)
        {
            case GameState.WAITING_FOR_COLOR:
                break;
            case GameState.WAITING_NEXT_TRIAL:
                break;
            case GameState.TEST_COMPLETE:
                break;
            default:
                break;
        }
	
	}

    private void setUpGame()
    {
        System.Array.Clear(trials, 0, totalTrials);
        gameText.GetComponent<Text>().text = "";
        _gameState = GameState.WAITING_FOR_COLOR;
        currentTrial = 0;
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
