using UnityEngine;

public class UpdateMeshColliderToAnimation : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    MeshCollider meshCollider;
    Mesh bakedMesh;

    Vector3 lastPos;
    [SerializeField] int updateColliderEveryNUpdate=5;
    int currentUpdate = 0;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        bakedMesh = new Mesh();
        lastPos = transform.position;
    }

    void LateUpdate()
    {
        if((transform.position - lastPos).magnitude > 0.1f)
        {
            currentUpdate++;
            if(currentUpdate%updateColliderEveryNUpdate==0)
            {
                skinnedMeshRenderer.BakeMesh(bakedMesh);
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = bakedMesh;
            }
        }
    }
}
