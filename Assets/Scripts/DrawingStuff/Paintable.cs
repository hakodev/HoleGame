using UnityEngine;
using Alteruna;

public class Paintable : AttributesSync {
	const int TEXTURE_SIZE = 1024;

	public float extendsIslandOffset = 1;

	
    RenderTexture extendIslandsRenderTexture;

    RenderTexture uvIslandsRenderTexture;

    RenderTexture maskRenderTexture;

    RenderTexture supportTexture;


    Renderer rend;
	
	PaintManager paintManager;


	int maskTextureID = Shader.PropertyToID("_MaskTexture");

	public RenderTexture getMask() => maskRenderTexture;
	public RenderTexture getUVIslands() => uvIslandsRenderTexture;
	public RenderTexture getExtend() => extendIslandsRenderTexture;
	public RenderTexture getSupport() => supportTexture;
	public Renderer getRenderer() => rend;

	/*
    string oldTag;
    [SynchronizableMethod]
	public void SetTag(string newTag)
	{
		oldTag = gameObject.tag;
		gameObject.tag = newTag;
	}
	[SynchronizableMethod]
	public void ResetTag() {
		gameObject.tag = oldTag;
	}
	*/
    public void Start() {
		paintManager = FindAnyObjectByType<PaintManager>();
		maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
		maskRenderTexture.filterMode = FilterMode.Bilinear;

		extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
		extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

		uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
		uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

		supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
		supportTexture.filterMode =  FilterMode.Bilinear;

		rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

		paintManager.initTextures(this);
	}

	public void OnDisable(){
		maskRenderTexture.Release();
		uvIslandsRenderTexture.Release();
		extendIslandsRenderTexture.Release();
		supportTexture.Release();
	}
}