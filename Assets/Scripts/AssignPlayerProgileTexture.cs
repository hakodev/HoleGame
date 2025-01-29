using Alteruna;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayerProgileTexture : AttributesSync { 
    public Texture2D texture;
    private Renderer screenrenderer;


    private Alteruna.Avatar avatar;
    void Start()
    {

        
    }

    [SynchronizableMethod]
    public void SetDrawing(string name)
    {
        if (!Multiplayer.GetAvatar().IsMe) return;
        foreach (var texture2d in TexturesManager.allTextures)
        {
            Debug.Log("tex2d " + texture2d.name);
            if(texture2d.name == name)
            {
                texture = texture2d;
            }
        }
        screenrenderer = GetComponent<Renderer>();
        screenrenderer.material.SetTexture("_MaskTexture", texture);
    }

}
