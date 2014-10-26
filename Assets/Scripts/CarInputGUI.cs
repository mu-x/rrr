using UnityEngine;
using System.Collections;

/** Translates Input to special car moves (singletone) */
public class CarInputGUI : ScreenGUI, ICarInput {
	public bool isEnabled = true;

	void OnGUI() {
		if (isEnabled) {
			isBreak = repeatButton(5, 80, 25, 15, "BREAKS");
			isGas = repeatButton(-5, 80, 25, 15, "ACCELERATOR");
			valueLook = scroll(20, 5, 60, 10, valueLook, -1, 1);
		}
	}
	
	public float GetStearing() {
		return Input.GetAxis("Horizontal") + Input.acceleration.x *
			PlayerPrefs.GetInt("input_accelerate", 2);
	}

	public float GetPedals() {
		return Input.GetAxis ("Vertical") + 
			(isGas ? 1 : 0) + (isBreak ? -1 : 0);
	}

	public float GetLook() { return valueLook; }

	bool isGas, isBreak;
	float valueLook;
}
