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
    public float gearLength = 300;
    public float armor = 50;

    public bool isReady { get; set; }
    public Action<GameObject> onTrigger { get; set; }

    public Transform driverHead { get { return transform.FindChild("Head"); } }
    Transform stearingWheel { get { return transform.FindChild("Stearing"); } }

    /** Loads resources, prepares @var this object for scene */
    void Start() {
        driverHead.renderer.enabled = false;
        rigidbody.centerOfMass = centerOfMass;

        crashSparks = crashSparks ?? Resources.Load<GameObject>("Crash Sparks");
        crashSmoke = crashSmoke ?? Resources.Load<GameObject>("Crash Smoke");

        wheelModel = wheelModel ?? Resources.Load<GameObject>("Wheel Basic USSR");
        SetupWheels(GetComponentsInChildren<WheelCollider>());

        var mid = new Vector3();
        Array.ForEach(frontWheels,
            w => mid += w.collider.center);
        centerOfMass += mid * (1 / frontWheels.Length);

        if (audio == null) {
            gameObject.AddComponent<AudioSource>();
            audio.clip = Resources.Load<AudioClip>("Car Engine");
            audio.Play();
        }

        health = 100;
        isReady = true;
    }

    /** Replace @var wheelColliders with @var wheelModel copies */
    void SetupWheels(WheelCollider[] wheelColliders) {
        List<Wheel> fw = new List<Wheel>(), rw = new List<Wheel>();
        foreach (var wc in wheelColliders) {
            var center = wc.renderer.bounds.center;
            wc.renderer.enabled = false;

            var tr = (Instantiate(wheelModel) as GameObject).transform;
            tr.parent = transform;
            tr.position = center;
            tr.rotation = transform.rotation;
            tr.localScale *= wc.radius;

            // Configure instantiated models
            var wheel = new Wheel { collider = wc, model = tr };
            var wm = wheel.model.GetChild(0);
            wm.localEulerAngles =
                new Vector3(0, wc.name.Contains("L") ? 0 : 180, 0);

            // Save wheel controls to provate members
            (wc.name.Contains("F") ? fw : rw).Add(wheel);
        }

        frontWheels = fw.ToArray();
        rearWheels = rw.ToArray();
    }

    /** Rotates wheels and play engine sound */
    void FixedUpdate() {
        Action<Wheel> rotate = delegate(Wheel w) {
            w.model.Rotate(w.collider.rpm / 60f * Time.deltaTime * 360f, 0, 0);
        };

        Array.ForEach(frontWheels, rotate);
        Array.ForEach(rearWheels, rotate);

        if (!audio.isPlaying && audio.clip.isReadyToPlay)
            audio.Play();

        var gear = (int)(speed / gearLength);
        var rest = speed - gear * gearLength;
        audio.pitch = 1 + 0.1f * gear + 0.5f * rest / gearLength;
        audio.volume = 0.2f + 0.15f * gear;
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
                var go = Instantiate(crashSmoke) as GameObject;
                engineEffect = go.transform;
                engineEffect.parent = stearingWheel;
                engineEffect.localPosition = Vector3.forward;
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
            stearingWheel.localEulerAngles = 
                new Vector3(stearingWheel.localEulerAngles.x, 0, -value * 10);
            foreach (var w in frontWheels) {
                w.collider.steerAngle = value;
                w.model.eulerAngles = transform.eulerAngles + new Vector3(
                    0, value, 0);
            }
        }
    }

    public float pedals {
        set {
            var engine = (float)health / 100f; 
            Array.ForEach(frontWheels,
                w => w.collider.motorTorque = value * frontDrive * engine);
            Array.ForEach(rearWheels,
                w => w.collider.motorTorque = value * rearDrive * engine);
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
    }
}
