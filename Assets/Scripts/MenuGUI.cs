using UnityEngine;
using System.Collections;

/** Car & track selection Menu */
public class MenuGUI : ScreenGUI {

	public Transform carSpot;
	public GameObject[] carList;

	// Keeps last selected prefab avaliable for Game
	public static GameObject currentCar;

	GameObject currentCarObject;
	int selectedCar = 0;
	
	void Start() {
		instantiateCar();
	}		
	void OnGUI () {
		if (button(5, 35, 5, 30, "<<")) {
			if (--selectedCar < 0) 
				selectedCar = carList.Length - 1;
			instantiateCar();
		}
		if (button(-5, 35, 5, 30, ">>")) {
			if (++selectedCar > carList.Length - 1) 
				selectedCar = 0;
			instantiateCar();
		}
		if (button(35, -5, 30, 10, "START GAME")) {
			Application.LoadLevel("ForestRing");
		}
	}
	void instantiateCar() {
		currentCar = carList[selectedCar];

		// Replace old one with new one
		Destroy(currentCarObject);
		currentCarObject = Instantiate(
			currentCar, carSpot.position, carSpot.rotation) as GameObject;

	}
}
