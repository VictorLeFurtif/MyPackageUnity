using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public MapDisplay display;

    public int octaves;
    [Range(0,1)] public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    [HideInInspector] public bool AutoUpdate = false;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistance,lacunarity,offset);
        
        display.DrawNoiseMap(noiseMap);
    }

    private void OnValidate()
    {
        if (mapWidth < 1 ) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

    public void ResetValue()
    {
        mapWidth = 100;
        mapHeight= 100;
        
        noiseScale = 10f;

        octaves = 4;

        persistance = 0.25f;

        lacunarity = 2;

        seed = 0;

        offset = Vector2.zero;
    }
}
