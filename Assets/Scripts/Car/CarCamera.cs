using System.Collections;
using UnityEngine;

/** Makes camera effects to simulate driver expirience */
public class CarCamera : MonoBehaviour {
    public Transform watchPoint; /**< Car to follow */
    public float viewLook; /**< Driver head rotation */
    public Vector3 backShift = new Vector3(0, 1, 5);

    public enum ViewMode : int { DRIVER, BACK };
    public ViewMode viewMode;

    void Update () {
        if (watchPoint == null)
            return;

        transform.eulerAngles = watchPoint.eulerAngles;
        transform.position = watchPoint.renderer.bounds.center;

        /* FIXME: replace obsolete 'input'

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
        */

        switch (viewMode) {
            case ViewMode.DRIVER:
                transform.eulerAngles += new Vector3(0, viewLook, 0);
                break;

            case ViewMode.BACK:
                transform.position += new Vector3(0, backShift.y, 0);
                transform.position += transform.forward * -backShift.z;
                break;
        }
    }
}