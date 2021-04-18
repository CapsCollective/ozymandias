using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NaughtyAttributes;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] private float scale;
    [SerializeField] private int noiseWidth;
    [SerializeField] private int noiseHeight;

    [Button]
    public void GenerateNoiseTexture()
    {
        Texture2D noiseTex = new Texture2D(noiseWidth, noiseHeight);
        Color[] colors = new Color[noiseWidth * noiseHeight];

        for (int x = 0; x < noiseWidth; x++)
        {
            for (int y = 0; y < noiseHeight; y++)
            {
                float xCoord = (float)x / noiseWidth * scale;
                float yCoord = (float)y / noiseHeight * scale;

                float sample = Mathf.Lerp(1, 0,Mathf.PerlinNoise(xCoord, yCoord));
                colors[(int)y * noiseWidth + (int)x] = new Color(sample, sample, sample);
            }
        }

        noiseTex.SetPixels(colors);
        noiseTex.Apply();
        File.WriteAllBytes($"{Application.dataPath}/Textures/NoiseTex.png", noiseTex.EncodeToPNG());
    }
}
