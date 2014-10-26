using UnityEngine;
using System.Collections;

/** Car & track selection Menu */
public class IntraControl : ScreenGUI {
	public Transform carSpot;
	public GameObject[] carList;
	public int carSelected = 0; // points to carList
	
	void Start() {
		instantiateCar();
	}		
	
	void OnGUI () {
		if (button(5, 35, 5, 30, "<<")) {
			if (--carSelected < 0) 
				carSelected = carList.Length - 1;
			instantiateCar();
		}
		if (button(-5, 35, 5, 30, ">>")) {
			if (++carSelected > carList.Length - 1) 
				carSelected = 0;
			instantiateCar();
		}
		if (button(35, -5, 30, 10, "START GAME")) {
			RaceControl.carModelPreset = carList[carSelected];
			Application.LoadLevel("ForestRing");
		}
	}

	void instantiateCar() {
		if (currentCarObject != null)
			Destroy(currentCarObject);
		currentCarObject = Instantiate(
			carList[carSelected], carSpot.position, carSpot.rotation) 
			as GameObject;
	}

	GameObject currentCarObject;
}
