using UnityEngine;
using System.Collections;
using UnityExtensions;

public class CarControl : MonoBehaviour 
{
    public GameObject wheelModel;

    public float frontDrive = 0, rearDrive = 64;
    public float weight = 950;
    public float frontBreak = 40, rearBreak = 15;

    public float stearing;
    public bool gas, breaks = true;

    public int price
    { get {
        return Mathf.RoundToInt((frontDrive + rearDrive) * 2 + 
            (weight / 10) + (frontBreak + rearBreak) / 3);
    } }

    public string info
    { get {
        var d = (frontDrive == 0) ? "R" : ((rearDrive == 0) ? "F" : "X");
        return string.Format(
            "Engine: {0} hp({1}) | Weight: {2} kg | Breaks: {3}/{4}",
            frontDrive + rearDrive, d, weight, frontBreak, rearBreak);
    } }

    public void CaptureCamera()
    {
        var car = gameObject.HasComponent<CarModel>();
        var camera = Camera.main.transform;
        camera.parent = car.head;
        camera.position = car.head.position;
        camera.rotation = car.transform.rotation;
    }

	void Start() 
    {
        gameObject.HasComponent<CarModel>().wheelModel = wheelModel;
        gameObject.HasComponent<BoxCollider>();

        var rb = gameObject.HasComponent<Rigidbody>();
        rb.drag = (rb.mass = weight) / 5000;
	}

    void FixedUpdate()
    {
        var car = GetComponent<CarModel>();
        car.stearAngle = stearing;

        var k = 0.5f / (weight / 1000f);
        car.frontTorque = (gas ? 1 : 0) * frontDrive * k;
        car.rearTorque = (gas ? 1 : 0) * rearDrive * k;
        car.frontBreak = (breaks ? 1 : 0) * frontBreak * k;
        car.rearBreak = (breaks ? 1 : 0) * rearBreak * k;
	}
}
