using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public string[] colors;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
