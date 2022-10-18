using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public static class Noise
    {
        public static float[,] ComplexNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity)
        {
            //initialise the noiseMap array
            float[,] noiseMap = new float[mapWidth, mapHeight];

            //random generation using a seed
            System.Random prng = new System.Random(seed);

            //octave offsets so that points in each octave are sampled from different areas
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                //too high a value will break the generation, this seems like a good range
                float offsetX = prng.Next(-100000, 100000);
                float offsetY = prng.Next(-100000, 100000);

                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0) scale = 0.01f; //without this, if we attempt to divide by 0 or lower it would give us an error

            //keeping track of the min and max noise height
            float maxNoiseHeight = float.MinValue, minNoiseHeight = float.MaxValue;

            //calculating the center of the noise map so that we can zoom in/out at the center rather than the top right corner
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            //nested arrays baby!
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //variables for amplitude, frequency and noise height
                    float amplitude = 1, frequency = 1, noiseHeight = 0;

                    for (int o = 0; o < octaves; o++)
                    {
                        //generating sample values, divided by scale and multiplied by frequency and sampling the octaveOffsets
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[o].x * frequency;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[o].y * frequency;

                        //generating perlin noise values
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        //increase noise height by perlin value of each octave
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    //updating the min and max noise height
                    if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }

            //more nested arrays! this time we're normalizing the values
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //normalize our noise map by inverse lerping between the min and max noise height
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            //return the noiseMap array
            return noiseMap;
        }

        public static float[,] SimpleNoiseMap(int mapWidth, int mapHeight, float scale)
        {
            //initialise the noiseMap array
            float[,] noiseMap = new float[mapWidth, mapHeight];

            if (scale <= 0) scale = 0.01f; //without this, if we attempt to divide by 0 or lower it would give us an error

            //nested arrays baby!
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //generating sample values, divided by scale
                    float sampleX = x / scale;
                    float sampleY = y / scale;

                    //generating perlin noise values
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseMap[x, y] = perlinValue;
                }
            }

            //return the noiseMap array
            return noiseMap;
        }
    }
}