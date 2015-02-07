using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/** The road curve @class Mesh controller */
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RoadCurve : MonoBehaviour {

    public float roadWidth = 2;
    public bool autoRebuild = true;

    public Vector3[] way;
    public MeshFilter meshFilter;

    // TODO: look up for better editor event
    void OnDrawGizmos() 
    {
        if (!autoRebuild) {
            var parts = GetComponentsInChildren<RoadPart>();
            way = Array.ConvertAll(parts, p => p.transform.localPosition);
            SmoothWayPoints();

            meshFilter = GetComponent<MeshFilter>();
            UpdateWayMesh();
        }
    }

    /** Adds more vertices to @var way to smooth it */
    void SmoothWayPoints() 
    {
        var points = way.ToList();
        for (int i = 1; i < points.Count; ++i) {
            var diff = points[i] - points[i - 1];

            // Add extra point if distance is above smooth limit
            if (diff.magnitude > roadWidth * 3) { 
                var shift = new Vector3();

                // Next neighbour point shifts extra one
                if (i != points.Count - 1) {
                    var next = (points[i + 1] - points[i]).normalized;
                    var orto = new Vector3(diff.z, diff.y, -diff.x).normalized;
                    shift = Vector3.Project(next, orto) * diff.magnitude;
                }

                var extra = points[i] - diff / 2 - shift / 10;
                points.Insert(i--, extra);
            }
        }
        way = points.ToArray();
    }

    /** Makes @class Mesh according to @arg way */
    void UpdateWayMesh()
    {
        var vertices = new Vector3[way.Length * 2];
        var triangles = new int[(way.Length - 1) * 2 * 3];

        float uvShift = 0;
        var uv = new Vector2[way.Length * 2];

        for (int i = 0; i < way.Length; ++i) {
            var norm = MakeWayNormal(i) * roadWidth;
            vertices[2 * i + 0] = way[i] - norm;
            vertices[2 * i + 1] = way[i] + norm;

            if (i != way.Length - 1) {
                var map = new[] { 2, 1, 0, 1, 2, 3 };
                for (int j = 0; j < map.Length; ++j)
                    triangles[map.Length * i + j] = map[j] + i * 2;
            }

            if (i != 0)
                uvShift += (way[i] - way[i - 1]).magnitude / roadWidth;
            uv[2 * i + 0] = new Vector2(0, uvShift);
            uv[2 * i + 1] = new Vector2(1, uvShift);
        }

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    /** Calculates @var way point normal */
    Vector3 MakeWayNormal(int i) 
    {
        var prev = (i == 0) ? 0 : (i - 1);
        var next = (i == way.Length - 1) ? (way.Length - 1) : (i + 1);

        var orto = (way[next] - way[prev]).normalized;
        return new Vector3(orto.z, orto.y, -orto.x);
    }
}
