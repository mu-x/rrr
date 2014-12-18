using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityExtensions;

[System.Serializable]
public class CarWheel
{
    public WheelCollider collider;
    public Transform model;
}

public class CarModel : MonoBehaviour 
{
    public GameObject wheelModel;

    public float stearAngle;
    public float frontTorque, rearTorque;
    public float frontBreak, rearBreak;

    public Transform stearing, head;
    public CarWheel[] frontWheels, rearWheels;

    public float speed;

	void Start ()
    {
        stearing = transform.Child("Stearing");
        head = transform.Child("Head");
        head.renderer.enabled = false;

        var front = new List<CarWheel>();
        var rear = new List<CarWheel>();
        var center = new Vector3();
        foreach (var wc in transform.Children("Wheel"))
        {
            wc.renderer.enabled = false;
            wc.rotation = transform.rotation;
            center += wc.localPosition;

            var wheel = new CarWheel();
            wheel.collider = wc.gameObject.AddComponent<WheelCollider>();
            wheel.model = gameObject.MakeChild(wc.name + ".Socket", wc).transform;

            var local = wheel.model.gameObject.MakeChild(wheelModel).transform;
            local.localScale *= wheel.collider.radius * 2;
            local.localEulerAngles = new Vector3(
                0, (wc.localPosition.x > 0) ? 180 : 0, 0);

            if (wc.localPosition.z > 0)
                front.Add(wheel); else rear.Add(wheel);
        }

        frontWheels = front.ToArray();
        rearWheels = rear.ToArray();
        gameObject.HasComponent<Rigidbody>().centerOfMass =
            center / (front.Count + rear.Count);

	}

    void FixedUpdate()
    {
        speed = (frontWheels.Average(w => w.collider.rpm * w.collider.radius) +
            rearWheels.Average(w => w.collider.rpm * w.collider.radius)) *
            Mathf.PI * 60f / 1000f;

        stearing.localEulerAngles = new Vector3(0, 0, -stearAngle * 3); 

        foreach (var w in frontWheels)
        {
            w.collider.motorTorque = frontTorque;
            w.collider.brakeTorque = frontBreak;
            w.collider.steerAngle = stearAngle;

            w.model.localEulerAngles = new Vector3(
                w.model.localEulerAngles.x, stearAngle, 0); 
            w.model.Rotate(w.collider.rpm / 60f * Time.deltaTime * 360f, 0, 0);
        }
        foreach (var w in rearWheels) 
        {
            w.collider.motorTorque = rearTorque;
            w.collider.brakeTorque = rearBreak;
            w.model.Rotate(w.collider.rpm / 60f * Time.deltaTime * 360f, 0, 0);
        }
    }
}
