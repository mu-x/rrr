using ScreenMap;
using System.Collections;
using System;
using UnityEngine;

/** Player controlled @calss Driver (GUI based) */
public class Player : Driver {
    public override void Prepare(GameObject carModel, GameObject[] route = null,
                                 int roundsExpected = 0, Action finish = null) {
        base.Prepare(carModel, route, roundsExpected, finish);
        Camera.main.GetComponent<CarCamera>().watchPoint = car.driverHead;
    }

    void FixedUpdate() {
        if (!car.isReady || !isEnabled)
            return;

        car.stearing =
            Input.GetAxis("Horizontal") * 25 +
            Input.acceleration.x * PlayerPrefs.GetInt("input_accelerate", 50);

        car.pedals =
            Input.GetAxis("Vertical") +
            (isGas ? 1 : 0) + (isBreak ? -1 : 0);
    }

    void OnGUI() {
        var map = this.ScreenRect();
        GUI.skin.button.fontSize = 15;

        isBreak = GUI.RepeatButton(map.X(1, 4).Y(-1, 4), "BREAKS");
        isGas = GUI.RepeatButton(map.X(-1, 4).Y(-1, 4), "ACCELERATOR");

        // Sync camera
        var camera = Camera.main.GetComponent<CarCamera>();
        camera.viewLook = GUI.HorizontalSlider(map.X(2, 5, 3).Y(1, 4),
            camera.viewLook, -90, 90);

        int mode = (int)camera.viewMode;
        var modeList = new[] {
            CarCamera.ViewMode.DRIVER.ToString(),
            CarCamera.ViewMode.BACK.ToString(),
        };

        mode = GUI.SelectionGrid(map.X(3, 5).Y(-1, 8),
                                    mode, modeList, modeList.Length);
        camera.viewMode = (CarCamera.ViewMode)mode;
    }

    protected override bool markCheckpints { get { return true; } }
    bool isGas, isBreak;
}
