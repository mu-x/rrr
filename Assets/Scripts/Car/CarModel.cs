using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Car controlling interface */
public interface ICarModel {
    bool isReady { get; }
    Transform driverHead { get; }
    Action<GameObject> onTrigger { set; }

    float stearing { set; }
    float pedals { set; }

    float speed { get; }
    string info { get; }
}

/** Car controlling Script */
public class CarModel : MonoBehaviour, ICarModel {
    public GameObject crashSparks;
    public GameObject crashSmoke;
    public GameObject wheelModel;

    public Vector3 centerOfMass = new Vector3(0, -0.9f, 0);
    public float maxStearAngle = 45;
    public float frontDrive = 0, rearDrive = 50;
    public float enginePitchDev = 150, engineVolumeDev = 20;
    public float armor = 50;

    public bool isReady { get; set; }
    public Action<GameObject> onTrigger { get; set; }

    public Transform driverHead { get { return transform.FindChild("Head"); } }
    Transform stearingWheel { get { return transform.FindChild("Stearing"); } }

    /** Loads resources, prepares @var this object for scene */
    void Start() {
        crashSparks = crashSparks ?? Resources.Load<GameObject>("Crash Sparks");
        crashSmoke = crashSmoke ?? Resources.Load<GameObject>("Crash Smoke");

        wheelModel = wheelModel ?? Resources.Load<GameObject>("Wheel Basic");
        SetupWheels(GetComponentsInChildren<WheelCollider>());

        if (audio == null)
            gameObject.AddComponent<AudioSource>().clip =
                Resources.Load<AudioClip>("Engine Sound");

        rigidbody.centerOfMass = centerOfMass;
        health = 100;
        isReady = true;
    }

    /** Replace @var wheelColliders with @var wheelModel copies */
    void SetupWheels(WheelCollider[] wheelColliders) {
        List<Wheel> fw = new List<Wheel>(), rw = new List<Wheel>();
        foreach (var wc in wheelColliders) {
            wc.renderer.enabled = false;
            var center = wc.renderer.bounds.center;
            var go = Instantiate(wheelModel, center, new Quaternion())
                as GameObject;

            // Configure instantiated models
            var wheel = new Wheel {
                collider = wc, model = go.transform,
                rotation = wc.name.Contains("L") ? 0 : 180
            };

            wheel.model.parent = wheel.collider.transform;
            wheel.model.localScale *= wheel.collider.radius;
            wheel.model.localRotation = new Quaternion(0, wheel.rotation, 0, 0);

            // Save wheel controls to provate members
            (wc.name.Contains("F") ? fw : rw).Add(wheel);
        }

        frontWheels = fw.ToArray();
        rearWheels = rw.ToArray();
    }

    /** Rotates wheels and play engine sound */
    void Update() {
        Array.ForEach(frontWheels, w => w.model.Rotate(speed, 0, 0));
        Array.ForEach(rearWheels, w => w.model.Rotate(speed, 0, 0));

        if (!audio.isPlaying)
            audio.Play();

        audio.pitch = 1 + (speed / enginePitchDev);
        audio.volume = (1 + (speed / engineVolumeDev)) / 10;
    }

    /** Handles collisions with environment objects */
    void OnCollisionEnter(Collision collision) {
        if (collision.transform == transform)
            return;

        // Apply danage and hit effect if collision beats armor
        foreach (var contact in collision.contacts) {
            var power = Vector3.Project(collision.relativeVelocity,
                                        contact.normal).sqrMagnitude;
            if (power < armor) return;
            if ((health -= power / armor) <= 0) health = 0;

            Instantiate(crashSparks, contact.point, Quaternion.identity);
            if (health < 75 && engineEffect == null) {
                var go = Instantiate(crashSmoke,
                    transform.position, transform.rotation) as GameObject;
                engineEffect = go.transform;
                engineEffect.parent = transform;
            }
        }
    }

    /** Handles visits to game places */
    void OnTriggerEnter(Collider other) {
        if (onTrigger != null)
            onTrigger(other.gameObject);
    }

    public float stearing {
        set {
            value = Mathf.Clamp(value, -maxStearAngle, maxStearAngle);
            stearingWheel.localEulerAngles = new Vector3(0, 0, value * 5);
            foreach (var w in frontWheels) {
                w.collider.steerAngle = value;
                w.model.localRotation = new Quaternion(
                    0, w.rotation + value, 0, 0);
            }
        }
    }

    public float pedals {
        set {
            Array.ForEach(frontWheels,
                w => w.collider.motorTorque = value * frontDrive);
            Array.ForEach(rearWheels,
                w => w.collider.motorTorque = value * rearDrive);
        }
    }

    public float speed {
        get { return rigidbody.velocity.sqrMagnitude; }
    }

    /** Car model specific information */
    public string info {
        get {
            return string.Join("\n", new string[] { name, "-",
                "Total mass: " + rigidbody.mass, "Patrol type: Gas",
                "Center mass: " + centerOfMass.y, "Stearing: " + maxStearAngle,
                "Front drive: " + frontDrive, "Rear drive: " + rearDrive,
                "Armor: " + armor,
            });
        }
    }

    Wheel[] frontWheels, rearWheels;
    Transform engineEffect;
    float health = 100;

    public struct Wheel {
        public WheelCollider collider;
        public Transform model;
        public float rotation;
    }
}
