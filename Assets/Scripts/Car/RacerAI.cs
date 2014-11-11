using UnityEngine;
using System.Collections;

public class RacerAI : Driver {
    public float maxSteer = 15;
    public float maxGas = 1;

    void FixedUpdate() {
        if (!isEnabled)
            return;

        var target = expectedPoint.transform.position;
        var point = car.driverHead.InverseTransformPoint(target);

        car.stearing = maxSteer * (point.x / point.magnitude);
        car.pedals = maxGas * Mathf.Sign(Mathf.Abs(point.z) * 20 - car.speed);
    }
}
