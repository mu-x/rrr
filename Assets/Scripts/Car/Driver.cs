using System.Collections;
using System;
using UnityEngine;

/** Abstract driver in the Race */
public abstract class Driver : MonoBehaviour {
    public bool isEnabled = false;
    public int pointsPassed = 0, roundsPassed = 0;

    public static int Compare(Driver a, Driver b) { 
        return a.pointsPassed.CompareTo(b.pointsPassed);  
    }

    void Start() {
        renderer.enabled = false;
    }

    public virtual void Prepare(GameObject carModel = null,
                                GameObject[] route = null,
                                int roundsExpected = 0, 
                                Action finish = null) {
        var newCar = (Instantiate(carModel ?? 
            Resources.Load<GameObject>("Car Models/Kopeck L2101"),
            transform.position, transform.rotation)
            as GameObject).GetComponent<CarModel>();

        newCar.transform.parent = transform;
        car = newCar;
        name += " " + newCar.name;

        if (route == null)
            return;

        chickpointSelector = new Selector<GameObject>(
            route, null,
            delegate(GameObject next) {
                pointsPassed++;
                expectedPoint = next;
                if (markCheckpints)
                    expectedPoint.renderer.enabled = true;
            },
            delegate() {
                if (++roundsPassed == roundsExpected)
                    finish();
            }
        );

        car.onTrigger = delegate(GameObject go) {
            if (go != expectedPoint)
                return;

            chickpointSelector.Next();
            if (markCheckpints)
                go.renderer.enabled = false;
        };
    }

    protected virtual bool markCheckpints { get { return false; } }

    protected ICarModel car;
    protected GameObject expectedPoint;
    ISelector chickpointSelector;
}
