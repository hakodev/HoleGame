using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
public class ReconnectPrefab : MonoBehaviour
{
    [MenuItem("Assets/Reconnect Prefabs With Tag replacingThisObjectWithPrefab")] 
    private static void Reconnect()
    {
        //string prefabPath = "Assets/Meshes/Prefab/SM_sink.prefab";
        //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Vector3 storedScale = Vector3.zero;

        List<GameObject> foundObjects = GameObject.FindGameObjectsWithTag("replacingThisObjectWithPrefab").ToList();

        for (int i=0; i < foundObjects.Count; i++)
        {
            if (PrefabUtility.GetPrefabAssetType(foundObjects[i]) == PrefabAssetType.NotAPrefab) { continue; }

            //GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(foundObjects[i]);

            if (PrefabUtility.GetPrefabInstanceStatus(foundObjects[i]) == PrefabInstanceStatus.Connected)
            {
                storedScale = foundObjects[i].transform.localScale;
                PrefabUtility.RevertPrefabInstance(foundObjects[i], InteractionMode.UserAction);
                foundObjects[i].transform.localScale = storedScale;
                //EditorUtility.SetDirty(obj);
            }
            Debug.Log(PrefabUtility.GetPrefabInstanceStatus(foundObjects[i]) + " " + foundObjects[i].name);

        }

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}