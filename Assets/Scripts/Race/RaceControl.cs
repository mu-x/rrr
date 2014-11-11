using ScreenMap;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Controls game processes */
public class RaceControl : MonoBehaviour {
    public Player player;
    public AudioClip signalPending;
    public AudioClip signalStart;

    void Start() {
        // DEBUG ONLY!!
        if (IntraControl.selectedCarModel == null)
            IntraControl.selectedCarModel = Resources.Load<GameObject>(
                "Car Models/Kopeck L2101");
        if (IntraControl.selectedRaceMode == null)
            IntraControl.selectedRaceMode = new CheckpointChaseRaceMode();

        signalPending = signalPending ?? Resources.Load<AudioClip>("Xylo");
        signalStart = signalStart ?? Resources.Load<AudioClip>("Car Skid");

        raceMode = IntraControl.selectedRaceMode;
        raceMode.Prepare(
            GetComponentInChildren<Player>(),
            IntraControl.selectedCarModel,
            GetComponentInChildren<Checkpoints>().Get(),
            delegate(string message) {
                isRunning = false;
                endOfTheGame = message;
            }
        );

        isRunning = true;
        StartCoroutine(StartRace());
    }

    IEnumerator StartRace() {
        player.isEnabled = false;
        gameDalay = COUNT_DOWN.Length;
        var position = Camera.main.transform.position;

        do {
            AudioSource.PlayClipAtPoint(signalPending, position);
            yield return new WaitForSeconds(1);
        } while (--gameDalay != 0) ;

        // Start race after a short dalay
        AudioSource.PlayClipAtPoint(signalStart, position);
        player.isEnabled = true;
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
                Camera.main.audio.Pause();
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
            Time.timeScale = value ? 1 : 0;

            if (gameDalay == 0)
                GetComponentInChildren<Player>().isEnabled = value;

            if (!value) { // Stop all sounds when paused
                var type = typeof(AudioSource);
                foreach (var audio in FindObjectsOfType(type) as AudioSource[])
                    audio.Pause();

                Camera.main.audio.Play();
            } else {
                Camera.main.audio.Pause();
            }
        }
        get { return !Time.timeScale.Equals(0); }
    }

    static string[] COUNT_DOWN = new string[] { "GO!", "STADY", "READY" };

    RaceMode raceMode;
    int gameDalay;
    string endOfTheGame;
}
