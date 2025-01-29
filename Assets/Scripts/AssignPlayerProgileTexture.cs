using UnityEngine;

public class AssignPlayerProgileTexture : MonoBehaviour
{
    public Texture2D texture;
    private Renderer screenrenderer;
    void Start()
    {
        texture = Resources.Load<Texture2D>(UIInput.PlayerNameSync);
        screenrenderer = GetComponent<Renderer>();
        screenrenderer.material.SetTexture("_MaskTexture", texture);
    }

}
