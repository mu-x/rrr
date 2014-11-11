using System.Collections;
using System;
using UnityEngine;

/** Abstract driver in the Race */
public abstract class Driver : MonoBehaviour {
    public bool isEnabled = true;
    public int pointsPassed = 0, roundsPassed = 0;

    void Start() {
        renderer.enabled = false;
    }

    public virtual void Prepare(GameObject carModel, GameObject[] route = null,
                                int roundsExpected = 0, Action finish = null) {
        car = (Instantiate(carModel, transform.position, transform.rotation)
            as GameObject).GetComponent<CarModel>();

        if (route == null)
            return;

        chickpointSelector = new Selector<GameObject>(
            route, null,
            delegate(GameObject next) {
                if (markCheckpints)
                    next.renderer.enabled = true;

                pointsPassed++;
                expectedPoint = next;
            },
            delegate() {
                if (++roundsPassed == roundsExpected)
                    finish();
            }
        );

        car.onTrigger = delegate(GameObject go) {
            if (go != expectedPoint)
                return;

            if (markCheckpints)
                go.renderer.enabled = false;

            chickpointSelector.Next();
        };
    }

    protected virtual bool markCheckpints { get { return false; } }

    protected ICarModel car;
    protected GameObject expectedPoint;
    ISelector chickpointSelector;
}
