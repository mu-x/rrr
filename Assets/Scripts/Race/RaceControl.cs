using ScreenMap;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Controls game processes */
public class RaceControl : MonoBehaviour
{
    public AudioClip signalPending;
    public AudioClip signalStart;
    public GameObject debugCarModel;

    static string[] COUNT_DOWN = new string[] { "GO!", "STADY", "READY" };

    RaceMode raceMode;
    int gameDalay;
    string endOfTheGame;

    public bool isRunning
    { set {
        Time.timeScale = value ? 1 : 0;
        if (gameDalay == 0)
            raceMode.approved = value;

        if (!value)
        {
            Array.ForEach(FindObjectsOfType<AudioSource>(), s => s.Pause());
            Camera.main.audio.Play();
        }
        else Camera.main.audio.Pause();
    } get {
        return !Time.timeScale.Equals(0);
    } }

    /** @addtgoup MonoBehaviour
     *  @{ */

    void Awake()
    {
        raceMode = IntraControl.selectedRaceMode ?? new RaceModeAI(1, 6); // DBG
        raceMode.Prepare(
            IntraControl.selectedCarModel ?? debugCarModel, // DBG
            delegate(string message)
            {
                isRunning = false;
                endOfTheGame = message;
            });

        isRunning = true;
        StartCoroutine(StartRace());
    }

    IEnumerator StartRace()
    {
        raceMode.approved = false;
        gameDalay = COUNT_DOWN.Length;

        var camera = Camera.main;
        do
        {
            AudioSource.PlayClipAtPoint(signalPending, camera.transform.position);
            yield return new WaitForSeconds(1);
        }
        while (--gameDalay != 0) ;

        // Start race after a short dalay
        AudioSource.PlayClipAtPoint(signalStart, camera.transform.position);
        raceMode.approved = true;
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

    /** @} */

    void MenuGUI() {
        var menu = this.ScreenRect().X(2, 4, 2);
        GUI.skin.button.fontSize = (int)menu.Y(1, 10).height;

        if (endOfTheGame == null)
        {
            if (GUI.Button(menu.Y(1, 3), "RETURN TO GAME"))
            {
                isRunning = true;
                Camera.main.audio.Pause();
            }
        }
        else
        {
            GUI.Button(menu.Y(1, 3), endOfTheGame);
        }
        if (GUI.Button(menu.Y(2, 3), "RESTART RACE"))
        {
            isRunning = true;
            Application.LoadLevel(Application.loadedLevel);
        }
        if (GUI.Button(menu.Y(-1, 3), "MAIN MENU"))
        {
            isRunning = true;
            Application.LoadLevel(0);
        }
    }
}
