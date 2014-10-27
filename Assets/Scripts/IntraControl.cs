using ScreenMap;
using System.Collections;
using UnityEngine;

/** Car & track selection menu */
public class IntraControl : MonoBehaviour {
	public Transform carSpot;
	public GameObject[] carList;

	void Start() {
        carSelector = new Selector< GameObject >(carList,
                                                 delegate (GameObject model) {
            RaceControl.carModelPreset = model;
            if (carObject != null) Destroy(carObject);
            carObject = Instantiate(model, carSpot.position, carSpot.rotation) 
                as GameObject;
        });
	}		
	
	void OnGUI () {
        var map = this.ScreenRect();
        GUI.skin.button.fontSize = 25;

		if (GUI.Button(map.X(1, 7).Y(2, 3), "<<"))
            carSelector.previous();
        if (GUI.Button(map.X(-1, 7).Y(2, 3), ">>"))
            carSelector.previous();
        if (GUI.Button(map.X(2, 3).Y(-1, 5), "START GAME"))
			Application.LoadLevel("ForestRing");
	}
    
    ISelector carSelector;
    GameObject carObject;
}
