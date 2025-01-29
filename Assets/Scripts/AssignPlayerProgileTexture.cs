using UnityEngine;

public class AssignPlayerProgileTexture : MonoBehaviour
{
    public Texture texture;
    private Renderer screenrenderer;
    void Start()
    {
        texture = Resources.Load<Texture>(UIInput.PlayerNameSync);
        screenrenderer = GetComponent<Renderer>();
        screenrenderer.material.SetTexture("_MaskTexture", texture);
    }

}
