using UnityEngine;
using System.Collections;
using System;
using UnityExtensions;

public class PlayerUI : MonoBehaviour
{
    public CarControl car;
    public float acceleration;
    public Action exit;

    public void Gas(bool on) { car.gas = on; Debug.Log("G"); }
    public void Breaks(bool on) { car.breaks = on; Debug.Log("B"); }
    public void Exit() { exit(); }

    void OnEnable()
    {
        car.CaptureCamera();
        car.breaks = false;
        acceleration = PlayerPrefs.GetFloat("Input.Acceleration", 2.5f);
        Debug.Log("Acc: " + acceleration, this);
    }

    void FixedUpdate () 
    {
        car.stearing = Input.acceleration.x * acceleration
            + Input.GetAxis("Horizontal") * 25;
	}
}
