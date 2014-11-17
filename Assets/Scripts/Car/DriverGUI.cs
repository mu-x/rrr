using System.Collections;
using System;
using UnityEngine;

/** GUI controlled @calss Driver, configurates camera */
public class DriverGUI : Driver
{
    protected override bool marker { get { return true; } }
    protected CarCamera carCamera;
    bool isGas = false, isBreak = false;

    void Start()
    {
        base.Start();
        carCamera = Camera.main.GetComponent<CarCamera>();
        carCamera.watchPoint = car.driverHead;
    }

    void FixedUpdate()
    {
        if (!approved) return;

        car.stearing =
            (car.stearing * 10 + Input.GetAxis("Horizontal") * 25) / 11 +
            Input.acceleration.x * PlayerPrefs.GetFloat("input_tilt", 2.5f);

        car.pedals =
            Input.GetAxis("Vertical") +
            (isGas ? 1 : 0) + (isBreak ? -1 : 0);

        car.driverHead.localEulerAngles = new Vector3(0,
            car.stearing / 5, -car.stearing);
    }

    void OnGUI()
    {
        var gui = new ExtraGUI(Color.white);
        isBreak =  gui.Pusher(2, -2, 20, 25, "BREAK / REV");
        isGas = gui.Pusher(-2, -2, 20, 25, "ACCELERATOR");

        gui.Grid(-35, -3, 10, 12, ref carCamera.viewMode);
        gui.Box(-46, -3, 19, 12, string.Format("{0}, {1} km/h, {2} %",
            (car.gear < 0) ? "R" : "G" + car.gear,
            (int)Mathf.Abs(car.speed / 100f), (int)car.health));
    }
}
