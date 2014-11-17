using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour
{
    public GameObject[] carModels;
    public string[] levelNames = new[] { "Long Road", "Forest Ring" };
    public Font font;

    public static IntraControl main;
    public GameObject playerModel { get; set; }
    public RaceMode raceMode { get; set; }

    enum Page { INTRA, CAR, RACE, OPTIONS }
    Page page = Page.INTRA;
    string selectedTrack;
    ISelector carSelector, modeSelector, trackSelector;

    /** @addtgoup MonoBehaviour
     *  @{ */

    /** Initializes selector controls */
    void Start()
    {
        main = this;
        
        var respawn = transform.FindChild("Respawn");
        respawn.renderer.enabled = false;

        carSelector = new Selector< GameObject >(
            carModels, "Car Model",
            delegate (GameObject model)
            {
                if (playerModel != null)
                    DestroyImmediate(playerModel);

                playerModel = (GameObject)Instantiate(model,
                    respawn.position, respawn.rotation);

                DontDestroyOnLoad(playerModel);
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
            RACE_MODES, "Race Mode", m => raceMode = m);
    }

    /** Draws menu selectors and 'start game' button */
    void OnGUI () 
    {
        var rrr = new ExtraGUI(Color.red, font);
        rrr.Label(10, 10, 80, 25, "Real Russian Racing");

        var gui = new ExtraGUI(Color.white, font);
        gui.Grid(1, 1, 98, 10, ref page); 

        MoveCamera(page);
        switch (page)
        {
            case Page.INTRA:
                break;

            case Page.CAR:
                var car = (ICarModel)playerModel.GetComponent<CarModel>();
                var info = string.Format("{0}\n-\n{1}", car.model, car.details);
                gui.Selection(2, 30, 96, 20, null, carSelector, 10);
                gui.Box(2, 60, 25, 38, info);
                break;

            case Page.RACE:
                gui.Selection(2, 67, 55, 10, selectedTrack, trackSelector);
                gui.Selection(2, 78, 55, 10, raceMode.info, modeSelector);
                if (gui.Button(2, 89, 55, 10, "START RACE"))
                Application.LoadLevel(selectedTrack);
                break;

            case Page.OPTIONS:
                gui.SliderOption(2, 68, 45, 10, "Stear Tilt", 2.5f, 0, 5);
                gui.ToggleOption(2, 79, 45, 10, "Real Mode", false,  "ON / OFF");
                break;
        }
    }

    /** @} */

    void MoveCamera(Page page)
    {
        Transform target = transform.FindChild(page.ToString());
        Camera.main.transform.position = target.position;
        Camera.main.transform.rotation = target.rotation;
    }
}
