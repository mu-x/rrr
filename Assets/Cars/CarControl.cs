using UnityEngine;
using System.Collections;
using System.Linq;
using UnityExtensions;

/** The car behaviour controller */
[RequireComponent(typeof(CarModel))]
public class CarControl : MonoBehaviour 
{
    public CarModel model;
    public CarInfo info;

    public float stearing;
    public bool gas, breaks = true;

    /** Car behaviour paramiters */
    [System.Serializable]
    public class CarInfo 
    {
        public float frontDrive = 0, rearDrive = 64;
        public float weight = 950;
        public float frontBreak = 100, rearBreak = 25;

        public float centerMassY = 0.03f;

        public int price;
        public string description;
    }

    // TODO: look up for better editor event
    void OnDrawGizmos()
    {
        model = GetComponent<CarModel>();

        rigidbody.mass = info.weight;
        rigidbody.drag = info.weight / 10000;

        info.price = Mathf.RoundToInt((info.frontDrive + info.rearDrive) * 2 +
            (info.weight / 10) + (info.frontBreak + info.rearBreak) / 3);

        info.description = string.Format(
            "Engine: {0} hp({1}) | Weight: {2} kg | Breaks: {3}/{4}",
            info.frontDrive + info.rearDrive, 
            (info.frontDrive == 0) ? "R" : ((info.rearDrive == 0) ? "F" : "X"), 
            info.weight, info.frontBreak, info.rearBreak);
    }

    /** Binds camera to the driver's head */
    public void CaptureCamera()
    {
        var camera = Camera.main.transform;
        camera.parent = model.head;
        camera.position = model.head.position;
        camera.rotation = model.transform.rotation;
    }

    /** Applies @var stearing, @var gas and @var breaks */
    void FixedUpdate() 
    {
        rigidbody.centerOfMass = new Vector3(0, info.centerMassY, 0);

        var la = model.stearing.localEulerAngles;
        model.stearing.localEulerAngles = new Vector3(la.x, la.y, -stearing * 3);

        var k = 600f / info.weight;
        foreach (var wheel in model.wheels) {
            var collider = wheel.collider;
            if (wheel.isFront) {
                collider.motorTorque = (gas ? 1 : 0) * info.frontDrive * k;
                collider.brakeTorque = (breaks ? 1 : 0) * info.frontBreak * k;
                collider.steerAngle = stearing;
            } else {
                collider.motorTorque = (gas ? 1 : 0) * info.rearDrive * k;
                collider.brakeTorque = (breaks ? 1 : 0) * info.rearBreak * k;
            }
        }
	}
}
