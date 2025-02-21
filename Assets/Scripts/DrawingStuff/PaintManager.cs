using UnityEngine;
using UnityEngine.Rendering;
using Alteruna;
using System;

public class PaintManager : AttributesSync{

    public Shader texturePaint;
    public Shader extendIslands;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");

    Material paintMaterial;
    Material extendMaterial;

    public GameObject[] stainPrefabs;

    public LayerMask mask;

    CommandBuffer command;
    private static PaintManager instance;

    public static PaintManager Instance
    { 
        get 
        { 
            if(instance == null)
            {
                instance = FindAnyObjectByType<PaintManager>();
            }

            return instance;
        } 
    }
    public void Awake(){
        
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        command = new CommandBuffer();
    }

    public GameObject GetStain()
    {
        return stainPrefabs[UnityEngine.Random.Range(0,stainPrefabs.Length)];
    }
  
    public void initTextures(Paintable paintable){
        //Paintable paintable = Multiplayer.GetGameObjectById(guid).GetComponent<Paintable>();
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    [SynchronizableMethod]
    public void paint(Guid guid, Vector3 pos, float radius = 1f, Color? color = null){
        Debug.Log(Multiplayer.GetUser().Name);

        Paintable paintable = Multiplayer.GetGameObjectById(guid).GetComponent<Paintable>();
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        RenderTexture kur = new RenderTexture(1024, 1024, 0);
       

        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, 1);
        paintMaterial.SetFloat(strengthID, 1);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID,support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        Debug.Log("prepUV " + paintMaterial.GetFloat(prepareUVID));
        Debug.Log(paintMaterial.GetVector(positionID));
        Debug.Log(paintMaterial.GetFloat(hardnessID));
        Debug.Log(paintMaterial.GetFloat(strengthID));
        Debug.Log(paintMaterial.GetFloat(radiusID));

        Debug.Log(command.name);
        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintMaterial, 0);
        
        command.SetRenderTarget(support);
        command.Blit(mask, support);

        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);
        

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();

    }

}