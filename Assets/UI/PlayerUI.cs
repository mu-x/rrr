using UnityEngine;
using System.Collections;
using System;
using UnityExtensions;

public class PlayerUI : MonoBehaviour
{
    public CarControl car;
    public float acceleration;
    public Action exit;

    public void Gas(bool on) { car.gas = on; }
    public void Breaks(bool on) { car.breaks = on; }
    public void Exit() { exit(); }

    void OnEnable()
    {
        car.CaptureCamera();
        car.breaks = false;
        acceleration = PlayerPrefs.GetFloat("Input.Acceleration", 2.5f);
    }

    void FixedUpdate () 
    {
        car.stearing = Input.acceleration.x * acceleration
            + Input.GetAxis("Horizontal") * 25;
	}
}
