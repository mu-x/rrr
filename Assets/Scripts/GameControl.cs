using UnityEngine;
using System.Collections;

/** Controls game processes */
public class GameControl : ScreenGUI {

	public GameObject modelCar;
	public Transform carSpot;

	static public bool isRunning = true;

	void setState(bool running) {
		isRunning = running;
		Time.timeScale = isRunning ? 1 : 0;
	}
	void Start () {
		if (MenuGUI.currentCar != null)
			modelCar = MenuGUI.currentCar;
		var userCar = Instantiate(
			modelCar, carSpot.position, carSpot.rotation) as GameObject;
		CarCamera.SetWatchPoint(userCar.transform.Find("Head"));
	}
	void OnGUI() {
		if (isRunning) {
			if (button(-5, 5, 10, 5, "||")) {
				setState(false);
			}
		} else {
			if (button(25, 10, 50, 20, "RESUME")) {
				setState(true);
			}
			if (button(25, 40, 50, 20, "MAIN MENU")) {
				setState(true);
				Application.LoadLevel("Menu");
			}
			if (button(25, 70, 50, 20, "QUIT")) {
				setState(true);
				Application.Quit();
			}
		}
	}
}
