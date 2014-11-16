using ScreenMap;
using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour
{
    public GameObject[] carModels;
    public string[] levelNames = new[] { "Long Road", "Forest Ring" };

    public static GameObject selectedCarModel;
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
    void OnGUI () {
        var top = this.ScreenRect().Y(1, 4);
        GUI.skin.label.fontSize = (int)(top.height / 1.5f);
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.skin.label.normal.textColor = Color.red;
        GUI.Label(top, "Real Russian Racing");

        if (enter) {
            var bottom = this.ScreenRect().Y(-1, 7).X(2, 5, 3);
            GUI.skin.button.fontSize = (int)bottom.height / 2;
            if (GUI.Button(bottom, "... touch to continue ...")) {
                enter = false;
                Camera.main.transform.position += Vector3.down;
                Camera.main.transform.LookAt(transform.FindChild("Respawn"));
            }
        } else {
            var middle = this.ScreenRect().Y(2, 4);
            GUI.skin.button.fontSize = (int)middle.height / 2;
            if (GUI.Button(middle.X(1, 7), "<")) carSelector.Previous();
            if (GUI.Button(middle.X(-1, 7), ">")) carSelector.Next();

            var lower = this.ScreenRect().Y(-2, 5, 2);
            var bottom1 = lower.Y(-2, 3).X(2, 4, 2, border: 0);
            var bottom2 = lower.Y(-1, 3).X(2, 4, 2, border: 0);
            GUI.skin.box.fontSize =
            GUI.skin.button.fontSize = (int)(bottom2.height / 2.5f);

            var car = (ICarModel)selectedCarModel.GetComponent<CarModel>();
            GUI.Box(lower.X(1, 4), string.Format("{0}\n-\n{1}",
                car.model, car.details));
            if (GUI.Button(lower.X(-1, 4), "START\nGAME"))
                Application.LoadLevel(selectedTrack);

            if (GUI.Button(bottom1.X(1, 5, border: 0), "<"))
                trackSelector.Previous();
            if (GUI.Button(bottom1.X(-1, 5, border: 0), ">"))
                trackSelector.Next();
            GUI.Box(bottom1.X(2, 5, 3, border: 0), selectedTrack);

            if (GUI.Button(bottom2.X(1, 5, border: 0), "<"))
                modeSelector.Previous();
            if (GUI.Button(bottom2.X(-1, 5, border: 0), ">"))
                modeSelector.Next();
            GUI.Box(bottom2.X(2, 5, 3, border: 0), selectedRaceMode.info);
        }
    }

    /** @} */
}
