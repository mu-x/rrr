using ScreenMap;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Controls game processes */
public class RaceControl : MonoBehaviour {
    public Transform[] carSpots;
    public CarCamera carCamera;
    public AudioClip pendingSound;
    public AudioClip startSound;

    void Start () {
        var playerModel = IntraControl.selectedCarModel;
        var route = transform.GetComponentsInChildren<CheckPoint>();

        var mapRoute = new List<GameObject>();
        foreach(var cp in route)
            mapRoute.Add(cp.gameObject);

        RaceMode.EndGame end = delegate (string message) {
            isRunning = false;
            endOfTheGame = message;
        };

        raceMode = IntraControl.selectedRaceMode;
        var car = raceMode.Prepare(carSpots, mapRoute.ToArray(), playerModel, end);

        carCamera.watchPoint = car.watchPoint.transform;
        carCamera.input = GetComponent<CarInputGUI>();

        isRunning = true;
        StartCoroutine(StartRace());
    }

    IEnumerator StartRace() {
        gameDalay = COUNT_DOWN.Length;

        do {
            AudioSource.PlayClipAtPoint(pendingSound,
                                        carCamera.transform.position);
            yield return new WaitForSeconds(1);
        } while (--gameDalay != 0) ;

        // Start race after a short dalay
        AudioSource.PlayClipAtPoint(startSound, carCamera.transform.position);
        raceMode.Race(GetComponent<CarInputGUI>());
    }

    void FixedUpdate() {
        // Game starts after short dalay
        if (gameDalay != 0)
            return;

        raceMode.FixedUpdate();
    }

    void OnGUI() {
        var top = this.ScreenRect().Y(1, 6f);
        GUI.skin.button.fontSize =
        GUI.skin.box.fontSize = (int)(top.height / 4);

        GUI.Box(top.X(1, 9, 2), raceMode.status);
        if (isRunning) {
            if (GUI.Button(top.X(-2, 9, 2).Y(1, 2, 1, 0), "PAUSE")) {
                isRunning = false;
            }

            int mode = (int) carCamera.viewMode;
            var modeList = new[] {
                CarCamera.ViewMode.DRIVER.ToString(),
                CarCamera.ViewMode.BACK.ToString(),
            };

            var rect = top.X(-2, 9, 2).Y(-1, 2, 1, 0);
            mode = GUI.SelectionGrid(rect, mode, modeList, modeList.Length);
            carCamera.viewMode = (CarCamera.ViewMode) mode;
        } else {
            MenuGUI();
        }

        if (gameDalay == 0)
            return;

        var counter = this.ScreenRect().X(2, 4, 2).Y(2, 3);
        GUI.skin.label.normal.textColor = Color.red;
        GUI.skin.label.fontSize = (int)top.height;
        GUI.Label(counter, COUNT_DOWN[gameDalay - 1]);
    }

    void MenuGUI() {
        var menu = this.ScreenRect().X(2, 4, 2);
        GUI.skin.button.fontSize = (int)menu.Y(1, 10).height;

        if (endOfTheGame == null) {
            if (GUI.Button(menu.Y(1, 3), "RETURN TO GAME")) {
                isRunning = true;
                carCamera.audio.Pause();
            }
        } else {
            GUI.Button(menu.Y(1, 3), endOfTheGame);
        }
        if (GUI.Button(menu.Y(2, 3), "RESTART RACE")) {
            isRunning = true;
            Application.LoadLevel(Application.loadedLevel);
        }
        if (GUI.Button(menu.Y(-1, 3), "MAIN MENU")) {
            isRunning = true;
            Application.LoadLevel(0);
        }
    }

    public bool isRunning {
        set {
            GetComponent<CarInputGUI>().isEnabled = value;
            Time.timeScale = value ? 1 : 0;

            if (!value) { // Stop all sounds when paused
                var type = typeof(AudioSource);
                foreach (var audio in FindObjectsOfType(type) as AudioSource[])
                    audio.Pause();

                carCamera.audio.Play();
            } else {
                carCamera.audio.Pause();
            }
        }
        get { return !Time.timeScale.Equals(0); }
    }

    static string[] COUNT_DOWN = new string[] { "GO!", "STADY", "READY" };

    RaceMode raceMode;
    int gameDalay;
    string endOfTheGame;
}

/** Controls part of the game process according to the race mode
 * (default: free race, inherit+override for other kind) */
public abstract class RaceMode {
    public delegate void EndGame(string message);
    public abstract string info { get; } /**< mode description */

    public virtual void FixedUpdate() {
        playTime += Time.deltaTime;
    }

    public virtual CarControl Prepare(Transform[] carSpots, GameObject[] mapRoute,
                                      GameObject playerModel, EndGame endGame) {
        playerCar = NewCar(playerModel, carSpots.Last());
        playerCar.type = CarControl.CarType.PLAYER;
        return playerCar;
    }

    public virtual void Race(ICarInput playerInput) {
        playerCar.control.input = playerInput;
    }

    public virtual string status {
        get {
            var min = (int)playTime / 60;
            var sec = playTime - min * 60f;
            return "Race Time: " + min + ":" + sec.ToString("00.00");
        }
    }

    protected CarControl NewCar(GameObject model, Transform spot) {
        var car = GameObject.Instantiate(model, spot.position, spot.rotation);
        return (car as GameObject).GetComponent<CarControl>();
    }

    protected float playTime;
    CarControl playerCar;
}
