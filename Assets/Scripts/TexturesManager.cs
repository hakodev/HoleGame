using UnityEngine;
using System.Collections.Generic;
using Alteruna;
using System;
using Unity.Collections;
using System.Runtime.CompilerServices;

public class TexturesManager : MonoBehaviour
{
    public NativeArray<Color32> texturearray;

    public Texture2D currentTexture;
    private static TexturesManager instance;
    public int width;
    public int height;

    public static TexturesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<TexturesManager>();
            }
            return instance;
        }
    }


    public void SetTextureParams(NativeArray<Color32> array, int width, int height)
    {

        currentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texturearray = currentTexture.GetPixelData<Color32>(0);
        for (int i = 0; i < array.Length; i++)
        {
            texturearray[i] = array[i];
        }

    }


}
