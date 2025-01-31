using UnityEngine;

public class UpdateMeshColliderToAnimation : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    MeshCollider meshCollider;
    Mesh bakedMesh;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        bakedMesh = new Mesh();
    }

    void LateUpdate()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh); // Bake the current animated shape
        meshCollider.sharedMesh = null; // Reset
        meshCollider.sharedMesh = bakedMesh; // Apply updated mesh
    }
}
