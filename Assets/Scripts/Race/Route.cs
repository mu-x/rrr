using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

/** Represents a path over child @class Transform */
public class Route : MonoBehaviour
{
    public Transform[] points { get; set; }

    void Start()
    {
        setupPoints();
        Array.ForEach(points, cp => cp.renderer.enabled = false);
    }

    /** Draws route on @class Gizmos */
    void OnDrawGizmos()
    {
        setupPoints();

        Gizmos.color = Color.yellow;
        for (int it = 0; it < points.Length; ++it)
        {
            var next = it + 1;
            if (next == points.Length)
                next = 0;

            points[it].name = it.ToString("00");
            Gizmos.DrawLine(points[it].position,
                            points[next].position);
        }
    }

    void setupPoints()
    {
        var cps = new List<Transform>();
        foreach (var tr in GetComponentsInChildren<Transform>())
            if (tr.transform != transform)
                cps.Add(tr);
        points = cps.ToArray();
    }
}
