using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, ColourMap, Mesh
    };

    public float meshHighMultiplier;
    public AnimationCurve meshHeightCurve;
    public DrawMode drawMode;
    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public TerrainType[] regions;
    public GameObject treePrefab;
    public float minTreeHeight;
    public float maxTreeHeight;
    public int numberOfTrees;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset);
        Texture2D[] textureMap = new Texture2D[regions.Length];

        for (int i = 0; i < regions.Length; i++)
        {
            textureMap[i] = regions[i].texture;
        }

        Texture2D finalTexture = new Texture2D(mapChunkSize, mapChunkSize);

        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int k = 0; k < regions.Length; k++)
                {
                    if (currentHeight <= regions[k].height)
                    {
                        finalTexture.SetPixel(x, y, textureMap[k].GetPixel(x % textureMap[k].width, y % textureMap[k].height));
                        break;
                    }
                }
            }
        }

        finalTexture.Apply();

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColourMap)
        {
            mapDisplay.DrawTexture(finalTexture);
        }
        else if (drawMode == DrawMode.Mesh)
        {
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHighMultiplier, meshHeightCurve, levelOfDetail), finalTexture);
        }
        GenerateTrees(noiseMap);
    }
    private void GenerateTrees(float[,] noiseMap)
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            float randomX = Random.Range(-40f, 30f);
            float randomZ = Random.Range(-38f, 33f);

            int xIndex = Mathf.Clamp((int)randomX, 0, noiseMap.GetLength(0) - 1);
            int zIndex = Mathf.Clamp((int)randomZ, 0, noiseMap.GetLength(1) - 1);

            float currentHeight = noiseMap[xIndex, zIndex];

            if (currentHeight >= minTreeHeight && currentHeight <= maxTreeHeight)
            {
                Vector3 position = new Vector3(randomX, 3300f, randomZ);
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                Instantiate(treePrefab, position, rotation);
            }
        }
    }



    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Texture2D texture;
}