using ScreenMap;
using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour {
	public Transform carSpot;
	public GameObject[] carList;

    RaceMode[] RACE_MODES = new RaceMode[] {
        new FreeRaceMode(), /* Default */
        new CheckpointRaceMode(1), new CheckpointRaceMode(10)
    };

	void Start() {
        carSelector = new Selector< GameObject >(carList, RaceControl.carModel,
                                                 delegate (GameObject model) {
            RaceControl.carModel = model;
            if (carObject != null) Destroy(carObject);
            carObject = CarControl.New(model, carSpot, null).gameObject;
        });

        modeSelector = new Selector< RaceMode >(RACE_MODES, RaceControl.raceMode,
                                                delegate (RaceMode mode) {
            RaceControl.raceMode = mode;
        });

	}		
	
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

        var lower = this.ScreenRect().Y(-2, 4, 2);
        var bottom = lower.Y(-1, 3).X(2, 4, 2, border: 0);
        GUI.skin.box.fontSize = 
        GUI.skin.button.fontSize = (int)bottom.height / 3;

        var car = RaceControl.carModel.GetComponent<CarControl>();
        GUI.Box(lower.X(1, 4), car.name + "\n-\n" + car.control);
        if (GUI.Button(lower.X(-1, 4), "START\nGAME"))
            Application.LoadLevel("ForestRing");

        if (GUI.Button(bottom.X(1, 5, border: 0), "<")) modeSelector.Previous();
        GUI.Box(bottom.X(2, 5, 3, border: 0), RaceControl.raceMode.ToString());
        if (GUI.Button(bottom.X(-1, 5, border: 0), ">")) modeSelector.Next();
	}
    
    ISelector carSelector;
    string carInfo;
    GameObject carObject;
    ISelector modeSelector;
}
