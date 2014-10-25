using UnityEngine;
using System.Collections;

/** Translates Input to special car moves (singletone) */
public class InputControl : ScreenGUI {
	public float acceleration = 2;
	public float accelerator = 1, breaks = 1;
	
	static InputControl singletone;
	bool isAccelerator, isBreaks;
	float valueLook;
	
	void Start () { // registr singletone
		if (singletone != null)
			throw new UnityException (
				"Singleton 'Manipulator' had already bean set");
		singletone = this;
	}
	void OnGUI() {
		if (GameControl.isRunning) {
			isBreaks = repeatButton(5, 80, 25, 15, "BREAKS");
			isAccelerator = repeatButton(-5, 80, 25, 15, "ACCELERATOR");
			valueLook = scroll(20, 5, 60, 10, valueLook, -1, 1);
		}
	}

	// Interface, to be used like standart Input
	
	static public float GetStearing() {
		if (singletone == null) return 0;

		var accel = Input.acceleration.x * singletone.acceleration;
		var keybd = Input.GetAxis ("Horizontal");
		return accel + keybd;
	}
	static public float GetPedals() {
		if (singletone == null) return 0;

		float buttons = 0;
		if (singletone.isAccelerator) buttons += singletone.accelerator;
		if (singletone.isBreaks) buttons -= singletone.breaks;

		var keybd = Input.GetAxis ("Vertical");
		return buttons + keybd;
	}
	static public float GetLook() {
		if (singletone == null) return 0;
		return singletone.valueLook;
	}
}
