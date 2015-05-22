using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct Trial
{
    float reactionTime;
    int trialNumber;
}

public class GameController : MonoBehaviour {

    public string[] colors;
    public int totalTrials;
    private Trial[] trials;
    private GameObject gameText;

	// Use this for initialization
	void Start () {
        gameText = GameObject.Find("GameText");
        trials = new Trial[totalTrials];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void setUpGame()
    {
        System.Array.Clear(trials, 0, totalTrials);
        gameText.GetComponent<Text>().text = "";
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
