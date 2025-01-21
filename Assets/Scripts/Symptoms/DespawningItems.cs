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
            renderer.material.DOFloat(5, "_GlitchStrength", 0.4f);
 
        }

        
        Debug.Log("Despawn symptom triggered!");
    }

    public static IEnumerator DestroyItem(GameObject heldObject)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(heldObject);
    }
}
