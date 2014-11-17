using System.Collections;
using System;
using UnityEngine;

/** Interface to control @class Driver from the race */
public interface IDriver
{
    bool approved { set; }
    int roundsExpected { set; }

    int pointsPassed { get; }
    int roundsPassed { get; }

    Action onVisit { set; }
    Action onFinish { set; }
    GameObject carModel { set; }
}

/** Abstract car driver base, gets @class Route, attaches to the
 *  @interface CarModel and fires @interface IDriver */
public abstract class Driver : MonoBehaviour, IDriver {
    protected virtual bool marker { get { return false; } }
    protected ICarModel car;
    protected Transform expectedPoint;

    ISelector cpSelector;

    /** @addtgoup IDriver
     *  @{ */

    public bool approved { get;  set; }
    public int roundsExpected { get; set; }

    public int pointsPassed { get; set; }
    public int roundsPassed { get; set; }

    public Action onVisit { get; set; }
    public Action onFinish { get; set; }

    public GameObject carModel
    { set {
        var newCar = Instantiate(value, transform.position, transform.rotation)
                as GameObject;

        newCar.transform.parent = transform;
        car = newCar.GetComponent<CarModel>();
    } }

    /** @}
     *  @addtgoup MonoBehaviour
     *  @{ */

    void Awake()
    {
        renderer.enabled = false;
        approved = false;
    }

    protected void Start()
    {
        var route = FindObjectOfType<Route>();
        if (route == null) return; // DBG

        if (car == null)
        {
            this.enabled = false;
            return; // driver is not playing this race
        }

        cpSelector = new Selector<Transform>(
            route.points, null,
            delegate(Transform next)
            {
                pointsPassed++;
                expectedPoint = next;
                if (marker)
                    expectedPoint.renderer.enabled = true;
                if (onVisit != null)
                    onVisit();
            },
            delegate()
            {
                if (++roundsPassed == roundsExpected && onFinish != null)
                    onFinish();
            });

        car.onTrigger = delegate(Transform tr)
        {
            if (tr == expectedPoint)
            {
                if (marker)
                    expectedPoint.renderer.enabled = false;
                cpSelector.Next();
            }
        };
    }

    /** @} */
}
