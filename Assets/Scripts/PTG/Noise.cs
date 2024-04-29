using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                // obliczanie wartosci szumu dla piksela
                for (int k = 0; k < octaves; k++)
                {

                    float temp_x = (j - halfWidth) / scale * frequency + octaveOffsets[k].x;
                    float temp_y = (i - halfHeight) / scale * frequency + octaveOffsets[k].y;
                    float perlinValue = Mathf.PerlinNoise(temp_x, temp_y) * 2 - 1;
                    //noiseMap[j, i] = perlinValue;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[j, i] = noiseHeight;
            }
        }
        // normalizacja szumu
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                noiseMap[j, i] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[j, i]);
            }
        }
        return noiseMap;
    }

}
