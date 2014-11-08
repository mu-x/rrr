using System.Collections;
using UnityEngine;

/** Makes camera effects to simulate driver expirience */
public class CarCamera : MonoBehaviour {
	public float xMove = 1, zMove = 1;
	public float angleMove = 10, angleRot = 15, angleLook = 15;
    public float yBack = 1, rBack = 5;

	public Transform watchPoint; /**< car to follow by default */
    public ICarInput input; /**< input to correct camera position */

    public enum ViewMode : int { DRIVER, BACK };
    public ViewMode viewMode;

	void Update () {
        if (watchPoint == null)
            return;

        transform.eulerAngles = watchPoint.eulerAngles;
        transform.position = watchPoint.position;

        if (input != null) {
            // Move camera when wheel or pedals are affected
            transform.position += new Vector3(
                input.stearing * (xMove / 100), 0,
                input.pedals * -(zMove / 100));

            // Rotate camera
            transform.eulerAngles += new Vector3(0,
                input.stearing * angleMove + input.look * angleLook,
                input.stearing * -angleRot);
        }

        switch (viewMode) {
            case ViewMode.DRIVER:
                break;

            case ViewMode.BACK:
                transform.position += new Vector3(0, yBack, 0);
                transform.position += transform.forward * -rBack;
                break;
        }

	}
}