using System.Collections;
using UnityEngine;

/** AI controlled @calss Driver */
public class DriverAI : Driver
{
    public float steerRatio = 15;
    public float speedRatio = 20;

    void FixedUpdate()
    {
        if (approved && expectedPoint != null)
        {
            var target = expectedPoint.transform.position;
            var point = car.driverHead.InverseTransformPoint(target);

            car.stearing = steerRatio * (point.x / point.magnitude);
            car.pedals = Mathf.Sign(Mathf.Abs(point.z) * speedRatio - car.speed);
        }
    }
}
