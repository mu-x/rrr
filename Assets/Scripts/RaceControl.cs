using System;
using System.Collections;
using UnityEngine;
using ScreenMap;

/** Controls game processes */
public class RaceControl : MonoBehaviour {
    public Transform carSpot;
	public CarInputGUI carInput; /**< is here to be provided to car */
	public CarCamera carCamera; /**< is here to receive a new car */
    public GameObject debugCarModel;

    public static GameObject carModel; /**< Car selected by user */
    public static RaceMode raceMode;

	public bool isRunning {
        set {
            carInput.isEnabled = value;
            Time.timeScale = value ? 1 : 0;
        }
        get { return !Time.timeScale.Equals(0); }
	}
	
	void Start () {
        // Scene debug only!
        carModel = carModel ?? debugCarModel;
        raceMode = raceMode ?? new CheckpointRaceMode();

		carCamera.carToWatch = CarControl.New(carModel, carSpot, carInput,
                                              CarControl.CarType.PLAYER);
        raceMode.Start(transform.GetComponentsInChildren<CheckPoint>(),
                       delegate (string message) {
            isRunning = false;
            endOfTheGame = message;
        });
		isRunning = true;
	}

	void OnGUI() {
        var top = this.ScreenRect().Y(1, 7.5f);
        GUI.skin.button.fontSize = 
        GUI.skin.box.fontSize = (int)top.height / 3;

        GUI.Box(top.X(1, 5), raceMode.Status());
        if (isRunning) {
            if (GUI.Button(top.X(-1, 5), "PAUSE"))
                isRunning = false;
            return;
        }

        var menu = this.ScreenRect().X(2, 4, 2);
        GUI.skin.button.fontSize = (int)top.height;

		if (endOfTheGame == null) {
            if (GUI.Button(menu.Y(1, 3), "RETURN TO GAME"))
                isRunning = true;
        } else {
            GUI.Button(menu.Y(1, 3), endOfTheGame);
        }
        if (GUI.Button(menu.Y(2, 3), "RESTART RACE")) {
            isRunning = true;
            Application.LoadLevel(Application.loadedLevel);
        }
        if (GUI.Button(menu.Y(-1, 3), "MAIN MENU")) {
            isRunning = true;
            Application.LoadLevel("MainMenu");
        }
	}

    void FixedUpdate() { raceMode.FixedUpdate(); }
	string endOfTheGame;
}
