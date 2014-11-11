using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class TestArea : MonoBehaviour {
	void Start () {
        cp = GetComponentInChildren<Route>().Points();
        setup(GetComponentInChildren<Player>());
        Array.ForEach(GetComponentsInChildren<RacerAI>(), setupR);
	}

    void setup(Driver driver) {
        driver.Prepare(route: cp);
        driver.isEnabled = true;
    }

    void setupR(RacerAI racer) {
        setup(racer);
        //UnityEngine.Random.seed = (int)(DateTime.Now.ToOADate() * 1000);
        //racer.maxGas *= UnityEngine.Random.Range(0.5f, 1f);
    }

    GameObject[] cp;
}
