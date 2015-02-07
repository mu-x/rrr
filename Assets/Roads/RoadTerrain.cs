using UnityEngine;
using System;
using System.Collections;

/** Terrain adjaster based on @var RoadCurve(s) */
[RequireComponent(typeof(Terrain))]
public class RoadTerrain : MonoBehaviour
{
    public float hillDelta = 0.01f;
    public float hillRadius = 1.5f;
    public int hillTexture = 1;
    public float hillAlphamap = 0.7f;

    float[,] stashHeights;
    float[, ,] stashAlphamap;

	void Start() 
    {
        var terrain = GetComponent<Terrain>();
        var td = terrain.terrainData;

        stashHeights = td.GetHeights(0, 0, td.heightmapWidth, td.heightmapHeight);
        stashAlphamap = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);

        foreach (var road in GetComponentsInChildren<RoadCurve>())
            AdjastTerrainToRoadMesh(td, road.roadWidth, Array.ConvertAll(
                road.meshFilter.mesh.vertices,
                p => p + road.transform.localPosition));
	}

    void OnDestroy() 
    {
        var terrain = GetComponent<Terrain>();
        terrain.terrainData.SetHeights(0, 0, stashHeights);
        terrain.terrainData.SetAlphamaps(0, 0, stashAlphamap);
	}

    void AdjastTerrainToRoadMesh(TerrainData data, float width, Vector3[] road)
    {
        int xResH = data.heightmapWidth, yResH = data.heightmapHeight;
        int xResA = data.alphamapWidth, yResA = data.alphamapHeight;
        var heights = data.GetHeights(0, 0, xResH, yResH);
        var alphamap = data.GetAlphamaps(0, 0, xResA, yResA);

        foreach (var point in road) {
            int xH = (int)(point.z * (float)xResH / data.size.z);
            int yH = (int)(point.x * (float)yResH / data.size.x);
            var h = (point.y - hillDelta) / data.size.y;
            int xA = (int)(point.z * xResA / data.size.z);
            int yA = (int)(point.x * yResA / data.size.x);

            int rH = (int)(width * hillRadius * (float)yResH / data.size.z);
            for (int x = xH - rH; x <= xH + rH; ++x)
                for (int y = yH - rH; y <= yH + rH; ++y)
                    if (x >= 0 && x < xResH && y >= 0 && y < yResH)
                        heights[x, y] = h;

            int rA = (int)(width * hillRadius * (float)yResA / data.size.z);
            for (int x = xA - rA; x <= xA + rA; ++x)
                for (int y = yA - rA; y <= yA + rA; ++y)
                    if (x >= 0 && x < xResA && y >= 0 && y < yResA) {
                        for (int i = 0; i < data.alphamapLayers; ++i)
                            alphamap[x, y, i] *= 1 - hillAlphamap;
                        alphamap[x, y, hillTexture] += hillAlphamap;
                    }
        }

        data.SetHeights(0, 0, heights);
        data.SetAlphamaps(0, 0, alphamap);
    }
}
