using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Interface for car input controllers */
public interface ICarInput {
	float GetStearing();
	float GetPedals();
	float GetLook();
}

/** Adapter for @interface ICarInput */
[System.Serializable]
public class Controls {
	public float stearPower = 25, stearSensivity = 4;
	public float frontDrive = 0, rearDrive = 50;
	public float centerMassDrop = 0.5f;
	public ICarInput input;

	public void Setup(Rigidbody body) {
		var c = body.centerOfMass;
		body.centerOfMass = new Vector3(c.x, c.y - centerMassDrop, c.z);
	}

	public float GetFrontTorque() { return frontDrive * input.GetPedals(); }
	public float GetRearTorque() { return rearDrive * input.GetPedals(); }
	public float GetStearing() { return stearPower * input.GetStearing(); }
	public float GetStearMove() { return -stearSensivity * this.GetStearing(); }
}

/** Effects provider */
[System.Serializable]
public class Effects { 
	public float enginePitchDev = 150, engineVolumeDev = 20;
	
	public void Engine(AudioSource audio, float speed) {
		audio.pitch = 1 + (Mathf.Abs(speed) / enginePitchDev);
		audio.volume = (1 + (Mathf.Abs(speed) / engineVolumeDev)) / 10;
	}
}

/** Controlls Transforms and Colliders of Wheels */
[System.Serializable]
public class Wheels {
	public WheelCollider cFL, cFR, cRL, cRR;
	public Transform tFL, tFR, tRL, tRR;
	
	public void SetStearing(float angle) {
		cFL.steerAngle = angle;
		cFR.steerAngle = angle;

		tFL.localEulerAngles = new Vector3(0, angle, 0);
		tFR.localEulerAngles = new Vector3(0, angle, 0);
	}

	public void SetTorque(float front, float rear) {
		cFL.motorTorque = front;
		cFR.motorTorque = front;
		cRL.motorTorque = rear;
		cRR.motorTorque = rear;
	}

	public void Update() {
		foreach (var tr in new Transform[] { tFL, tFR, tRL, tRR })
			tr.Rotate(CarSpeed(), 0, 0);
	}

	public static float Speed(WheelCollider colider) {
		return (2 * Mathf.PI * colider.radius * colider.rpm * 60) / 1000;
	}

	public float CarSpeed() {
		return (Speed(cFL) + Speed(cFR) + Speed(cRL) + Speed(cRR)) / 4; 
	}
}

/** Applies effect of the Control to car Wheels */
public class CarControl : MonoBehaviour {
	public Controls control;
	public Effects effects;
	public Wheels wheels;
    public Transform stearingWheel, watchPoint;
    public Dictionary<string, ISelector> options;

	void Start() { control.Setup(rigidbody); }
	void Update() { wheels.Update(); }

	void FixedUpdate() {
		if (control.input == null) return; // Nothing to apply :)

		stearingWheel.localEulerAngles = new Vector3(0, 0, control.GetStearMove());
		wheels.SetStearing(control.GetStearing());
		wheels.SetTorque(control.GetFrontTorque(), control.GetRearTorque());
		effects.Engine(audio, wheels.CarSpeed());
	}	
}