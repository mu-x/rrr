using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour
{
    public GameObject[] carModels;
    public string[] levelNames = new[] { "Long Road", "Forest Ring" };

    public static GameObject selectedCarModel;
    public static GameObject[] oponentCarModels;
    public static RaceMode selectedRaceMode;
    public static string selectedTrack;

    bool enter = true;
    ISelector carSelector, modeSelector, trackSelector;
    GameObject carObject;

    /** @addtgoup MonoBehaviour
     *  @{ */

    /** Initializes selector controls */
    void Start()
    {
        oponentCarModels = carModels;
        var respawn = transform.FindChild("Respawn");
        respawn.renderer.enabled = false;

        carSelector = new Selector< GameObject >(
            carModels, "Car Model",
            delegate (GameObject model)
            {
                selectedCarModel = model;
                if (carObject != null)
                    Destroy(carObject);

                carObject = (GameObject)Instantiate(model,
                    respawn.position, respawn.rotation);
            });

        trackSelector = new Selector<string>(
            levelNames, "Track", t => selectedTrack = t);

        var RACE_MODES = new RaceMode[]
        {
            new RaceModeAI(1, 5), new RaceModeAI(3, 1),
            new RaceMode(),
            new RaceModeCC(1), new RaceModeCC(5)
        };

        modeSelector = new Selector<RaceMode>(
            RACE_MODES, "Race Mode", m => selectedRaceMode = m);
    }

    /** Draws menu selectors and 'start game' button */
    void OnGUI ()
    {
        var gui = new ExtraGUI(Color.white);
        var rrr = new ExtraGUI(Color.red);
        rrr.Label(10, 5, 80, 25, "Real Russian Racing");

        if (enter)
        {
            if (gui.Button(10, 85, 80, 10, "... touch to continue ..."))
            {
                enter = false;
                Camera.main.transform.position += Vector3.down;
                Camera.main.transform.LookAt(transform.FindChild("Respawn"));
            }
        }
        else
        {
            var car = (ICarModel)selectedCarModel.GetComponent<CarModel>();
            var info = string.Format("{0}\n-\n{1}", car.model, car.details);
            gui.Selection(2, 30, 96, 20, null, carSelector);
            gui.Box(2, 60, 16, 38, info);

            gui.Selection(25, 80, 50, 8, selectedTrack, trackSelector);
            gui.Selection(25, 90, 50, 8, selectedRaceMode.info, modeSelector);
            if (gui.Button(82, 60, 16, 38, "START\nGAME"))
                Application.LoadLevel(selectedTrack);
        }
    }

    /** @} */
}
