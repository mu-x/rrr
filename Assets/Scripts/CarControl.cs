using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/** Interface for car input controllers */
public interface ICarInput {
    float stearing { get; } /**< Staering wheel position */
    float pedals { get; } /**< Gas/break pedals balance */
    float look { get; } /**< Driver head position (affects camera) */
}

/** Applies effect of the Control to car Wheels */
public class CarControl : MonoBehaviour {
    public Controls control;
    public Effects effects;
    public Wheels wheels;
    public Transform stearingWheel, watchPoint;
    public Dictionary<string, ISelector> options;
    public CheckpointControl checkpoints;

    public enum CarType { PLAYER, RACER, TRAFIC };
    public CarType type;

    void Start() {
        control.Setup(rigidbody);
        health = 100;
    }

    void Update() {
        wheels.Update();
    }

    /** Apllies car @var control to car @var wheels */
    void FixedUpdate() {
        if (control.input == null)
            return; // Nothing to apply :)

        var engine = health / 100f;
        stearingWheel.localEulerAngles = new Vector3(0, 0, control.move * engine);

        wheels.SetStearing(control.stearing * engine);
        wheels.SetTorque(control.frontTorque * engine,
                         control.rearTorque * engine);

        effects.Engine(audio, wheels.CarSpeed());
    }

    /** Handles collisions with environment objects */
    void OnCollisionEnter (Collision collision) {
        if (type == CarType.PLAYER)
            Handheld.Vibrate();

        if (collision.transform == transform)
            return;

        // Apply danage and hit effect if collision beats armor
        foreach (var contact in collision.contacts) {
            var power = Vector3.Project(collision.relativeVelocity,
                                        contact.normal).sqrMagnitude;
            if (power > control.armor) {
                if ((health -= power / control.armor) <= 0) {
                    health = 0;
                }
                effects.Hit(contact.point, health);
            }
        }
    }

    /** Handles visits to the special game places */
    void OnTriggerEnter (Collider other) {
        if (checkpoints != null && other.tag == "CheckPoint")
            checkpoints.enter(other.gameObject);
    }

    /** Car model specific information */
    public string info {
        get {
            return string.Join("\n", new string[] { name, "-",
                "Front drive: " + control.frontDrive,
                "Rear drive: " + control.rearDrive,
                "Armor: " + control.armor,
                "Stearing amp: " + control.stearPower,
                "Patrol type: gas",
                "Transmission: S",
                "Total mass: " + rigidbody.mass,
                "Center mass: " + rigidbody.centerOfMass.y,
            });
        }
    }

    float health = 100;
}

/** Adapter for @interface ICarInput */
[System.Serializable]
public class Controls {
    public float stearPower = 25, stearSensivity = 4;
    public float frontDrive = 0, rearDrive = 50;
    public float centerMassY = -0.9f;
    public float armor = 100;
    public ICarInput input;

    public void Setup(Rigidbody body) {
        body.centerOfMass = new Vector3(
            body.centerOfMass.x, centerMassY, body.centerOfMass.z);
    }

    public float frontTorque { get { return frontDrive * input.pedals; } }
    public float rearTorque { get { return rearDrive * input.pedals; } }
    public float stearing { get { return stearPower * input.stearing; } }
    public float move { get { return -stearSensivity * stearing; } }
}

/** Effects provider */
[System.Serializable]
public class Effects {
    public float enginePitchDev = 150, engineVolumeDev = 20;
    public GameObject spark, smoke;
    public Transform engine;

    public void Engine(AudioSource audio, float speed) {
        if (!audio.isPlaying) audio.Play();
        audio.pitch = 1 + (Mathf.Abs(speed) / enginePitchDev);
        audio.volume = (1 + (Mathf.Abs(speed) / engineVolumeDev)) / 10;
    }

    public void Hit(Vector3 point, float helth = 100f) {
        Object.Instantiate(spark, point, Quaternion.identity);
        if (helth < 75 && engineEffect == null) {
            engineEffect = Object.Instantiate(smoke, engine.position,
                                              Quaternion.identity) as GameObject;
            engineEffect.transform.parent = engine;
            engineEffect.transform.position = engine.position;
        }
    }

    GameObject engineEffect;
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