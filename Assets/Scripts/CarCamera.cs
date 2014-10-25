using UnityEngine;
using System.Collections;

/** Makes camera effects to simulate driver expirience */
public class CarCamera : MonoBehaviour {
	public float xMove = 1, zMove = 1;
	public float angleMove = 10, angleRot = 15, angleLook = 15;
	
	static Transform watchPoint; // point to follow by default

	/** Sets point to follow, camera dont move othervice! */
	public static void SetWatchPoint(Transform point) {
		watchPoint = point;
	}
	void Update () {
		if (watchPoint == null) return;
		var point = watchPoint.position;
		var rotor = watchPoint.eulerAngles;

		// Move camera when wheel or pedals are affected
		var x = point.x + InputControl.GetStearing() * (xMove / 100);
		var z = point.z - InputControl.GetPedals () * (zMove / 100);

		// Rotate camera 
		var ays = InputControl.GetStearing() * -angleMove;
		var ayl = InputControl.GetLook() * -angleLook;
		var ay = rotor.y - ays - ayl;
		var az = rotor.z - InputControl.GetStearing() * angleRot;

		transform.position = new Vector3(x, point.y, z);
		transform.eulerAngles = new Vector3 (rotor.x, ay, az);
	}
}