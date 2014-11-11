using ScreenMap;
using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour {
    public string[] levelNames = new[] { "Forest Ring" };

    public static GameObject selectedCarModel;
    public static RaceMode selectedRaceMode;

    /** Initializes selector controls */
    void Start() {
        carSelector = new Selector< GameObject >(
            Resources.LoadAll<GameObject>("Car Models"),
            "Car Model",
            delegate (GameObject model) {
                selectedCarModel = model;
                if (carObject != null)
                    Destroy(carObject);

                var r = transform.FindChild("Respawn");
                carObject = (GameObject)Instantiate(model, r.position, r.rotation);
            });

        var raceModes = new RaceMode[] {
            new RaceModeAI(1, 5), new RaceModeAI(3, 1),
            new RaceMode(),
            new RaceModeCC(1), new RaceModeCC(5)
        };

        modeSelector = new Selector<RaceMode>(
            raceModes, "race_mode",
            delegate (RaceMode mode) {
                selectedRaceMode = mode;
            });
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
            var bottom = lower.Y(-1, 3).X(2, 4, 2, border: 0);
            GUI.skin.box.fontSize =
            GUI.skin.button.fontSize = (int)(bottom.height / 3);

            GUI.Box(lower.X(1, 4), 
                selectedCarModel.GetComponent<CarModel>().info);
            if (GUI.Button(lower.X(-1, 4), "START\nGAME"))
                Application.LoadLevel(levelNames[0]);

            if (GUI.Button(bottom.X(1, 5, border: 0), "<")) 
                modeSelector.Previous();

            GUI.Box(bottom.X(2, 5, 3, border: 0), selectedRaceMode.info);
            if (GUI.Button(bottom.X(-1, 5, border: 0), ">")) 
                modeSelector.Next();
        }
    }

    bool enter = true;
    ISelector carSelector;
    string carInfo;
    GameObject carObject;
    ISelector modeSelector;
}
