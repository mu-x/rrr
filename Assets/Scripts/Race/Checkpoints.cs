using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

/** Represents a path over child @class Transform */
public class Checkpoints : MonoBehaviour {
    void Start() {
        Array.ForEach(Get(), cp => cp.renderer.enabled = false);
    }

    /** Draws route on @class Gizmos */
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        var points = Get();
        for (int it = 0; it < points.Length; ++it) {
            var next = it + 1;
            if (next == points.Length)
                next = 0;

            Gizmos.DrawLine(points[it].transform.position,
                            points[next].transform.position);
        }
    }

    /** Searches for children if not cached */
    public GameObject[] Get() {
        var cps = new List<GameObject>();
        foreach (var cp in GetComponentsInChildren<Transform>())
            if (cp.transform != transform)
                cps.Add(cp.gameObject);

        return cps.ToArray();
    }
}
