using UnityEngine;
using System.Collections;

/** The part of road's geometry */
public class RoadPart : MonoBehaviour
{
    /** Makes object visible on gismos */
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
