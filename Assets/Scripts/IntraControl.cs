using ScreenMap;
using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour {
	public Transform carSpot;
	public GameObject[] carList;
	public string[] levelNames;

    public static GameObject selectedCarModel;
    public static RaceMode selectedRaceMode;

    /** Initializes selector controls */
	void Start() {
        carSelector = new Selector< GameObject >(
            carList, "CarModel",
            delegate (GameObject model) {
                selectedCarModel = model;
                if (carObject != null)
                    Destroy(carObject);

                carObject = Instantiate(model, carSpot.position, carSpot.rotation)
                    as GameObject;
            });

        var raceModes = new RaceMode[] {
            new FreeRaceMode(), /* Default */
            new CheckpointRaceMode(1), new CheckpointRaceMode(10)
        };

        modeSelector = new Selector<RaceMode>(
            raceModes, "RaceMode",
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

        var middle = this.ScreenRect().Y(2, 4);
        GUI.skin.button.fontSize = (int)middle.height / 2;
        if (GUI.Button(middle.X(1, 7), "<")) carSelector.Previous();
        if (GUI.Button(middle.X(-1, 7), ">")) carSelector.Next();

        var lower = this.ScreenRect().Y(-2, 5, 2);
        var bottom = lower.Y(-1, 3).X(2, 4, 2, border: 0);
        GUI.skin.box.fontSize =
        GUI.skin.button.fontSize = (int)(bottom.height / 3);

        GUI.Box(lower.X(1, 4), selectedCarModel.GetComponent<CarControl>().info);
        if (GUI.Button(lower.X(-1, 4), "START\nGAME"))
			Application.LoadLevel(levelNames[0]);

        if (GUI.Button(bottom.X(1, 5, border: 0), "<")) modeSelector.Previous();
        GUI.Box(bottom.X(2, 5, 3, border: 0), selectedRaceMode.info);
        if (GUI.Button(bottom.X(-1, 5, border: 0), ">")) modeSelector.Next();
	}

    ISelector carSelector;
    string carInfo;
    GameObject carObject;
    ISelector modeSelector;
}
