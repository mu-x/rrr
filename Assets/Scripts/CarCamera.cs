using System.Collections;
using UnityEngine;

/** Makes camera effects to simulate driver expirience */
public class CarCamera : MonoBehaviour {
	public float xMove = 1, zMove = 1;
	public float angleMove = 10, angleRot = 15, angleLook = 15;
	public CarControl carToWatch; /**< car to follow by default */

	void Update () {
		if (carToWatch == null) 
            return;
		
        var point = carToWatch.watchPoint.transform.position;
		var rotor = carToWatch.watchPoint.transform.eulerAngles;
		var input = carToWatch.control.input;

        if (input != null) {
    		// Move camera when wheel or pedals are affected
    		point.x += input.GetStearing() * (xMove / 100);
    		point.z -= input.GetPedals() * (zMove / 100);

    		// Rotate camera 
            rotor.y += input.GetStearing() * angleMove;
            rotor.y += input.GetLook() * angleLook;
    		rotor.z -= input.GetStearing() * angleRot;
        }

		transform.position = point;
		transform.eulerAngles = rotor;
	}
}