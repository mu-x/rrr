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
        rrr.Label(5, 0, 90, 25, "Real Russian Racing");

        var gui = new ExtraGUI(Color.white, font);
        MoveCamera(page);
        switch (page)
        {
            case Page.INTRA:
                if (gui.Button(30, -2, 40, 10, "... touch to continue ..."))
                    page = Page.CAR;
                break;

            case Page.CAR:
                if (gui.Button(2, 2, 15, 10, "Options")) page = Page.OPTIONS;
                if (gui.Button(-2, 2, 15, 10, "Back")) page = Page.INTRA;

                var car = (ICarModel)playerModel.GetComponent<CarModel>();
                var info = string.Format("{0}\n-\n{1}", car.model, car.details);
                gui.Selection(2, 30, 96, 20, null, carSelector, 10);
                gui.Box(2, 60, 20, 38, info);

                if (gui.Button(30, -2, 40, 10, "Select Race"))
                    page = Page.RACE;
                break;

            case Page.RACE:
                if (gui.Button(2, 2, 15, 10, "Options")) page = Page.OPTIONS;
                if (gui.Button(-2, 2, 15, 10, "Back")) page = Page.CAR;

                gui.Selection(2, 62, 55, 10, selectedTrack, trackSelector);
                gui.Selection(2, 73, 55, 10, raceMode.info, modeSelector);

                if (gui.Button(30, -2, 40, 10, "Start Race"))
                    Application.LoadLevel(selectedTrack);
                break;

            case Page.OPTIONS:
                if (gui.Button(2, 2, 15, 10, "Intra")) page = Page.INTRA;
                if (gui.Button(-2, 2, 15, 10, "Back")) page = Page.CAR;

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
