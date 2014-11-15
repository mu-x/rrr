using ScreenMap;
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
            Input.GetAxis("Horizontal") * 25 +
            Input.acceleration.x * PlayerPrefs.GetInt("input_accelerate", 50);

        car.pedals =
            Input.GetAxis("Vertical") +
            (isGas ? 1 : 0) + (isBreak ? -1 : 0);

        car.driverHead.localEulerAngles = new Vector3(0,
            car.stearing / 5, -car.stearing);
    }

    void OnGUI()
    {
        if (!approved) return;

        var map = this.ScreenRect().Y(-1, 5);
        GUI.skin.box.fontSize =
        GUI.skin.button.fontSize = 15;

        isBreak = GUI.RepeatButton(map.X(1, 4), "BREAKS");
        isGas = GUI.RepeatButton(map.X(-1, 4), "ACCELERATOR");

        var modes = Enum.GetNames(typeof(CarCamera.ViewMode));
        carCamera.viewMode = (CarCamera.ViewMode)GUI.SelectionGrid(
            this.ScreenRect().Y(1, 6f).X(-2, 9, 2).Y(-1, 2, 1, 0),
            (int)carCamera.viewMode, modes, modes.Length);

        GUI.Box(map.X(3, 5).Y(-1, 2), string.Format("{0}, {1} km/h, {2} %",
            (car.gear < 0) ? "R" : "G" + car.gear,
            (int)Mathf.Abs(car.speed / 100f), (int)car.health));
    }
}
