using System.Collections;
using UnityEngine;

public class RacerAI : Driver {
    public float steerRatio = 15;
    public float speedRatio = 20;

    void FixedUpdate()
    {
        if (expectedPoint != null)
        {
            var target = expectedPoint.transform.position;
            var point = car.driverHead.InverseTransformPoint(target);

            car.stearing = steerRatio * (point.x / point.magnitude);
            car.pedals = Mathf.Sign(Mathf.Abs(point.z) * speedRatio - car.speed);
        }
    }
}
