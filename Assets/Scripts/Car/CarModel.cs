using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Car controlling interface */
public interface ICarModel
{
    Transform driverHead { get; }
    Action<Transform> onTrigger { set; }

    string model { get; }
    string details { get; }

    float stearing { get; set; }
    float pedals { get; set; }

    int gear { get; }
    float speed { get; }
    float health { get; }
}

/** Car controlling script */
public class CarModel : MonoBehaviour, ICarModel
{
    public Perfabs perfabs;
    public Attributes attributes;
    public Parts __parts;

    /** @addtgoup ICarModel
     *  @{ */

    public Transform driverHead { get { return transform.FindChild("Head"); } }
    public Action<Transform> onTrigger { get; set; }

    public string model { get { return name; } }
    public string details { get { return string.Join("\n", attributes.details); } }

    public float stearing { get; set; }
    public float pedals { get; set; }

    public int gear { get; set; }
    public float speed { get; set; }
    public float health { get; set; }

    /** @}
     *  @addtgoup MonoBehaviour
     *  @{ */

    void Start()
    {
        name = name.Replace("(Clone)", "");
        driverHead.renderer.enabled = false;
        gameObject.AddComponent<AudioSource>().clip = perfabs.engineSound;
        health = 100;

        // Setup internal links
        __parts.stearingWheel = transform.FindChild("Stearing");
        foreach (var wc in GetComponentsInChildren<WheelCollider>())
        {
            // Create new model
            var tr = (Instantiate(perfabs.wheelModel) as GameObject).transform;
            tr.parent = transform;
            tr.position = wc.renderer.bounds.center;
            tr.rotation = transform.rotation;
            tr.localScale *= wc.radius;
            tr.GetChild(0).localEulerAngles = new Vector3(
                0, wc.name.Contains("L") ? 0 : 180, 0);

            // Replace old one
            wc.renderer.enabled = false;
            var container = wc.name.Contains("F") ?
                __parts.frontWheels : __parts.rearWheels;
            container.Add(new Wheel { collider = wc, model = tr });
        }

        // HUCK: set centerOfMax in the middle of wheels makes car go forward
        attributes.centerOfMass.x +=
            __parts.frontWheels.Average(w => w.collider.center.x);
    }

    void FixedUpdate()
    {
        // Stearing Wheel
        var angle = Mathf.Clamp(stearing, -attributes.maxStearAngle,
            attributes.maxStearAngle) * (health / 100);

        if (__parts.stearingWheel != null)
            __parts.stearingWheel.localEulerAngles = new Vector3(
                __parts.stearingWheel.localEulerAngles.x, 0, -angle * 10f);

        // Wheel colliders
        foreach (var w in __parts.frontWheels)
        {
            w.collider.steerAngle = angle;
            w.collider.motorTorque = pedals *
                attributes.frontDrive * (health / 100);
        }

        foreach (var w in __parts.rearWheels)
            w.collider.motorTorque = pedals *
                attributes.rearDrive * (health / 100);

        // Wheel models
        foreach (var w in __parts.allWheels)
        {
            w.model.eulerAngles = transform.eulerAngles + new Vector3(0, angle, 0);
            w.model.Rotate(w.collider.rpm / 60f * Time.deltaTime * 360f, 0, 0);
        }

        // Current timed characteristics
        speed = __parts.allWheels.Sum(w => w.speed()) / __parts.allWheels.Length;
        gear = (int)Mathf.Sign(speed);

        var power = Mathf.Abs(speed);
        while (power > attributes.gearLength)
        {
            power -= attributes.gearLength;
            gear += (int)Mathf.Sign(speed);
        }

        // Object
        rigidbody.centerOfMass = attributes.centerOfMass;
        audio.pitch = 1 + power / attributes.gearLength;
        audio.volume = (float)Mathf.Abs(gear) / 10f;
        if (!audio.isPlaying && audio.clip.isReadyToPlay)
            audio.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == transform)
            return;

        foreach (var contact in collision.contacts)
        {
            var vec = Vector3.Project(collision.relativeVelocity, contact.normal);
            var extra = 2 - transform.InverseTransformDirection(Vector3.up).y;
            var power = Mathf.Pow(vec.sqrMagnitude, extra);
            if (power > attributes.armor)
            {
                health = Mathf.Clamp(health - power / attributes.armor, 0, 100);
                if (health < 50 && __parts.engineEffect == null)
                {
                    var go = Instantiate(perfabs.crashSmoke) as GameObject;
                    go.transform.parent = transform;
                    go.transform.localPosition = rigidbody.centerOfMass;
                    __parts.engineEffect = go.transform;
                }

                Instantiate(perfabs.crashSparks,
                    contact.point, Quaternion.identity);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (onTrigger != null)
            onTrigger(other.transform);
    }

    /** @}
     *  @addtgoup Types
     *  @{ */

    [Serializable]
    public class Perfabs
    {
        public GameObject crashSparks;
        public GameObject crashSmoke;
        public GameObject wheelModel;
        public AudioClip engineSound;
    }

    [Serializable]
    public class Attributes
    {
        public Vector3 centerOfMass = new Vector3(0, -0.9f, 0);
        public float maxStearAngle = 45;
        public float frontDrive = 0, rearDrive = 50;
        public float gearLength = 50;
        public float armor = 50;

        public float price
        { get {
            return (frontDrive + rearDrive) * armor * maxStearAngle;
        } }

        public string[] details
        { get {
            return new string[] {
                price.ToString("0,000") + " RUR",
                "Stearing: " + maxStearAngle, "Armor: " + armor,
                "Front drive: " + frontDrive, "Rear drive: " + rearDrive,
            };
        } }
    }

    [Serializable]
    public class Wheel
    {
        public WheelCollider collider;
        public Transform model;

        public float speed()
        {
            return 2 * Mathf.PI * collider.rpm * collider.radius * 0.06f;
        }
    }

    [Serializable]
    public class Parts
    {
        public List<Wheel> frontWheels = new List<Wheel>();
        public List<Wheel> rearWheels = new List<Wheel>();

        public Wheel[] allWheels
        { get {
            return frontWheels.Concat(rearWheels).ToArray();
        } }

        public Transform stearingWheel;
        public Transform engineEffect;
    }

    /** @} */
}
