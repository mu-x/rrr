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
    public GameObject[] carModels;

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
            IntraControl.selectedCarModel ?? carModels.First(), // DBG
            IntraControl.oponentCarModels ?? carModels,
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
        gameDalay = 3;

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

    void FixedUpdate()
    {
        raceMode.FixedUpdate();
        if (Input.GetKey(KeyCode.Escape))
            isRunning = false;
    }

    void OnGUI()
    {
        if (gameDalay != 0) // Ready, Stady, GO!
        {
            var COUNT_DOWN = new[]
            {
                new { color = Color.red, text = "READY" },
                new { color = Color.yellow, text = "STADY" },
                new { color = Color.green, text = "GO!" },
            };

            var options = COUNT_DOWN[COUNT_DOWN.Length - gameDalay];
            var count = new ExtraGUI(options.color);
            count.Label(30, 30, 40, 40, options.text);
        }

        var gui = new ExtraGUI(Color.white);
        if (gui.Button(-2, 2, 20, 15, raceMode.status))
            isRunning = false;

        if (isRunning) return; // else Menu:

        if (endOfTheGame == null)
        {
            if (gui.Button(30, 3, 40, 25, "RETURN TO GAME"))
            {
                isRunning = true;
                Camera.main.audio.Pause();
            }
        }
        else
        {
            gui.Box(30, 10, 40, 20, endOfTheGame);
        }
        if (gui.Button(30, 33, 40, 20, "RESTART RACE"))
        {
            isRunning = true;
            Application.LoadLevel(Application.loadedLevel);
        }
        if (gui.Button(30, 58, 40, 20, "MAIN MENU"))
        {
            isRunning = true;
            Application.LoadLevel(0);
        }
    }

    /** @} */
}
