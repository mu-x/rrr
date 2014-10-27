using System;
using System.Collections;
using UnityEngine;
using ScreenMap;

/** Controls game processes */
public class RaceControl : MonoBehaviour {
	public GameObject carModel;
	public Transform carSpot;
	public CarInputGUI carInput; /**< is here to be provided to car */
	public CarCamera carCamera; /**< is here to receive a new car */
	public Transform[] mapRoute; 

	public static GameObject carModelPreset; /** one time @var carModel replacer */

	public bool isRunning {
        set {
            carInput.isEnabled = value;
            Time.timeScale = value ? 1 : 0;
        }
        get { return Time.timeScale != 0; }
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
        var top = this.ScreenRect().Y(1, 8);
        GUI.skin.box.fontSize = 20;
        GUI.skin.button.fontSize = 20;

        GUI.Box(top.X(1, 5), playTime.ToString("0.00"));
        if (isRunning) {
            if (GUI.Button(top.X(-1, 5), "PAUSE"))
                isRunning = false;
            return;
        }

        var menu = this.ScreenRect().X(2, 4, 2);
		if (endOfTheGame == null) {
            if (GUI.Button(menu.Y(1, 3), "RESUME"))
                isRunning = true;
        } else {
            GUI.Button(menu.Y(1, 3), endOfTheGame);
        }
        if (GUI.Button(menu.Y(2, 3),  "RESTART")) {
            carModelPreset = carModel;
            isRunning = true;
            Application.LoadLevel(Application.loadedLevel);
        }
        if (GUI.Button(menu.Y(-1, 3).X(1, 2, border: 0), "MAIN MENU")) {
            isRunning = true;
            Application.LoadLevel("MainMenu");
        }
        if (GUI.Button(menu.Y(-1, 3).X(-1, 2, border: 0), "QUIT")) {
            isRunning = true;
            Application.Quit();
        }
	}

	float playTime;
	string endOfTheGame;
}
