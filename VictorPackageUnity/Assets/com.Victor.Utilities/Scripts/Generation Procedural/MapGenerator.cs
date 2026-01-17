using System;
using com.Victor.Utilities.Scripts.Generation_Procedural;
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

    public TerrainType[] regions;

    public DrawMode drawMode;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistance,lacunarity,offset);
        Color[] colourMap = new Color[mapWidth * mapHeight];
        
        for (int y = 0; y < mapHeight; y++)
        for (int x = 0; x < mapWidth; x++)
        {
            float currentHeight = noiseMap[x, y];
            
            for (int i = 0; i < regions.Length; i++)
            {
                if (currentHeight <= regions[i].Height)
                {
                    colourMap[y * mapWidth + x] = regions[i].Colour;
                    break;
                }    
            }
        }
        
        
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;
            case DrawMode.ColourMap:
                display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap,mapWidth,mapHeight));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap),TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                break;
        }
        
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
