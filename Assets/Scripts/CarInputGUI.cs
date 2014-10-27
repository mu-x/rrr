using ScreenMap;
using System.Collections;
using UnityEngine;

/** Translates Input to special car moves (singletone) */
public class CarInputGUI : MonoBehaviour, ICarInput {
	public bool isEnabled = true;

	void OnGUI() {
        if (isEnabled) {
            var map = this.ScreenRect();
            GUI.skin.button.fontSize = 15;

			isBreak = GUI.RepeatButton(map.X(1, 4).Y(4, 4), "BREAKS");
            isGas = GUI.RepeatButton(map.X(-1, 4).Y(4, 4), "ACCELERATOR");
            valueLook = GUI.HorizontalSlider(map.X(2, 5, 3).Y(1, 4), valueLook, -1, +1);
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

	public float GetLook() { 
        return valueLook * PlayerPrefs.GetInt("input_look", 2);
    }

	bool isGas, isBreak;
	float valueLook;
}
