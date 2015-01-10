using UnityEngine;
using System.Collections;
using UnityExtensions;

/** Represents thw wheel controllable by @class WheelCollider */
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(WheelCollider))]
public class CarWheel : MonoBehaviour 
{
    public GameObject prefab; /**< Wheel model to replace default */
    public AudioClip skid; /**< Skid sound */
    public bool isFront; /**< Front or rear wheel */
    public float speed; /**< Speed of wheel relative to the world */
    public WheelCollider collider; /**< Collider to contoll the wheel */

    public float slipStart = 1.5f, slipLimit = 5;
    public float slipFriction, slipWait; /**< Wheel slip */
    public Transform local, model;

    // TODO: look up for better editor event
    void OnDrawGizmos() 
    {
        this.checkField("prefab", prefab);
        transform.rotation = transform.parent.rotation;
        isFront = (transform.localPosition.z > 0);

        collider = GetComponent<WheelCollider>();
        collider.forwardFriction = FrictionCurve(1, 30, 2, 5);
        collider.sidewaysFriction = FrictionCurve(0.5f, 10, 2, 1);
    }

    /** Replace wheel model with a @var prefab instance */
    void Start()
    {
        if (model == null) {
            collider.renderer.enabled = false;
            model = transform.parent.gameObject.MakeChild(
                name + ".Socket", transform).transform;

            local = model.gameObject.MakeChild(prefab).transform;
            local.localScale *= collider.radius * 2;
            local.localEulerAngles = new Vector3(
                0, (transform.localPosition.x > 0) ? 180 : 0, 0);
        }
    }

    /** Updates wheel model position and rotation */
    void FixedUpdate() 
    {
        speed = collider.rpm * collider.radius * Mathf.PI * 60f / 1000f;
        model.localEulerAngles = new Vector3(
                model.localEulerAngles.x, collider.steerAngle, 0);
        var k = (transform.localPosition.x > 0) ? -1 : 1;
        local.Rotate(collider.rpm / 60f * Time.deltaTime * 360f * k, 0, 0);

        WheelHit hit; collider.GetGroundHit(out hit);
        slipFriction = Mathf.Abs(hit.sidewaysSlip);
        slipWait -= Time.deltaTime * slipLimit;
        if (slipFriction > slipStart && slipWait <= 0) {
            audio.PlayOneShot(skid);
            slipWait = 1;
        }
    }

    WheelFrictionCurve FrictionCurve(float eS, float eV, float aS, float aV)
    {
        var fc = new WheelFrictionCurve();
        fc.extremumSlip = eS; fc.extremumValue = eV * 1000;
        fc.asymptoteSlip = aS; fc.asymptoteValue = aV * 1000;
        fc.stiffness = 0.5f;
        return fc;
    }
}
 