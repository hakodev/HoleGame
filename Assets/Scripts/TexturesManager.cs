using UnityEngine;
using System.Collections.Generic;
using Alteruna;

public class TexturesManager : AttributesSync
{
    public static List<Texture2D> allTextures = new List<Texture2D>();
    public static Texture2D currentTexture = null;

    private void Update()
    {
        if (currentTexture != null)
        {
            BroadcastRemoteMethod(nameof(SyncTex));
        }
    }

    [SynchronizableMethod]
    void SyncTex()
    {
        Texture2D currentTex = currentTexture;
        if (allTextures.Contains(currentTex))
        {
            return;
        }
        allTextures.Add(currentTex);
        
    }
}
