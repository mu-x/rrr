using UnityEngine;
using System.Collections;

/** Controls game processes */
public class RaceControl : ScreenGUI {
	public GameObject carModel;
	public Transform carSpot;
	public CarInputGUI carInput; // is here to be provided to car
	public CarCamera carCamera; // is here to receive a new car
	public Transform[] mapRoute; 

	public static GameObject carModelPreset; // one time carModel replace
	public static bool isRunning { // public?
		get { return Time.timeScale != 0; }
		set { Time.timeScale = value ? 1 : 0; }
	}
	
	void Start () {
		if (carModelPreset != null) {
			carModel = carModelPreset;
			carModelPreset = null;
		}
		
		isRunning = false;
		var car = Instantiate(carModel, carSpot.position, carSpot.rotation);
		var userCar = (car as GameObject).GetComponent<CarControl>();

		userCar.control.input = carInput;
		carCamera.carToWatch = userCar;
		isRunning = true;
	}

	void FixedUpdate() {
		playTime += Time.deltaTime;
	}

	void OnGUI() {
		if (isRunning) {
			if (button(-5, 5, 10, 5, "||"))
				isRunning = false;
		} else {
			if (endOfTheGame == null)
				pauseMenu();
			else
				endMenu();
		}
		button(5, 5, 10, 5, playTime.ToString());
	}
	
	void pauseMenu() {
		if (button(25, 10, 50, 20, "RESUME"))
			isRunning = true;
		quitMenu();
	}

	void endMenu() {
		button(25, 10, 50, 20, endOfTheGame);
		quitMenu();
	}

	void quitMenu() {
		if (button(25, 40, 50, 20, "RESTART")) {
			carModelPreset = carModel;
			isRunning = true;
			Application.LoadLevel(Application.loadedLevel);
		}
		if (button(25, 70, 25, 20, "MAIN MENU")) {
			isRunning = true;
			Application.LoadLevel("MainMenu");
		}
		if (button(55, 70, 20, 20, "QUIT")) {
			isRunning = true;
			Application.Quit();
		}
	}

	float playTime;
	string endOfTheGame;
}
