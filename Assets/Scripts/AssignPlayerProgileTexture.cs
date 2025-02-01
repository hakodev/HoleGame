using Alteruna;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.NotBurstCompatible;

public class AssignPlayerProgileTexture : AttributesSync { 
    public Texture2D texture;
    private Renderer screenrenderer;
    [SynchronizableField] public Color32[] texturearray;

    private Alteruna.Avatar avatar;
    //NativeList<Color32> colors = new NativeList<Color32>();
    void Start()
    {
        
        screenrenderer = GetComponent<Renderer>();

        BroadcastRemoteMethod(nameof(SetTextureArray));

        
    }
    [SynchronizableMethod]
    public void SetTextureArray()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        if (!avatar.IsMe) return;
        
        texturearray = TexturesManager.Instance.texturearray;
        BroadcastRemoteMethod(nameof(SetDrawing));
    }

    [SynchronizableMethod]
    public void SetDrawing()
    {
        texture = new Texture2D(TexturesManager.Instance.width, TexturesManager.Instance.height, TextureFormat.RGBA32, false);


        //Unity.Collections.NotBurstCompatible.Extensions.CopyFromNBC<Color32>(colors, texturearray);

        texture.SetPixelData(texturearray, 0, 0);


        texture.Apply(false);


    }

    private void Update()
    {
        BroadcastRemoteMethod(nameof(SyncTexture));
    }

    

    [SynchronizableMethod]
    void SyncTexture()
    {
        if(avatar == null)
        {
            avatar = transform.root.GetComponent<Alteruna.Avatar>();
        }
       
        if (avatar.IsMe) return;

        if (screenrenderer == null)
        {

            screenrenderer = GetComponent<Renderer>();
        }
        screenrenderer.material.SetTexture("_MaskTexture", texture);
    }
}
