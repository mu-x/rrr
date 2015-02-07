using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Road : MonoBehaviour 
{
    public float roadWidth = 2;
    public float roadHillDelta = 0.01f;
    public int roadHillTexture = 1;
    public bool autoRebuild = true;

    public int smoothRate = 1;
    public float smoothPoint = 0.5f;
    public float smoothDistanse = 0.1f;
    public float smoothCorrection = 0.1f;

    public Terrain terrain;
    public Transform[] points;
    public Vector3[] way;

    public float[,] stashHeights;
    public float[, ,] stashAlphamap;

    // TODO: look up for better editor event
    void OnDrawGizmos() 
    {
        terrain = GetComponentInParent<Terrain>();
        points = transform.Children("Point");
        way = Array.ConvertAll(points, p => p.localPosition);
        if (autoRebuild) {
            for (int i = 0; i < smoothRate; ++i) SmoothWay();
            GetComponent<MeshFilter>().mesh = MakeWayMesh();
        }
	}

    void Start()
    {
        foreach (var p in points) 
            p.renderer.enabled = false;

        var td = terrain.terrainData;
        stashHeights = td.GetHeights(0, 0, td.heightmapWidth, td.heightmapHeight);
        stashAlphamap = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
        AdjastTerrainToWay(td);
    }

    void OnDestroy() 
    {
        terrain.terrainData.SetHeights(0, 0, stashHeights);
        terrain.terrainData.SetAlphamaps(0, 0, stashAlphamap);
    }

    void AdjastTerrainToWay(TerrainData td)
    {
        int xResH = td.heightmapWidth, yResH = td.heightmapHeight;
        int xResA = td.alphamapWidth, yResA = td.alphamapHeight;

        var heights = td.GetHeights(0, 0, xResH, yResH);
        var maps = td.GetAlphamaps(0, 0, xResA, yResA);

        foreach (var point in way) {
            var glob = transform.TransformPoint(point);
            var self = terrain.transform.InverseTransformPoint(glob);

            int xH = (int)(self.z * xResH / td.size.z);
            int yH = (int)(self.x * yResH / td.size.x);
            var h = (self.y - roadHillDelta) / td.size.y;
            for (int x = xH - 1; x <= xH + 1; ++x)
                for (int y = yH - 1; y <= yH + 1; ++y)
                    if (x >= 0 && x < xResH && y >= 0 && y < yResH)
                        heights[x, y] = h;

            int xM = (int)(self.z * xResA / td.size.z);
            int yM = (int)(self.x * yResA / td.size.x);
            for (int x = xM - 2; x <= xM + 2; ++x)
                for (int y = yM - 2; y <= yM + 2; ++y)
                    if (x >= 0 && x < xResA && y >= 0 && y < yResA) {
                        maps[x, y, 0] = 0.5f;
                        maps[x, y, 1] = 0.5f;
                    }
        }

        td.SetHeights(0, 0, heights);
        td.SetAlphamaps(0, 0, maps);
    }

    /** Adds more vertices to @var way to smooth it */
    void SmoothWay()
    {
        var newWay = new List<Vector3>();
        newWay.Add(way.First());

        for (int i = 1; i < way.Length - 1; ++i) {
            Vector3 cur = way[i] - way[i - 1], next = way[i + 1] - way[i];
            var orto = new Vector3(cur.z, cur.y, -cur.x);
            var shift = Vector3.Project(next.normalized, orto) * cur.magnitude;

            newWay.Add(way[i - 1] + cur * smoothPoint - shift * smoothDistanse);
            newWay.Add(way[i] + shift * smoothCorrection);
        }

        newWay.Add(way.Last());
        way = newWay.ToArray();
    }

    /** Makes @class Mesh according to @arg way */
    Mesh MakeWayMesh()
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
        return mesh;
    }

    Vector3 MakeWayNormal(int i)
    {
        var prev = (i == 0) ? 0 : (i - 1);
        var next = (i == way.Length - 1) ? (way.Length - 1) : (i + 1);

        var orto = (way[next] - way[prev]).normalized;
        return new Vector3(orto.z, orto.y, -orto.x);
    }
}
