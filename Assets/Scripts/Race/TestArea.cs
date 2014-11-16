using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class TestArea : MonoBehaviour
{
    public GameObject playerModel;

    void Awake()
    {
        var player = (IDriver)UnityEngine.Object.FindObjectOfType<DriverGUI>();
        player.carModel = playerModel;
        player.approved = true;
    }
}
