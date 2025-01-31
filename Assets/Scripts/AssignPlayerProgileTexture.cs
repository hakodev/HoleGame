using Alteruna;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AssignPlayerProgileTexture : AttributesSync { 
    public Texture2D texture;
    private Renderer screenrenderer;
    [SynchronizableField] public NativeArray<Color32> texturearray;

    private Alteruna.Avatar avatar;

    void Start()
    {
        
        screenrenderer = GetComponent<Renderer>();

        BroadcastRemoteMethod(nameof(SetTextureArray));

        BroadcastRemoteMethod(nameof(SetDrawing));
    }
    [SynchronizableMethod]
    public void SetTextureArray()
    {
        if (!Multiplayer.GetAvatar().IsMe) return;
        texturearray = TexturesManager.Instance.texturearray;
    }

    [SynchronizableMethod]
    public void SetDrawing()
    {
        texture = new Texture2D(TexturesManager.Instance.width, TexturesManager.Instance.height, TextureFormat.RGBA32, false);

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
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        if (avatar.IsMe) return;
        screenrenderer = GetComponent<Renderer>();
        screenrenderer.material.SetTexture("_MaskTexture", texture);
    }
}
