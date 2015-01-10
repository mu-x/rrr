using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityExtensions;

/** THe Car model behaviour controller */
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CarModel : MonoBehaviour 
{
    [System.Serializable]
    public class Prefabs 
    { 
        public GameObject wheel;
        public AudioClip engine;
        public GameObject crash;
        public AudioClip skid;
    }
    public Prefabs prefabs;

    public Transform stearing, head;
    public CarWheel[] wheels;

    public float speed;

    // TODO: look up for better editor event
    void OnDrawGizmos()
    {
        stearing = transform.Child("Stearing");
        head = transform.Child("Head");
        head.renderer.enabled = false;

        if (wheels == null || wheels.Length == 0)
            wheels = Array.ConvertAll(transform.Children("Wheel"),
                t => t.HasComponent<CarWheel>());

        this.checkField("prefabs.wheel", prefabs.wheel);
        this.checkField("prefabs.skid", prefabs.skid);
        foreach (var w in wheels) {
            w.prefab = prefabs.wheel;
            w.skid = prefabs.skid;
        }

        this.checkField("prefabs.engine", prefabs.engine);
        audio.clip = prefabs.engine;
        audio.loop = true;
    }

    /** Calculates speed and adjasts engine sound to it */
    void FixedUpdate() 
    {
        speed = wheels.Average(w => w.speed);

        var gear = (int)Mathf.Abs(speed / 30);
        var power = Mathf.Abs(speed) - gear * 30;

        audio.pitch = 1 + power / 30;
        audio.volume = 0.5f + (float)Mathf.Abs(gear) / 10f;
        if (!audio.isPlaying && audio.clip.isReadyToPlay)
            audio.Play();
    }

    /** Performs collision effect */
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == transform)
            return;

        foreach (var c in collision.contacts) {
            var vec = Vector3.Project(collision.relativeVelocity, c.normal);
            var exp = 2 - transform.InverseTransformDirection(Vector3.up).y;
            var power = Mathf.Pow(vec.sqrMagnitude, exp);
            if (power > 50)
                Instantiate(prefabs.crash, c.point, Quaternion.identity);
        }
    }
}
