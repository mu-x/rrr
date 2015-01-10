using UnityEngine;
using System.Collections;

/** Destroys gameobject after some time */
public class DestroyTimer : MonoBehaviour 
{
    public float time = 2; /**< Destruction time in seconds */

	void Update () 
    {
        if ((time -= Time.deltaTime) <= 0)
            Destroy(gameObject);
	}
}
