using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DespawningItems : MonoBehaviour {

    public static void DespawnItem(GameObject heldObject) {
        heldObject.GetComponent<Collider>().enabled = false;
        Renderer[] renderers = heldObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.shader = Shader.Find("Shader Graphs/Glitch");
            renderer.material.DOFloat(4, "_GlitchStrength", 0.4f);
        }
        heldObject.transform.DOScale(new Vector3(0f, 0f, 0f), 0.4f);
        Debug.Log("Despawn symptom triggered!");
    }
}
