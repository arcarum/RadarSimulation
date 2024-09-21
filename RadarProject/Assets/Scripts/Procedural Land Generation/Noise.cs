using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Static since 1 instance only, and no MonoBehaviour since we are not applying this 
// to any object in our scene
public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) scale = 0.0001f; // Avoid div by zero

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Go over each pixel and assign a value based on Perlin noise
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1; // The higher -> the further the sample pts are -> height values will change more rapidly 
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency; // we do not want to use int values since perlin noise 
                    float sampleY = y / scale * frequency; // will give us the same value each time

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // * 2 - 1 to get -ve values sometimes to dec the noiseHeight
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; // dec over time
                    frequency *= lacunarity; // inc over time
                }
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize noiseMap to have values b/w 0 & 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
