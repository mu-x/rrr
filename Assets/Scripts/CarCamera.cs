using System.Collections;
using UnityEngine;

/** Makes camera effects to simulate driver expirience */
public class CarCamera : MonoBehaviour {
	public float xMove = 1, zMove = 1;
	public float angleMove = 10, angleRot = 15, angleLook = 15;
	public CarControl carToWatch; /**< car to follow by default */

	void Update () {
		if (carToWatch == null) return;
		var point = carToWatch.watchPoint.transform.position;
		var rotor = carToWatch.watchPoint.transform.eulerAngles;
		var input = carToWatch.control.input;

		// Move camera when wheel or pedals are affected
		var x = point.x + input.GetStearing() * (xMove / 100);
		var z = point.z - input.GetPedals() * (zMove / 100);

		// Rotate camera 
		var ay = rotor.y - (input.GetStearing() * -angleMove) 
			             - (input.GetLook() * -angleLook);
		var az = rotor.z - input.GetStearing() * angleRot;

		transform.position = new Vector3(x, point.y, z);
		transform.eulerAngles = new Vector3(rotor.x, ay, az);
	}
}