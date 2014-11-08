using System.Collections;
using System.Linq;
using UnityEngine;

/**  */
public class CheckPoint : MonoBehaviour {
    public delegate void Enter();
    public Enter playerEnter;
    /*
    public bool isEnabled {
        get { return collider.enabled || renderer.enabled; }
        set { collider.enabled = renderer.enabled = value; }
    }

    void OnTriggerEnter (Collider other) {
        lock(this) {
            CarControl[] cars  = other.GetComponentsInParent<CarControl>();
            if (cars.Length != 0 && isEnabled)
                if(cars.First().type == CarControl.CarType.PLAYER) {
                    playerEnter();
                    isEnabled = false;
                }
        }
    }

    void Start() { isEnabled = false; }
    */
}
